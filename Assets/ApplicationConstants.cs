using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// general constants we'd like access to in the game

public class ApplicationConstants : MonoBehaviour {

    // screen height and width just cuz
    public static int SCREEN_WIDTH, SCREEN_HEIGHT, 
                        HALF_SCREEN_WIDTH, HALF_SCREEN_HEIGHT;

    public static int TILE_WIDTH = 108, TILE_HEIGHT = 125;

    // max dungeon size in tiles?
    public static int DUNGEON_WIDTH = 16, DUNGEON_HEIGHT = 16;

    public void Awake()
    {
        SCREEN_WIDTH = Screen.width;
        HALF_SCREEN_WIDTH = SCREEN_WIDTH / 2;

        SCREEN_HEIGHT = Screen.height;
        HALF_SCREEN_HEIGHT = SCREEN_HEIGHT / 2;

    }
}
