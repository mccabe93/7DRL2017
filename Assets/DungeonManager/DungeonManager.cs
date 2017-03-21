/*
*TODO
*-Add "GameManager" tag to whatever gameobject this is attached to
*-Add WorldCoordinate calculation when building the grid
*-Update logic for keeping this grid up to date as the game changes?
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class DungeonManager : MonoBehaviour
{

    protected struct Movement
    {
        public Actor actor;
        public Vector3 position;
        public Vector3 destination;
        public Vector2i start;
        public Vector2i end;
        public int sortingOrder;
        public float snapPositionTime;
    }

    public static DungeonTile[,] WorldGrid;
    public bool spawnEnemy = false;

    private DungeonGenerator dg = new DungeonGenerator();

    private static GameObject player;
    private static GameObject mainCamera;

    private static Movement mover;

    // Use this for initialization
    void Start()
    {
        dg.init(Random.Range(0,5));
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        player = GameObject.Instantiate(Resources.Load("Actors/Player")) as GameObject;
        WorldGrid = new DungeonTile[ApplicationConstants.DUNGEON_WIDTH, ApplicationConstants.DUNGEON_HEIGHT];
        for (int i = 0; i < ApplicationConstants.DUNGEON_WIDTH; i++)
        {
            for (int j = 0; j < ApplicationConstants.DUNGEON_HEIGHT; j++)
            {
                WorldGrid[i, j] = new DungeonTile(null, null, dg.costGrid[i, j]);
            }
        }
        Vector2i spawn = dg.getSpawnableTile();
        addActor(player, spawn.y, spawn.x);
        renderWorld();
        drawActors();
        mainCamera.GetComponent<CameraFollow>().init();
    }

    public void renderWorld()
    {
        var map = dg.map;
        for (int i = 0; i < ApplicationConstants.DUNGEON_WIDTH; i++)
        {
            for (int j = 0; j < ApplicationConstants.DUNGEON_HEIGHT; j++)
            {
                DungeonTile current_Tile = WorldGrid[i, j];
                current_Tile.Tile = DungeonGenerator.tileFromId(map[i, j]);
                current_Tile.Tile.transform.position = PerspectiveMap.renderPerspective(i, j);
                current_Tile.Tile.GetComponent<SpriteRenderer>().sortingOrder = j * ApplicationConstants.DUNGEON_WIDTH - i - 1;
                WorldGrid[i, j] = current_Tile;

            }
        }
    }

    public void drawActors()
    {
        for (int i = 0; i < ApplicationConstants.DUNGEON_WIDTH; i++)
        {
            for (int j = 0; j < ApplicationConstants.DUNGEON_HEIGHT; j++)
            {
                if (WorldGrid[i, j].Actor != null && ((mover.actor != null && !WorldGrid[i, j].Actor.Equals(mover.actor.gameObject)) 
                    || mover.actor == null))
                {
                    drawActorAtPosition(i, j);
                }
            }
        }
        if (mover.actor != null)
        {
            if (!WorldGrid[mover.start.x, mover.start.y].moving)
            {
                Debug.Log("mover done movin");
                moveActor(mover.start, mover.end);
                mover.actor.returnToIdle();
                mover.actor = null;
            }
            else
            {
                WorldGrid[mover.start.x, mover.start.y].moving = animateMoverTowardDestination();// || mover.snapPositionTime <= Time.time;

                GameObject actor = mover.actor.gameObject;

                actor.transform.position = mover.position;
            }
        }
    }

    public bool animateMoverTowardDestination()
    {
        if (mover.actor == null)
            return false;

        if (Vector3.Distance(mover.position, mover.destination) >= 3f)
        {
            mover.position = Vector3.Lerp(mover.position, mover.destination, Time.deltaTime * 10f);

        }
        else
        {
            Debug.Log("mover should be done now");
            mover.position = mover.destination;
            return false;
        }

        return true;
    }

    void Update()
    {
        mainCamera.GetComponent<CameraFollow>().updateCamera();
        player.GetComponent<Player>().playerControls();
        drawActors();
    }

    public void drawActorAtPosition(int i, int j)
    {
        var actor = WorldGrid[i, j].Actor;
        actor.transform.position = PerspectiveMap.renderPerspective(i + 1, j);
        actor.transform.position += actor.GetComponent<SpriteRenderer>().sprite.bounds.extents.y * Vector3.up;
        actor.transform.position -= actor.GetComponent<SpriteRenderer>().sprite.bounds.extents.x / 2 * Vector3.right;
        actor.GetComponent<SpriteRenderer>().sortingOrder = getSortingOrder(i, j);
    }

    public static Vector3 getCenteredActorPosition(GameObject actor, Vector3 currentPosition)
    {
        Vector3 spriteExtents = actor.GetComponent<SpriteRenderer>().sprite.bounds.extents;
        return new Vector3(currentPosition.x - spriteExtents.x / 2, currentPosition.y + spriteExtents.y);
    }

    public static int getSortingOrder(int i, int j)
    {
        return (j * ApplicationConstants.DUNGEON_WIDTH - i) + 1;
    }

    public static bool moveActor(Actor actor, int dX, int dY)
    {
        if (mover.actor != null)
        {
            Debug.Log("moving too fast");
            return false;
        }

        mover.actor = actor;

        // grid x and y
        mover.start = new Vector2i(actor.x, actor.y);
        mover.end = new Vector2i(dX, dY);

        // drawn x and y
        mover.position = getCenteredActorPosition(actor.gameObject, PerspectiveMap.renderPerspective(actor.x + 1, actor.y));
        mover.destination = getCenteredActorPosition(actor.gameObject, PerspectiveMap.renderPerspective(dX + 1, dY));

        mover.snapPositionTime = Time.time + 0.4f;

        mover.sortingOrder = Mathf.Max(getSortingOrder(mover.start.x, mover.start.y), getSortingOrder(mover.end.x, mover.end.y));

        mover.actor.gameObject.GetComponent<SpriteRenderer>().sortingOrder = mover.sortingOrder;

        WorldGrid[actor.x, actor.y].moving = true;

        return true;
    }

    public static void moveActor(Vector2i start, Vector2i end)
    {
        GameObject actor = WorldGrid[start.x, start.y].Actor;
        WorldGrid[start.x, start.y].Actor = null;
        WorldGrid[end.x, end.y].Actor = actor;
        //        Debug.Log(newX + ", " + newY);
        if (actor != null && actor.tag.Equals("Player"))
        {
            //            enemy.GetComponent<Enemy>().canMove = true;
        }
    }

    public static Vector3 getDestination(int x, int y)
    {
        return PerspectiveMap.renderPerspective(x, y);
    }

    public void addActor(GameObject actor, int x, int y)
    {
        //actor.GetComponent<SpriteMovement>().MoveToPosition(x, y);
        WorldGrid[x, y].Actor = actor;
        actor.GetComponent<Actor>().x = x;
        actor.GetComponent<Actor>().y = y;
        drawActors();
    }


    /*
   * @param Vector2 position we are checking
   * @returns GameObject Actor that resides in said position if applicable
   * @desc Gets all GameObjects, then checks to see if any reside at the given position
   * @status Untested
   */
    private GameObject getActor(int x, int y)
    {
        return WorldGrid[x, y].Actor;
    }


    public class DungeonTile
    {
        public GameObject Tile;
        public GameObject Actor { get; set; }
        public int Cost { get; set; }
        public bool moving = false;

        public DungeonTile() { }

        public DungeonTile(GameObject tile, GameObject act, int cos)
        {
            this.Actor = act;
            this.Cost = cos;
            this.Tile = tile;
        }
    }
}
