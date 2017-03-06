using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar_Pathfind : MonoBehaviour
{
    public GameObject target;
    public bool pathFound = false;
    Path discoveredPath = new Path();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        #region Testing Area Only
        //UNIT TESTING AREA
        Vector2 start = new Vector2(0, 0);
        Vector2 end = new Vector2(2, 4);
        Path thePath = getPath(start, end);
        //END UNIT TESTING
        #endregion

        //Get the shortest path
        Vector2 thisPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);    //position of this object
        Vector2 targetPos = new Vector2(target.gameObject.transform.position.x, target.gameObject.transform.position.y); //position of target object
        Path shortestPath = getPath(gameObject.transform.position, targetPos);  //Get the shortest path from position of this object, to target object

    }


    /*
   * @param Vector2 startPosition, Vector2 targetPosition
   * @returns Path discoveredPath
   * @desc Finds best path from one Vector2 to another
   * @status Tested
   */
    public Path getPath(Vector2 startPos, Vector2 targetPos)
    {
        Path startingPath = new Path(true);
        startingPath.nodes.Add(startPos);
        List<Path> paths = getNodeMoves(startingPath, startPos, targetPos);


        while (!pathFound)
        {
            List<Path> updatedPaths = new List<Path>();

            //get all paths
            for (int i = 0; i < paths.Count; i++)
            {
                if (pathFound) break;
                //get node moves available for each path
                List<Path> curPaths = getNodeMoves(paths[i], paths[i].nodes[paths[i].nodes.Count - 1], targetPos);

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
   * @status Tested
   */
    public List<Path> getNodeMoves(Path currentPath, Vector2 startPos, Vector2 targetPos)
    {
        List<Path> paths = new List<Path>();

        //see if position one position to the right is valid
        if (isValidPosition(new Vector2(startPos.x + 1, startPos.y)))
        {
            Path tempPath = new Path(true);
            for (int p = 0; p < currentPath.nodes.Count; p++)
            {
                tempPath.nodes.Add(currentPath.nodes[p]);
            }
            tempPath.nodes.Add(new Vector2(startPos.x + 1, startPos.y));
            if (targetPos == new Vector2(startPos.x + 1, startPos.y))
            {
                tempPath.pathFound = true;    //if this is the target
                discoveredPath = tempPath;
                pathFound = true;
            }

            paths.Add(tempPath);
        }

        //check up
        if (isValidPosition(new Vector2(startPos.x, startPos.y + 1)))
        {
            Path tempPath2 = new Path(true);
            for (int p = 0; p < currentPath.nodes.Count; p++)
            {
                tempPath2.nodes.Add(currentPath.nodes[p]);
            }
            tempPath2.nodes.Add(new Vector2(startPos.x, startPos.y + 1));
            if (targetPos == new Vector2(startPos.x, startPos.y + 1))
            {
                tempPath2.pathFound = true;    //if this is the target
                discoveredPath = tempPath2;
                pathFound = true;
            }

            paths.Add(tempPath2);
        }

        //check down
        if (isValidPosition(new Vector2(startPos.x, startPos.y - 1)))
        {
            Path tempPath3 = new Path(true);
            for (int p = 0; p < currentPath.nodes.Count; p++)
            {
                tempPath3.nodes.Add(currentPath.nodes[p]);
            }
            tempPath3.nodes.Add(new Vector2(startPos.x, startPos.y - 1));
            if (targetPos == new Vector2(startPos.x, startPos.y - 1))
            {
                tempPath3.pathFound = true;    //if this is the target
                discoveredPath = tempPath3;
                pathFound = true;
            }

            paths.Add(tempPath3);
        }

        //check left
        if (isValidPosition(new Vector2(startPos.x - 1, startPos.y)))
        {
            Path tempPath4 = new Path(true);
            for (int p = 0; p < currentPath.nodes.Count; p++)
            {
                tempPath4.nodes.Add(currentPath.nodes[p]);
            }
            tempPath4.nodes.Add(new Vector2(startPos.x - 1, startPos.y));
            if (targetPos == new Vector2(startPos.x - 1, startPos.y))
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
   * @param Vector2 Position to check
   * @returns bool Is it a valid position?
   * @desc Checks to see if a position is valid by checking for a collider at the given spot (walls, objects, enemies, etc)
   * @status Haven't tested with colliders
   */
    public bool isValidPosition(Vector2 pos)
    {
        Vector2 spawnPos = pos; //(whatever position you want to check)
        float radius = 1.0f; // (whatever radius you want to check)

        if (Physics.CheckSphere(spawnPos, radius))
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
