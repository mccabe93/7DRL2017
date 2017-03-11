using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utility;

public class Pathfinder {

    public static int[,] environmentMap;
    public int[,] map;

    private List<Vector2i> visited = new List<Vector2i>();
    public List<Vector2i> path = new List<Vector2i>();

    int destinationX, destinationY;
	// Use this for initialization
	public Pathfinder (int destinationX, int destinationY) {
        this.destinationX = destinationX;
        this.destinationY = destinationY;
//        Debug.Log("MATRIX WE GOT IN PATHFINDER");
//        DungeonGenerator.printMatrix(environmentMap);
//        Debug.Log("END MATRIX WE GOT IN PATHFINDER");
        map = new int[environmentMap.GetLength(0),environmentMap.GetLength(1)];
        floodFillCostMap();
//        Debug.Log("MATRIX WE FILLED IN PATHFINDER");
//        DungeonGenerator.printMatrix(map);
//        Debug.Log("END MATRIX WE FILLED IN PATHFINDER");
    }

    private void floodFillCostMap()
    {
        for(int x = 0; x < environmentMap.GetLength(0); x++)
        {
            for(int y = 0; y < environmentMap.GetLength(1); y++)
            {
                // the cost of the destination + its distance from the destination
                map[x, y] = environmentMap[x, y] + Mathf.Abs(x - destinationX) + Mathf.Abs(y - destinationY);
            }
        }
    }

    public void findNewPath(int x, int y, bool abs = false)
    {
        visited = new List<Vector2i>();
        path = new List<Vector2i>();
        findPath(x, y, abs);
    }

    public void findPath(int x, int y, bool abs = false)
    {
        // if we're within 1 tile of the destination we're done.
        if (!abs && Mathf.Abs(x - destinationX) <= 1 && Mathf.Abs(y - destinationY) <= 1)
        {
            return;
        }
        // if absolute position is needed, we only stop when we're at the destination tile (may result in impossible path)
        else
        {
            if (x == destinationX && y == destinationY)
                return;
        }
        Vector2i leastCostVec = new Vector2i(-1, -1);
        int leastCost = int.MaxValue;
        for (int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                Vector2i tmp = new Vector2i(x + i, y + j);
                // if it's the same as the parent, out of bounds, or we have already visited the node then skip this motha
                if ((i == 0 && j == 0) ||
                     x - i < 0 || x + i >= ApplicationConstants.DUNGEON_WIDTH || y - j < 0 || y + j >= ApplicationConstants.DUNGEON_HEIGHT ||
                     visited.Contains(tmp) || (Mathf.Abs(i) + Mathf.Abs(j) >= 2))
                {
                    continue;
                }
                if(map[x + i, y + j] < leastCost)
                {
                    leastCost = map[x + i, y + j];
                    leastCostVec = tmp;
                }
                visited.Add(tmp);
            }
        }
        path.Add(leastCostVec);
        findPath(leastCostVec.x, leastCostVec.y, abs);
    }

    public void printPath()
    {
        foreach(Vector2i node in path)
        {
            Debug.Log(node.x + ", " + node.y);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
