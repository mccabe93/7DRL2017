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

    public DungeonTile[,] WorldGrid;

	// Use this for initialization
	void Start ()
    {
        WorldGrid = new DungeonTile[ApplicationConstants.DUNGEON_WIDTH, ApplicationConstants.DUNGEON_HEIGHT];
        for (int i = 0; i < ApplicationConstants.DUNGEON_WIDTH; i++)
        {
            for (int j = 0; j < ApplicationConstants.DUNGEON_HEIGHT; j++)
            {
                DungeonTile current_Tile = new DungeonTile();
                current_Tile.DungeonPosition = new Vector2(i, j);   //DOES THIS NEED A MULTIPLIER?
                current_Tile.Actor = getActor(current_Tile.DungeonPosition);    //See if any GameObjects are at this position, and assign actor if so
                //current_Tile.WorldPosition = //NEED TO SET WORLD POSITION
                //current_Tile.Cost = //NEED TO SET COST
                WorldGrid[i, j] = current_Tile;
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
	    	
	}

    /*
   * @param Vector2 position we are checking
   * @returns GameObject Actor that resides in said position if applicable
   * @desc Gets all GameObjects, then checks to see if any reside at the given position
   * @status Untested
   */
    private GameObject getActor(Vector2 pos)
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.activeInHierarchy)
            {
                Vector2 toVector2 = new Vector2(go.gameObject.transform.position.x, go.gameObject.transform.position.y);
                if (toVector2 == pos)   //Is this a valid check? Will GameObject's position be comparable to grid?
                    return go;
            }
        }

        return null;
    }


    public class DungeonTile
    {
        public Vector2 DungeonPosition { get; set; }
        public Vector2 WorldPosition { get; set; }
        public GameObject Actor { get; set; }
        public int Cost { get; set; }

        public DungeonTile() { }

        public DungeonTile(Vector2 dungeonPos, GameObject act, int cos, Vector2 worldPos)
        {
            this.DungeonPosition = dungeonPos;
            this.Actor = act;
            this.Cost = cos;
            this.WorldPosition = worldPos;
        }
    }
}
