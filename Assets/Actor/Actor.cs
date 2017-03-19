﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class Actor : MonoBehaviour {
    
    public int x, y;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        /*
        if(inMotion)
        {
            position = Vector3.Lerp(position, destination, Time.deltaTime);
            if(position == destination) {
                inMotion = false;
                DungeonManager.moveActor(currentGridX, currentGridY, lastGridX, lastGridY);
            }
            Debug.Log("motioning");
        }
        */
    }

    public bool trySpawnAt(int x, int y)
    {
        if (moveToPosition(x, y) != 0)
        {
            return false;
        }
        return true;
    }

    // -1 = error moving
    // 0 = moved
    // 1 = combat
    public int moveToPosition(int nx, int ny)
    {
        if(nx >= 0 && nx < ApplicationConstants.DUNGEON_WIDTH &&
            ny >= 0 && ny < ApplicationConstants.DUNGEON_HEIGHT)
        {
            var occupier = DungeonManager.WorldGrid[nx, ny].Actor;
            if (occupier == null && DungeonManager.WorldGrid[nx,ny].Cost < 10000)
            {
                DungeonManager.moveActor(this, nx, ny);
                x = nx;
                y = ny;
                return 0;
            }
            else if(occupier != null)
            {
                return 1;
            }
        }
        return -1;
    }

    public static GameObject actorFromId(int id)
    {
        switch (id)
        {
            case 99:
                return GameObject.Instantiate(Resources.Load("Actors/Player")) as GameObject;
        }
        return null;
    }
}