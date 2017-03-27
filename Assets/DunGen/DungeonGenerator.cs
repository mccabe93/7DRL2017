﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

public class DungeonGenerator : MonoBehaviour {

    // constants for generating our automata
    public int automata_BirthLimit = 4, automata_DeathLimit = 3, automata_Generations = 2;
    public float automata_InitalAlivePercent = 0.7f;

    public int[,] map { get; private set; }

    public int maxRoomConnections = 3;

    public Sprite wall;
    public Sprite ground;

    // to not devour enormouse amounts of memory, just build the cost grid after completion?
    public int[,] costGrid;// = new int[ApplicationConstants.DUNGEON_HEIGHT][ApplicationConstants.DUNGEON_WIDTH];
    private List<Vector2i> floorTiles = new List<Vector2i>();
    private int[,] automataMap = new int[ApplicationConstants.DUNGEON_HEIGHT,ApplicationConstants.DUNGEON_WIDTH];
    private int[,] previousAutomataMap = new int[ApplicationConstants.DUNGEON_HEIGHT, ApplicationConstants.DUNGEON_WIDTH];

    private List<List<Vector2i>> rooms = new List<List<Vector2i>>();
    private List<Vector2i> roomCenters = new List<Vector2i>();

    private Dictionary<Vector2i, List<string>> tileContents = new Dictionary<Vector2i, List<string>>();

    private void Start()
    {
        //     init();
    }

    public Vector2i getSpawnableTile()
    {
        return roomCenters[0];
    }

    // Use this for initialization
    public void init(int seed = 420) {
        Random.InitState(seed);
        getAutomataMatrix();
        floodFillMap();
        fillTinyRooms();
        getRoomCenters();
        connectRooms();
        map = new int[ApplicationConstants.DUNGEON_HEIGHT, ApplicationConstants.DUNGEON_WIDTH];
        for (int y = 0; y < automataMap.GetLength(0); y++)
            for (int x = 0; x < automataMap.GetLength(1); x++)
                map[y, x] = automataMap[y, x];
        createCostGrid();
        sodRandomRooms();
    }
    
