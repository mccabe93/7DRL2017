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

    private GameObject player;

    public int spawnX = 13, spawnY = 0;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.Instantiate(Resources.Load("Actors/Player")) as GameObject;
        dg = new DungeonGenerator();
        dg.init();
        WorldGrid = new DungeonTile[ApplicationConstants.DUNGEON_WIDTH, ApplicationConstants.DUNGEON_HEIGHT];
        for (int i = 0; i < ApplicationConstants.DUNGEON_WIDTH; i++)
        {
            for (int j = 0; j < ApplicationConstants.DUNGEON_HEIGHT; j++)
            {
                WorldGrid[i, j] = new DungeonTile(null, null, 0);
            }
        }
        player.GetComponent<Actor>().moveToPosition(spawnX, spawnY);
        addActor(player, spawnX, spawnY);//player.GetComponent<SpriteMovement>().x_PositionInDungeonGrid, player.GetComponent<SpriteMovement>().y_PositionInDungeonGrid);
        //        PerspectiveMap.renderDungeon(dg.map);
        renderWorld();
        drawActors();
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
                current_Tile.Tile.GetComponent<SpriteRenderer>().sortingOrder = j;
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
        player.GetComponent<Actor>().playerControls();
        drawActors();
    }

    public void drawActorAtPosition(int i, int j)
    {
        var actor = WorldGrid[i, j].Actor;
        actor.transform.position = PerspectiveMap.renderPerspective(i, j);
        actor.transform.position += actor.GetComponent<SpriteRenderer>().sprite.bounds.extents.y * Vector3.up;
        actor.transform.position -= actor.GetComponent<SpriteRenderer>().sprite.bounds.extents.x / 2 * Vector3.right;
        actor.GetComponent<SpriteRenderer>().sortingOrder = j+1;
    }

    public static void moveActor(int oldX, int oldY, int newX, int newY)
    {
        GameObject actor = WorldGrid[oldX, oldY].Actor;
        WorldGrid[oldX, oldY].Actor = null;
        WorldGrid[newX, newY].Actor = actor;
        Debug.Log(newX + ", " + newY);
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
