using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {

    // constants for generating our automata
    public int automata_BirthLimit = 4, automata_DeathLimit = 3, automata_Generations = 2;
    public float automata_InitalAlivePercent = 0.4f;

    public int maxRoomConnections = 3;

    public Sprite wall;
    public Sprite ground;

    // to not devour enormouse amounts of memory, just build the cost grid after completion?
    private int[,] costGrid;// = new int[ApplicationConstants.DUNGEON_HEIGHT][ApplicationConstants.DUNGEON_WIDTH];
    private List<Vector2> floorTiles = new List<Vector2>();
    private int[,] automataMap = new int[ApplicationConstants.DUNGEON_HEIGHT,ApplicationConstants.DUNGEON_WIDTH];
    private int[,] previousAutomataMap = new int[ApplicationConstants.DUNGEON_HEIGHT, ApplicationConstants.DUNGEON_WIDTH];

    private List<List<Vector2>> rooms = new List<List<Vector2>>();

    private Dictionary<Vector2, List<string>> tileContents = new Dictionary<Vector2, List<string>>();

    // Use this for initialization
    void Start() {
        getAutomataMatrix();
        Random.seed = 42;
        //        printMatrix(automataMap);
        floodFillMap();
        fillTinyRooms();
        connectRooms();
        for (int i = 0; i < automataMap.GetLength(0); i++)
        {
            for (int j = 0; j < automataMap.GetLength(1); j++) {
                //if (!(i == 0 && j == 7))
                {
                    GameObject tmp = null;
                    if (automataMap[i, j] == 0)
                    {
                        tmp = GameObject.Instantiate(Resources.Load("Environment/Wall")) as GameObject;
                    }
                    else
                    {
                        tmp = GameObject.Instantiate(Resources.Load("Environment/Ground")) as GameObject;
                    }
                    tmp.transform.Translate(j * 0.16f, i * 0.16f, 0);
                }
            }
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void fillTinyRooms(int minSize = 10)
    {
        foreach (List<Vector2> room in rooms.ToArray())
        {
            if (room.Count <= minSize)
            {
                foreach(Vector2 tile in room)
                {
                    automataMap[(int)tile.y, (int)tile.x] = 0;
                }
                rooms.Remove(room);
            }
        }
    }
    

    private void connectRooms()
    {
        // we have rooms A, B, C, D
        // we need them all to connect
        // as long as each room connects to at least one other room that does not connect to itself,
        // then all rooms will be connected

        // rooms that haven't connect to any room yet
        List<int> disconnectedRooms = new List<int>();
        disconnectedRooms.AddRange(Enumerable.Range(0, 10));

        // all of the room connections
        Dictionary<int, List<int>> roomConnections = new Dictionary<int, List<int>>();

        for (int i = 0; i < rooms.Count; i++)
        {
            List<Vector2> room = rooms[i];
            // connect to some rooms
            List<int> connectedRooms = new List<int>();
            // keep searching for rooms to connect
            for (int k = 0; k < Random.Range(1, maxRoomConnections); k++)
            {
                int roomToTryConnect = Random.Range(0, disconnectedRooms.Count);
                if(!roomConnections[i].Contains(i))
                {
                    
                } 
                else
                {
                    // try again
                    k--;
                }
            }
            // select a random tile to act as the door
            // this should be
            int tile = Random.Range(0, room.Count - 1);
            
        }
    }

    // use 'dumb' pathfinding to connect rooms
    private void carveCorridor(int startX, int startY, int targetX, int targetY)
    {
        int currentX = startX, currentY = startY;
        while(currentX != targetX)
        {
            if (currentX < targetX)
            {
                currentX++;
            }
            else
            {
                currentX--;
            }
            automataMap[currentY, currentX] = 1;
        }
        while(currentY != targetY)
        {
            if(currentY < targetY)
            {
                currentY++;
            }
            else
            {
                currentY--;
            }
            automataMap[currentY, currentX] = 1;
        }
    }

    // recursive function for finding size of a room.
    // flood fill until you hit a wall
    private int[,] checkedCells;
    private void floodFillMap()
    {
        checkedCells = new int[ApplicationConstants.DUNGEON_HEIGHT, ApplicationConstants.DUNGEON_WIDTH];
        foreach(Vector2 floor in floorTiles)
        {
            if(checkedCells[(int)floor.y, (int)floor.x] == 0)
            {
                List<Vector2> room = new List<Vector2>();
                floodFill((int)floor.x, (int)floor.y, room);
                rooms.Add(room);
            }
        }
    }
    private int floodFill(int x, int y, List<Vector2> room)
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
        room.Add(new Vector2(x, y));
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
        Random rng = new Random();
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
                    floorTiles.Add(new Vector2(x, y));
                }
            }
        }
        previousAutomataMap = automataMap;
    }


    private void printMatrix(int[,] matrix)
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
