/*
*TODO
*-Add "DungeonManager" tag to whatever gameobject this is attached to
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
        public float snapPositionTime;
    }

    public static DungeonTile[,] WorldGrid;
    public bool spawnEnemy = false;

    private DungeonGenerator dg;

    private static GameObject player;
    private static GameObject mainCamera;

    private static Movement mover;

    public int spawnX, spawnY;

    // Use this for initialization
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        player = GameObject.Instantiate(Resources.Load("Actors/Player")) as GameObject;
        dg = new DungeonGenerator();
        dg.init();
        Pathfinder.environmentMap = dg.costGrid;
        WorldGrid = new DungeonTile[ApplicationConstants.DUNGEON_WIDTH, ApplicationConstants.DUNGEON_HEIGHT];
        for (int i = 0; i < ApplicationConstants.DUNGEON_WIDTH; i++)
        {
            for (int j = 0; j < ApplicationConstants.DUNGEON_HEIGHT; j++)
            {
                WorldGrid[i, j] = new DungeonTile(null, null, dg.costGrid[i, j]);
            }
        }
        player.GetComponent<Player>().moveToPosition(spawnX, spawnY);
        addActor(player, spawnX, spawnY);
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
                if (WorldGrid[i, j].Actor != null && !WorldGrid[i,j].Actor.Equals(mover.actor.gameObject))
                {
                    drawActorAtPosition(i, j);
                }
            }
        }
        if (mover.actor != null)
        {
            if (!WorldGrid[mover.start.x, mover.start.y].moving)
            {
                mover.actor = null;
                moveActor(mover.start, mover.end);
                Debug.Log("mover done movin");
            }
            else
            {
                WorldGrid[mover.start.x, mover.start.y].moving = animateMoverTowardDestination() || mover.snapPositionTime <= Time.time;

                GameObject actor = mover.actor.gameObject;
                actor.GetComponent<SpriteRenderer>().sortingOrder = getSortingOrder(mover.end.x, mover.end.y);
                actor.transform.position = mover.position;
            }
        }
    }

    public bool animateMoverTowardDestination()
    {
        if (mover.actor == null)
            return false;

        float moveTime = Time.deltaTime / 5.0f;

        mover.position = Vector3.Lerp(mover.position, mover.destination, Time.deltaTime);

        if (mover.position == mover.destination)
            return false;

        return true;
    }

    void Update()
    {
        player.GetComponent<Player>().playerControls();
        drawActors();
    }

    public void drawActorAtPosition(int i, int j)
    {
        var actor = WorldGrid[i, j].Actor;
        actor.transform.position = PerspectiveMap.renderPerspective(i+1, j);
        actor.transform.position += actor.GetComponent<SpriteRenderer>().sprite.bounds.extents.y * Vector3.up;
        actor.transform.position -= actor.GetComponent<SpriteRenderer>().sprite.bounds.extents.x / 2 * Vector3.right;
        actor.GetComponent<SpriteRenderer>().sortingOrder = getSortingOrder(i, j);
    }

    public int getSortingOrder(int i, int j)
    {
        return j * ApplicationConstants.DUNGEON_WIDTH - i + 1; 
    }

    public static void moveActor(Actor actor, int dX, int dY)
    {
        mover.actor = actor;
        // grid x and y
        mover.start = new Vector2i(actor.x, actor.y);
        mover.end = new Vector2i(dX, dY);

        // drawn x and y
        mover.position = PerspectiveMap.renderPerspective(actor.x+1, actor.y);
        mover.destination = PerspectiveMap.renderPerspective(dX, dY);

        mover.snapPositionTime = Time.time + 0.4f;

        WorldGrid[actor.x, actor.y].moving = true;
    }

    public static void moveActor(Vector2i start, Vector2i end)
    {
        mainCamera.GetComponent<CameraFollow>().updateCamera();
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
        Debug.Log(actor + " added to " + WorldGrid[x, y]);
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
