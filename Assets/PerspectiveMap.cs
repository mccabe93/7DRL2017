using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveMap : MonoBehaviour {

    DungeonGenerator dg = new DungeonGenerator();

    double isoProejction = 1 / Mathf.Sqrt(6) * (ApplicationConstants.TILE_HEIGHT * Mathf.Cos(Mathf.Deg2Rad * 45f));

    // Use this for initialization
    void Start () {
        dg.init();
        renderDungeon();
//        dg.renderDungeon();
        
    }
    public void renderDungeon()
    {
        var map = dg.map;
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0, di = 0; j < map.GetLength(1); j++, di++)
            {
                //if (!(i == 0 && j == 7))
                {
                    GameObject tmp = null;
                    if (map[i, j] == 0)
                    {
                        tmp = GameObject.Instantiate(Resources.Load("Environment/iso/Wall")) as GameObject;
                    }
                    else if (map[i, j] == 1)
                    {
                        tmp = GameObject.Instantiate(Resources.Load("Environment/iso/Floor")) as GameObject;
                    }
                    tmp.transform.Translate((j + i) * 54, (float)(di * isoProejction), 0);
                    tmp.GetComponent<SpriteRenderer>().sortingOrder = -i;
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
