/*
*TODO
*-Need to set x_LocationInDungeonGrid, y_LocationInDungeonGrid, x_TargetLocationInGrid, y_TargetLocationInGrid relative to this gameobject's position
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unittest : MonoBehaviour
{
    public GameObject target;
    public bool pathFound = false;
    Path discoveredPath = new Path();
    public DungeonManager dungeonManager;
    public DungeonManager.DungeonTile[,] dungeonGrid;
    public int x_LocationInDungeonGrid;
    public int y_LocationInDungeonGrid;
    public int x_TargetLocationInGrid;
    public int y_TargetLocationInGrid;

    // Use this for initialization
    void Start()
    {
        dungeonGrid = GameObject.FindGameObjectWithTag("DungeonManager").GetComponent<DungeonManager>().WorldGrid;
    }

    // Update is called once per frame
    void Update()
    {
        #region Testing Area Only
        //UNIT TESTING AREA
        //Vector2 start = new Vector2(0, 0);
        //Vector2 end = new Vector2(2, 4);
        //Path thePath = getPath(start, end);
        //END UNIT TESTING
        #endregion

        //Get the shortest path
        Path shortestPath = getPath(x_LocationInDungeonGrid, y_LocationInDungeonGrid, x_TargetLocationInGrid, y_TargetLocationInGrid);  //Get the shortest path from position of this object, to target object
        
    }


    /*
   * @param Vector2 startPosition, Vector2 targetPosition
   * @returns Path discoveredPath
   * @desc Finds best path from one Vector2 to another
   * @status Completed with Grid implementation. Untested
   */
    public Path getPath(int x_Start, int y_Start, int x_Target, int y_Target)
    {
        Path startingPath = new Path(true);
        startingPath.nodes.Add(new Vector2(x_Start, y_Start));
        List<Path> paths = getNodeMoves(startingPath, x_Start, y_Start, x_Target, y_Target);


        while (!pathFound)
        {
            List<Path> updatedPaths = new List<Path>();

            //get all paths
            for (int i = 0; i < paths.Count; i++)
            {
                if (pathFound) break;
                //get node moves available for each path
                List<Path> curPaths = getNodeMoves(paths[i], (int)paths[i].nodes[paths[i].nodes.Count - 1].x, (int)paths[i].nodes[paths[i].nodes.Count - 1].y, x_Target, y_Target);

                //If we have some paths, add them to updated paths
                if (curPaths.Count != 0)
                {
                    for (int j = 0; j < curPaths.Count; j++)
                    {
                        if (pathFound) break;
                        updatedPaths.Add(curPaths[j]);
                    }
                }
            }

            //set paths to updatedPaths and go again
            paths = updatedPaths;
        }
        
        return discoveredPath;
    }


    /*
   * @param Path currentPath, Vector2 startPos, Vector2 targetPos
   * @returns List<Path> List of still potential paths
   * @desc Gets all available node moves for a given node (up, right, down, left)
   * @status Completed with Grid implementation. Untested
   */
    public List<Path> getNodeMoves(Path currentPath, int x_CurrentDungeonSpot, int y_CurrentDungeonSpot, int x_TargetDungeonSpot, int y_TargetDungeonSpot)
    {
        List<Path> paths = new List<Path>();

        //see if position one position to the right is valid
        if (isValidPosition(x_CurrentDungeonSpot, y_CurrentDungeonSpot))
        {
            Path tempPath = new Path(true);
            for (int p = 0; p < currentPath.nodes.Count; p++)
            {
                tempPath.nodes.Add(currentPath.nodes[p]);
            }
            tempPath.nodes.Add(new Vector2(x_CurrentDungeonSpot + 1, y_CurrentDungeonSpot));
            if (x_TargetDungeonSpot == (x_CurrentDungeonSpot + 1) && y_TargetDungeonSpot == y_CurrentDungeonSpot)
            {
                tempPath.pathFound = true;    //if this is the target
                discoveredPath = tempPath;
                pathFound = true;
            }

            paths.Add(tempPath);
        }

        //check up
        if (isValidPosition(x_CurrentDungeonSpot, y_CurrentDungeonSpot + 1))
        {
            Path tempPath2 = new Path(true);
            for (int p = 0; p < currentPath.nodes.Count; p++)
            {
                tempPath2.nodes.Add(currentPath.nodes[p]);
            }
            tempPath2.nodes.Add(new Vector2(x_CurrentDungeonSpot, y_CurrentDungeonSpot + 1));
            if (x_TargetDungeonSpot == x_CurrentDungeonSpot && y_TargetDungeonSpot == (y_CurrentDungeonSpot +1))
            {
                tempPath2.pathFound = true;    //if this is the target
                discoveredPath = tempPath2;
                pathFound = true;
            }

            paths.Add(tempPath2);
        }

        //check down
        if (isValidPosition(x_CurrentDungeonSpot, y_CurrentDungeonSpot -1))
        {
            Path tempPath3 = new Path(true);
            for (int p = 0; p < currentPath.nodes.Count; p++)
            {
                tempPath3.nodes.Add(currentPath.nodes[p]);
            }
            tempPath3.nodes.Add(new Vector2(x_CurrentDungeonSpot, y_CurrentDungeonSpot - 1));
            if (x_TargetDungeonSpot == x_CurrentDungeonSpot && y_TargetDungeonSpot == (y_CurrentDungeonSpot - 1))
            {
                tempPath3.pathFound = true;    //if this is the target
                discoveredPath = tempPath3;
                pathFound = true;
            }

            paths.Add(tempPath3);
        }

        //check left
        if (isValidPosition(x_CurrentDungeonSpot - 1, y_CurrentDungeonSpot))
        {
            Path tempPath4 = new Path(true);
            for (int p = 0; p < currentPath.nodes.Count; p++)
            {
                tempPath4.nodes.Add(currentPath.nodes[p]);
            }
            tempPath4.nodes.Add(new Vector2(x_CurrentDungeonSpot - 1, y_CurrentDungeonSpot));
            if (x_TargetDungeonSpot == (x_CurrentDungeonSpot-1) && y_TargetDungeonSpot == y_CurrentDungeonSpot)
            {
                tempPath4.pathFound = true;    //if this is the target
                discoveredPath = tempPath4;
                pathFound = true;
            }

            paths.Add(tempPath4);
        }

        return paths;
    }


    /*
   * @param int x, int y
   * @returns bool Is it a valid position?
   * @desc Checks to see if a position in dungeon grid has an actor
   * @status Completed with Grid implementation. Untested
   */
    public bool isValidPosition(int x_PositionInGrid, int y_PositionInGrid)
    {

        if (dungeonGrid[x_LocationInDungeonGrid, y_LocationInDungeonGrid].Actor != null)
        {
            //found something
            return false;
        }
        else {
            // spot is empty, we can spawn
            return true;
        }
    }


    /*
   * @desc Path class contains data regarding a path (duh) 
   * @status Tested
   */
    public class Path
    {
        //class properties
        public bool isOpen { get; set; }
        public List<Vector2> nodes { get; set; }
        public int cost { get; set; }   //this will more or less be equal to the length of the path list
        public bool pathFound { get; set; }

        //default constructor
        public Path()
        {
            this.nodes = new List<Vector2>();
            this.pathFound = false;
        }

        //overload constructor
        public Path(bool isOpen)
        {
            this.nodes = new List<Vector2>();
            this.pathFound = false;
            this.isOpen = isOpen;
        }
    }
}
