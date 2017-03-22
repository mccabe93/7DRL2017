using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar_Pathfinder : MonoBehaviour {
    
    public int[,] dungeon_CostGrid;
    DungeonGenerator dg;
    public DungeonManager.DungeonTile[,] worldGrid;
    public Node[,] nodeGrid = new Node[ApplicationConstants.DUNGEON_WIDTH, ApplicationConstants.DUNGEON_HEIGHT];
    public List<Node> bestPath = new List<Node>();  //This is our grand path per say
    public List<Node> inProgressPath = new List<Node>();
    public Node startNode;
    public Node destinationNode;
    public Node previousNode;
    public bool pathFound = false;
    public bool nodesSet = false;

    /*
    *   The following is a snippet for how to use this class...
            Component pathScript = gameObject.AddComponent<AStar_Pathfinder>();
            AStar_Pathfinder pathFinder = GetComponent<AStar_Pathfinder>();
            pathFinder.init();
            pathFinder.setNodes(new Vector2(x, y), new Vector2(0, 6));
            List<AStar_Pathfinder.Node> path = pathFinder.buildPath(pathFinder.startNode, pathFinder.destinationNode);
    */


    /*
    *@desc Initialization of some things. Should be called before anything further
    *@param null
    *@return void
    *@status Working
    */
    public void init()
    {
        //Get world grid and populate our node grid
        dg = new DungeonGenerator();
        dg.init();
        dungeon_CostGrid = dg.costGrid;
        worldGrid = DungeonManager.WorldGrid;
        populateNodeGrid();
    }

    /*
    *@desc Sets the startNode and destinationNode as well as sets "nodesSet" to true. Nothing will run until these are set. 
    *@param Vector2 the coordinates in the world grid of the starting position, Vector2 the coordinates in the world grid of the ending position
    *@return void
    *@status Working
    */
    public void setNodes(Vector2 positionInWorldGrid_Start, Vector2 positionInWorldGrid_End)
    {
        this.startNode = nodeGrid[(int)positionInWorldGrid_Start.x, (int)positionInWorldGrid_Start.y];
        this.destinationNode = nodeGrid[(int)positionInWorldGrid_End.x, (int)positionInWorldGrid_End.y];
       // nodesSet = true;
    }

    /*
    *@desc Checks to see if a better path is available to this node
    *@param List<Node> path leading to the node we're checking, Node node we're checking for a better path to
    *@return Node neighboring Node with the shortest total cost
    *@status Working
    */
    public List<Node> buildPath(Node start, Node end)
    {
        if(start.Position == end.Position)
        {
            inProgressPath = new List<Node>();
            return inProgressPath;
        }
            

        Node cur_Node = start;
        
        while (!pathFound)
        {
            Node bestNeigh = getBestNeighbor(inProgressPath, cur_Node, start.Position, end.Position);
            
            if(inProgressPath.Count > 0 && isInPathAlready(inProgressPath, bestNeigh) && !pathFound)
            {
                inProgressPath = reducePathToPreviousPoint(inProgressPath, bestNeigh);
                cur_Node.IsAvailable = false;
            }
            else if (!pathFound)
            {
                inProgressPath.Add(bestNeigh);
            }
            
            previousNode = cur_Node;
            cur_Node = bestNeigh;
         
        }

        if(inProgressPath.Count == 1 && inProgressPath[0] == start)
            inProgressPath = new List<Node>();

        return inProgressPath;
    }

    /*
    *@desc Chops a list based on when it reached a point previously
    *@param List<Node> path so far, Node node we want to "revert" to
    *@return List<Node> path
    *@status Working
    */
    public List<Node> reducePathToPreviousPoint(List<Node> pathRightNow, Node nodeToFindAndReduceTo)
    {
        List<Node> tempList = new List<Node>();
        for (int i = 0; i < pathRightNow.Count; i++)
        {
            if(pathRightNow[i].Position == nodeToFindAndReduceTo.Position)
            {
                tempList = pathRightNow.GetRange(0, i+1);
                return tempList;
            }
        }

        return tempList;
    }

    /*
    *@desc Checks to see if a better path is available to this node
    *@param List<Node> path leading to the node we're checking, Node node we're checking for a better path to
    *@return Node neighboring Node with the shortest total cost
    *@status Working
    */
    public List<Node> hasShorterPathCheck(List<Node> pathThusFar, Node nodeToCheck)
    {
        Node startNode = pathThusFar[0];

        return pathThusFar;
    }

    /*
    *@desc Checks to see a node already exists in a path so we don't go backwards
    *@param List<Node> path leading to the node we're checking, Node node we're checking for
    *@return Boolean isInPathAlready
    *@status Working
    */
    public bool isInPathAlready(List<Node> pathThusFar, Node nodeToCheck)
    {
        for (int i = 0; i < pathThusFar.Count; i++)
        {
            if (nodeToCheck.Position == pathThusFar[i].Position)
                return true;
        }

        return false;
    }


    /*
    *@desc Gets the best neighbor from the current node, based on heuristic evaluation from start point and end point
    *@param Node current node on, Vector2 original starting position, Vector2 destination position
    *@return Node neighboring Node with the shortest total cost
    *@status Working
    */
    public Node getBestNeighbor(List<Node> currentPath, Node curNode, Vector2 startPos, Vector2 endPos)
    {
        Vector2 curPos = curNode.Position;
        float leastCost = 1000f;
        Node bestNeighbor = new Node();
        
        //top - If top is in bounds and available
        if ((curPos.y > 0) && nodeGrid[(int)curPos.x, (int)curPos.y - 1].IsAvailable || nodeGrid[(int)curPos.x, (int)curPos.y - 1].Position == endPos)
        {
            Vector2 tempTop = new Vector2((int)curPos.x, (int)curPos.y - 1);
            nodeGrid[(int)tempTop.x, (int)tempTop.y].Cost_ToStart = getDistance(startPos, tempTop);
            nodeGrid[(int)tempTop.x, (int)tempTop.y].Cost_ToDestination = getDistance(endPos, tempTop);
            nodeGrid[(int)tempTop.x, (int)tempTop.y].Total_CostEstimate = nodeGrid[(int)tempTop.x, (int)tempTop.y].getTotalCost();
            if (nodeGrid[(int)tempTop.x, (int)tempTop.y].Cost_ToDestination < leastCost)
            {
                leastCost = nodeGrid[(int)tempTop.x, (int)tempTop.y].Cost_ToDestination;
                bestNeighbor = nodeGrid[(int)tempTop.x, (int)tempTop.y];
            }
        }

        //right - If right is in bounds and available
        if ((curPos.x < ApplicationConstants.DUNGEON_WIDTH - 1) && nodeGrid[(int)curPos.x + 1, (int)curPos.y].IsAvailable || nodeGrid[(int)curPos.x + 1, (int)curPos.y].Position == endPos)
        {
            Vector2 tempRight = new Vector2((int)curPos.x + 1, (int)curPos.y);
            nodeGrid[(int)tempRight.x, (int)tempRight.y].Cost_ToStart = getDistance(startPos, tempRight);
            nodeGrid[(int)tempRight.x, (int)tempRight.y].Cost_ToDestination = getDistance(endPos, tempRight);
            nodeGrid[(int)tempRight.x, (int)tempRight.y].Total_CostEstimate = nodeGrid[(int)tempRight.x, (int)tempRight.y].getTotalCost();
            if (nodeGrid[(int)tempRight.x, (int)tempRight.y].Cost_ToDestination < leastCost)
            {
                leastCost = nodeGrid[(int)tempRight.x, (int)tempRight.y].Cost_ToDestination;
                bestNeighbor = nodeGrid[(int)tempRight.x, (int)tempRight.y];
            }
        }

        //bottom- If bottom is in bounds and available
        if ((curPos.y < ApplicationConstants.DUNGEON_HEIGHT - 1) && nodeGrid[(int)curPos.x, (int)curPos.y + 1].IsAvailable || nodeGrid[(int)curPos.x, (int)curPos.y + 1].Position == endPos)
        {
            Vector2 tempBottom = new Vector2((int)curPos.x, (int)curPos.y+1);
            nodeGrid[(int)tempBottom.x, (int)tempBottom.y].Cost_ToStart = getDistance(startPos, tempBottom);
            nodeGrid[(int)tempBottom.x, (int)tempBottom.y].Cost_ToDestination = getDistance(endPos, tempBottom);
            nodeGrid[(int)tempBottom.x, (int)tempBottom.y].Total_CostEstimate = nodeGrid[(int)tempBottom.x, (int)tempBottom.y].getTotalCost();
            if (nodeGrid[(int)tempBottom.x, (int)tempBottom.y].Cost_ToDestination < leastCost)
            {
                leastCost = nodeGrid[(int)tempBottom.x, (int)tempBottom.y].Cost_ToDestination;
                bestNeighbor = nodeGrid[(int)tempBottom.x, (int)tempBottom.y];
            }
        }

        //left- If left is in bounds and available
        if ((curPos.x > 0) && nodeGrid[(int)curPos.x - 1, (int)curPos.y].IsAvailable || nodeGrid[(int)curPos.x - 1, (int)curPos.y].Position == endPos)
        {
            Vector2 tempLeft = new Vector2((int)curPos.x - 1, (int)curPos.y);
            nodeGrid[(int)tempLeft.x, (int)tempLeft.y].Cost_ToStart = getDistance(startPos, tempLeft);
            nodeGrid[(int)tempLeft.x, (int)tempLeft.y].Cost_ToDestination = getDistance(endPos, tempLeft);
            nodeGrid[(int)tempLeft.x, (int)tempLeft.y].Total_CostEstimate = nodeGrid[(int)tempLeft.x, (int)tempLeft.y].getTotalCost();
            if (nodeGrid[(int)tempLeft.x, (int)tempLeft.y].Cost_ToDestination < leastCost)
            {
                leastCost = nodeGrid[(int)tempLeft.x, (int)tempLeft.y].Cost_ToDestination;
                bestNeighbor = nodeGrid[(int)tempLeft.x, (int)tempLeft.y];
            }
        }

        if(bestNeighbor == previousNode)
        {
            curNode.IsAvailable = false;
            return previousNode;
        }

        if (bestNeighbor.Position == endPos)
            pathFound = true;

        return bestNeighbor;
    }

    /*
    *@desc Populates grid of nodes, based on world grid
    *@param null
    *@return null
    *@status Working
    */
    private void populateNodeGrid()
    {
        DungeonManager.DungeonTile[,] temp = worldGrid;
        for (int i = 0; i < ApplicationConstants.DUNGEON_WIDTH; i++)
        {
            for (int j = 0; j < ApplicationConstants.DUNGEON_HEIGHT; j++)
            {
                Node cur_Node = new Node();
                cur_Node.DungeonTile = worldGrid[i, j];
                int cur_Cost = dungeon_CostGrid[i, j];
                if (cur_Cost >= 10000 || cur_Node.DungeonTile.Actor != null)
                {
                    cur_Node.IsAvailable = false;
                }
                else
                    cur_Node.IsAvailable = true;
                cur_Node.Position = new Vector2(i, j);                
                nodeGrid[i, j] = cur_Node;
            }
        }
    }

    /*
    *@desc Gets the distance between two points, not taking into account blocked routes (estimate)
    *@param Vector starting position, Vector2 ending position
    *@return float Distance
    *@status Working
    */
    public float getDistance(Vector2 startPos, Vector2 endPos)
    {
        return Vector2.Distance(startPos, endPos);
    }

    public class Node
    {
        public DungeonManager.DungeonTile DungeonTile { get; set; }
        public Vector2 Position { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsOpen { get; set; }
        public float Cost_ToStart { get; set; }
        public float Cost_ToDestination { get; set; }
        public float Total_CostEstimate { get; set; }

        public Node() { }

        public float getTotalCost()
        {
            return Cost_ToStart + Cost_ToDestination;
        }
    }
}
