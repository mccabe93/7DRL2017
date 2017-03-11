/*
*TODO
*-Add "DungeonManager" tag to whatever gameobject this is attached to
*-Add WorldCoordinate calculation when building the grid
*-Update logic for keeping this grid up to date as the game changes?
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour {

    public static DungeonTile[,] WorldGrid;

    private DungeonGenerator dg;

    private static GameObject player, enemy;
    private static GameObject mainCamera;

    public int spawnX, spawnY;
    public int enemySpawnX, enemySpawnY;

    // Use this for initialization
    void Start ()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        player = GameObject.Instantiate(Resources.Load("Actors/Player")) as GameObject;
        enemy = GameObject.Instantiate(Resources.Load("Actors/Enemy") as GameObject);
        dg = new DungeonGenerator();
        dg.init();
        Pathfinder.environmentMap = dg.costGrid;
        WorldGrid = new DungeonTile[ApplicationConstants.DUNGEON_WIDTH, ApplicationConstants.DUNGEON_HEIGHT];
        for (int i = 0; i < ApplicationConstants.DUNGEON_WIDTH; i++)
        {
            for (int j = 0; j < ApplicationConstants.DUNGEON_HEIGHT; j++)
            {
                WorldGrid[i, j] = new DungeonTile(null, null, dg.costGrid[i,j]);
            }
        }
        player.GetComponent<Player>().moveToPosition(spawnX, spawnY);
        enemy.GetComponent<Enemy>().moveToPosition(enemySpawnX, enemySpawnY);
        addActor(player, spawnX, spawnY);//player.GetComponent<SpriteMovement>().x_PositionInDungeonGrid, player.GetComponent<SpriteMovement>().y_PositionInDungeonGrid);
        addActor(enemy, enemySpawnX, enemySpawnY);
        //        PerspectiveMap.renderDungeon(dg.map);
        renderWorld();
        drawActors();
        Pathfinder test = new Pathfinder(player.GetComponent< Actor>().x, player.GetComponent<Actor>().y);
        try {
            test.findPath(13, 3);
            test.printPath();
        }catch(System.Exception e)
        {
            Debug.Log("could not create a path to player");
        }
        mainCamera.GetComponent<CameraFollow>().init();
    }

    public void renderWorld()
    {
        var map = dg.map;
        for (int i = 0; i < ApplicationConstants.DUNGEON_WIDTH; i++)
        {
            for (int j = 0; j < ApplicationConstants.DUNGEON_HEIGHT; j++)
            {
                DungeonTile current_Tile = WorldGrid[i,j];
                current_Tile.Tile = DungeonGenerator.tileFromId(map[i, j]);
                current_Tile.Tile.transform.position = PerspectiveMap.renderPerspective(i, j);
                current_Tile.Tile.GetComponent<SpriteRenderer>().sortingOrder = j*ApplicationConstants.DUNGEON_WIDTH - i - 1;
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
                if (WorldGrid[i, j].Actor != null)
                {
                    drawActorAtPosition(i, j);
                }
            }
        }
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
        actor.GetComponent<SpriteRenderer>().sortingOrder = j * ApplicationConstants.DUNGEON_WIDTH - i + 1;
    }

    public static void moveActor(int oldX, int oldY, int newX, int newY)
    {
        mainCamera.GetComponent<CameraFollow>().updateCamera();
        GameObject actor = WorldGrid[oldX, oldY].Actor;
        WorldGrid[oldX, oldY].Actor = null;
        WorldGrid[newX, newY].Actor = actor;
        Debug.Log(newX + ", " + newY);
        if(actor != null && actor.tag.Equals("Player"))
        {
            enemy.GetComponent<Enemy>().canMove = true;
        }
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

        public DungeonTile() { }

        public DungeonTile(GameObject tile, GameObject act, int cos)
        {
            this.Actor = act;
            this.Cost = cos;
            this.Tile = tile;
        }
    }
}
