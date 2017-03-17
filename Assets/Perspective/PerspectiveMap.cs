using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveMap : MonoBehaviour {

    DungeonGenerator dg = new DungeonGenerator();

    private static double isoProjection = 1 / Mathf.Sqrt(6) * ((ApplicationConstants.TILE_WIDTH) * 1/Mathf.Sqrt(2));

    private static int halfTileWidth = (ApplicationConstants.TILE_WIDTH) / 2;

    // Use this for initialization
    void Start () {
//        dg.init();
//        renderDungeon();
//        Debug.Log("iso projection = " + isoProjection);
//        dg.renderDungeon();
        
    }

    public static void renderDungeon(int[,] map)
    {
        /*
        // small test map
        var map = new int[2,4];
        map[0,0] = 0;
        map[0,1] = 1;
        map[0,2] = 0;
        map[0,3] = 1;
        map[1, 0] = 0;
        map[1, 1] = 1;
        map[1, 2] = 0;
        map[1, 3] = 1;
        */
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                float offset = (float)(i * isoProjection);
                GameObject tmp = null;
                if (map[i, j] == 0)
                {
                    tmp = GameObject.Instantiate(Resources.Load("Environment/iso/Wall")) as GameObject;
                }
                else if (map[i, j] == 1)
                {
                    tmp = GameObject.Instantiate(Resources.Load("Environment/iso/Floor")) as GameObject;
                }
                    
                tmp.transform.position = new Vector3(j * halfTileWidth + i * halfTileWidth, (float)((j * isoProjection) - offset), 0);
                tmp.GetComponent<SpriteRenderer>().sortingOrder = i;
            }
        }
    }

    public static Vector3 renderPerspective(int x, int y)
    {
        float offset = (float)(y * isoProjection);
        return new Vector3(x * halfTileWidth + y * halfTileWidth, (float)((x * isoProjection) - offset), 0);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