    public void createCostGrid()
    {
        costGrid = new int[ApplicationConstants.DUNGEON_WIDTH, ApplicationConstants.DUNGEON_HEIGHT];
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                switch(map[i,j])
                {
                    // a wall
                    case 0:
                        costGrid[i, j] = 10000;
                        break;
                    case 1:
                        costGrid[i, j] = 0;
                        break;
                }
            }
        }
    }

    public void sodRandomRooms()
    {
        foreach(var room in rooms)
        {
            if(Random.Range(0.0f, 1.0f) <= 0.2f)
            {
                foreach(var tile in room)
                {
                    if (map[tile.x, tile.y] == 1)
                    {
                        map[tile.x, tile.y] = 3;
                    }
                    else if (map[tile.x, tile.y] == 2)
                    {
                        map[tile.x, tile.y] = 4;
                    }
                }
            }
        }

    }

    public static GameObject tileFromId(int id)
    {
        int num = 1;
        switch(id)
        {
            // rock walls
            case 0:
                num = Random.Range(1, 4);
                return GameObject.Instantiate(Resources.Load("Environment/Rock"+num)) as GameObject;
            // regular rock tile
            case 1:
                num = Random.Range(1, 7);
                return GameObject.Instantiate(Resources.Load("Environment/Floor"+num)) as GameObject;
            // grass
            case 3:
                num = Random.Range(1, 5);
                return GameObject.Instantiate(Resources.Load("Environment/Grass" + num)) as GameObject;
            // tree
            case 4:
                num = Random.Range(1, 4);
                return GameObject.Instantiate(Resources.Load("Environment/Tree" + num)) as GameObject;
            case 2:
                break;
        }
        return null;
    }

    private void fillTinyRooms(int minSize = 10)
    {
        foreach (List<Vector2i> room in rooms.ToArray())
        {
            if (room.Count <= minSize)
            {
                foreach(Vector2i tile in room)
                {
                    automataMap[(int)tile.y, (int)tile.x] = 0;
                }
                rooms.Remove(room);
            }
        }
    }

    // get the center of each room
    // o o o o[x]o o o
    // o{x}x x x x o o
    // o x o x x o o o
    // o x x x[x]x{x}o
    // o o o o o o o o 
    // get the highest and lowest horizontal tile of a room (indicated with {x})
    // and the highest and lowest vertical tile of a room (indicated with [x])
    // then take their average and you have the center of a room!
    private void getRoomCenters()
    {
        foreach (List<Vector2i> room in rooms.ToArray())
        {
            int smallestX = (int)room[0].x,
                largestX = (int)room[0].x,
                smallestY = (int)room[0].y,
                largestY = (int)room[0].y;

            foreach (Vector2i tile in room)
            {
                if (tile.x < smallestX)
                    smallestX = (int)tile.x;
                else if (tile.x > largestX)
                    largestX = (int)tile.x;
                if (tile.y < smallestY)
                    smallestY = (int)tile.y;
                else if (tile.y > largestY)
                    largestY = (int)tile.y;
            }

            int centerX = (smallestX + largestX) / 2,
                centerY = (smallestY + largestY) / 2;
            roomCenters.Add(new Vector2i(centerX, centerY));
        }
    }

    // finds the closest room and returns its index
    private int findClosestRoom(int room, List<int> ignoreRooms)
    {
        Vector2i sourceRoomCenter = roomCenters[room];
        float smallestDistance = float.MaxValue;
        int index = 0;

        for(int i = 0; i < roomCenters.Count; i++)
        {
            if (i == room || ignoreRooms.Contains(i))
                continue;
            Vector2i curRoomCenter = roomCenters[i];
            float curDist = Vector2i.Distance(sourceRoomCenter, curRoomCenter);
            if (curDist < smallestDistance)
            {
                smallestDistance = curDist;
                index = i;
            }
        }

        return index;
    }

    private int findClosestRoom(int room)
    {
        return findClosestRoom(room, new List<int>());
    }

    private void connectRooms()
    {
        // we have rooms A, B, C, D
        // we need them all to connect
        // for each room except the last in the map
        //      connect to the closest room that it is not connected to

        // all of the room connections
        Dictionary<int, List<int>> roomConnections = new Dictionary<int, List<int>>();
        List<int> pastRoomConnections = new List<int>();

        // initialize the list
        for(int i = 0; i < rooms.Count; i++)
        {
            roomConnections.Add(i, new List<int>());
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            //List<Vector2i> room = rooms[i];
            //bool roomFound = false;

            // find the closest room that isn't already connected
            int targetRoom = findClosestRoom(i, pastRoomConnections);
            pastRoomConnections.Add(i);
            pastRoomConnections.Add(targetRoom);
            //            int targetRoom = findClosestRoom(i, roomConnections[i]);
            // add to both rooms the fact that they're now connected
            //            roomConnections[i].Add(targetRoom);
            //            roomConnections[targetRoom].Add(i);

            // select a random tile to act as the door
            // this should be
            Vector2i startTile = roomCenters[i];
            Vector2i endTile = roomCenters[targetRoom];
            carveCorridor(startTile, endTile);
        }
        /*
        for(int i = 0; i < roomConnections.Count; i++)
        {
            string rms = "";
            foreach (int rm in roomConnections[i])
                rms += rm + ", ";
            Debug.Log("room #" + i + " is connected with room(s) " + rms);
        }
        */
    }

    // use 'dumb' pathfinding to connect rooms
    private void carveCorridor(Vector2i start, Vector2i target)
    {
        float currentX = start.x, currentY = start.y;
        while(currentX != target.x)
        {
            if (currentX < target.x)
            {
                currentX++;
            }
            else
            {
                currentX--;
            }
            automataMap[(int)currentY, (int)currentX] = 1;
        }
        while(currentY != target.y)
        {
            if(currentY < target.y)
            {
                currentY++;
            }
            else
            {
                currentY--;
            }
            automataMap[(int)currentY, (int)currentX] = 1;
        }
    }

    // recursive function for finding size of a room.
    // flood fill until you hit a wall
    private int[,] checkedCells;
    private void floodFillMap()
    {
        checkedCells = new int[ApplicationConstants.DUNGEON_HEIGHT, ApplicationConstants.DUNGEON_WIDTH];
        foreach(Vector2i floor in floorTiles)
        {
            if(checkedCells[(int)floor.y, (int)floor.x] == 0)
            {
                List<Vector2i> room = new List<Vector2i>();
                floodFill((int)floor.x, (int)floor.y, room);
                rooms.Add(room);
            }
        }
    }
    private int floodFill(int x, int y, List<Vector2i> room)
    {
//        Debug.Log(x + ", " + y);
        // if the value is a wall, return
        if(automataMap[y, x] == 0 || checkedCells[y,x] == 1)
        {
            checkedCells[y, x] = 1;
            return 0;
        }
        int size = 0;
        checkedCells[y, x] = 1;
        room.Add(new Vector2i(x, y));
        if(x > 0)
            size += floodFill(x - 1, y, room);
        if(x < ApplicationConstants.DUNGEON_WIDTH-1)
            size += floodFill(x + 1, y, room);
        if(y > 0)
            size += floodFill(x, y - 1, room);
        if(y < ApplicationConstants.DUNGEON_HEIGHT-1)
            size += floodFill(x, y + 1, room);
        return size + 1;
    }


    // cellular automata creation

    private int[,] getAutomataMatrix()
    {
        // initialize our matrix 
        for(int y = 0; y < ApplicationConstants.DUNGEON_HEIGHT; y++)
        {
            for(int x = 0; x < ApplicationConstants.DUNGEON_WIDTH; x++)
            {
                if (Random.Range(0.0f, 1.0f) < automata_InitalAlivePercent)
                {
                    int state = Random.Range(0, 2);

                    previousAutomataMap[y, x] = state;
                    automataMap[y, x] = state;
                }
            }
        }
        //printMatrix(previousAutomataMap);
        //printMatrix(automataMap);
        for(int g = 0; g < automata_Generations; g++)
        {
            generation();
        }
        return automataMap;
    }

    private int neighbors(int x, int y)
    {
        int x1 = (x > 0) ? x - 1 : x;
        int x2 = (x < ApplicationConstants.DUNGEON_WIDTH - 1) ? x + 1 : x;
        int y1 = (y > 0) ? y - 1 : y;
        int y2 = (y < ApplicationConstants.DUNGEON_HEIGHT - 1) ? y + 1 : y;
        int count = 0;
        for (int j = y1; j <= y2; j++)
        {
            for (int i = x1; i <= x2; i++)
            {
                if (previousAutomataMap[j, i] == 1)
                {
                    count += (previousAutomataMap[j, i]);
                }
            }
        }
        return count;
    }

    private void generation()
    {
        for (int y = 0; y < ApplicationConstants.DUNGEON_HEIGHT; y++)
        {
            for (int x = 0; x < ApplicationConstants.DUNGEON_WIDTH; x++)
            {
                int nb = neighbors(x, y);
                
                // c c c c c c c c c
                // c c c 1 1 1 c c c
                // c c c 1 x 1 c c c
                // c c c 1 1 1 c c c
                // c c c c c c c c c
                // moore neighborhood of a cell with all neighbor values = 1
                
                if(previousAutomataMap[y,x] == 1)
                {
                    if (nb < automata_DeathLimit)
                        automataMap[y, x] = 0;
                    else
                        automataMap[y, x] = 1;
                }
                else
                {
                    if (nb > automata_BirthLimit)
                        automataMap[y, x] = 1;
                    else
                        automataMap[y, x] = 0;
                }
                if(automataMap[y, x] == 1)
                {
                    floorTiles.Add(new Vector2i(x, y));
                }
            }
        }
        previousAutomataMap = automataMap;
    }


    public static void printMatrix(int[,] matrix)
    {
        for(int i = 0; i < matrix.GetLength(0); i++)
        {
            string logLine = "";
            for(int j = 0; j < matrix.GetLength(1); j++)
            {
                logLine += matrix[i, j] + " ";
            }
            Debug.Log(logLine);
        }
    }
    
}
