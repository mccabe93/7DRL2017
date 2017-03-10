/*
*TODO
*-Need to set x_PositionInDungeonGrid and y_PositionInDungeonGrid (how to map player's world position relative to the game grid?)
*-Need to add animation changes for which direction facing / moving
*-Logic for actually conducting combat (taking hits, giving hits, manipulating hit points?)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMovement : MonoBehaviour {

    private Direction currentDir;
    private Vector2 input;
    private float walkSpeed = 3.0f;
    private float t;
    private bool isMoving;
    public bool isAllowedToMove;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool toMove;
    public DungeonManager.DungeonTile[,] dungeonMap;
    public int x_PositionInDungeonGrid; //Need to set this somehow??
    public int y_PositionInDungeonGrid; //Need to set this somehow??

    // Use this for initialization
    void Start ()
    {
        dungeonMap = GameObject.FindGameObjectWithTag("DungeonManager").GetComponent<DungeonManager>().WorldGrid;
        currentDir = Direction.North;
        toMove = true;
        isAllowedToMove = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //able to move by default unless specified otherwise
        toMove = true;

        #region Movement handler - Implemented for grid. Just need coordinates in grid to test "Move" function
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


        if (Mathf.Abs(input.x) > Mathf.Abs(input.y) && Mathf.Abs(input.x) >= 0.25)
        {
            input.y = 0;
        }
        else if (Mathf.Abs(input.y) >= 0.25)    //Only registerring movement if key is pretty hard (long) enough. So could change direction animation by tapping without actually moving
        {
            input.x = 0;
        }
        else if (input.x > 0 && Mathf.Abs(input.x) < 0.25)    //Only registerring movement if key is pretty hard (long) enough. So could change direction animation by tapping without actually moving
        {
            currentDir = Direction.East;
            toMove = false;
        }
        else if (input.x < 0 && Mathf.Abs(input.x) < 0.25)    //Only registerring movement if key is pretty hard (long) enough. So could change direction animation by tapping without actually moving
        {
            currentDir = Direction.West;
            toMove = false;
        }
        else if (input.y > 0 && Mathf.Abs(input.y) < 0.25)    //Only registerring movement if key is pretty hard (long) enough. So could change direction animation by tapping without actually moving
        {
            currentDir = Direction.North;
            toMove = false;
        }
        else if (input.y < 0 && Mathf.Abs(input.y) < 0.25)    //Only registerring movement if key is pretty hard (long) enough. So could change direction animation by tapping without actually moving
        {
            currentDir = Direction.South;
            toMove = false;
        }

        if (!isMoving && isAllowedToMove)
        {
            if (input != Vector2.zero && toMove)   //If key pressed long enough, then perform move
            {
                if (input.x < 0)
                {
                    currentDir = Direction.West;
                    //West walking animation?
                }
                if (input.x > 0)
                {
                    currentDir = Direction.East;
                    //East walking animation?
                }
                if (input.y < 0)
                {
                    currentDir = Direction.South;
                    //South walking animation?
                }
                if (input.y > 0)
                {
                    currentDir = Direction.North;
                    //North walking animation?
                }
                StartCoroutine(Move(x_PositionInDungeonGrid, y_PositionInDungeonGrid, currentDir));
            }
            else    //If key just tapped, just perform animation / change sprite
            {
                switch (currentDir)
                {
                    case Direction.North:
                        //North facing sprite
                        break;
                    case Direction.East:
                        //East facing sprite
                        break;
                    case Direction.South:
                        //South facing sprite
                        break;
                    case Direction.West:
                        //West facing sprite
                        break;
                }
            }
        }
        #endregion

        

    }

    /*
    * @param GameObject attacker who is 'attacking" (player), GameObject enemy who is "being attacked
    * @returns null
    * @desc Begins combat of two GameObjects
    * @status Not Started
    */
    public void InitiateCombat(GameObject attacker, GameObject enemy)
    {
        //Animation

        //Health loss

    }


    /*
    * @param Transform 'entity' holding the transform (probably not necessary anymore)
    * @returns GameObject 'Combatanant"
    * @desc Checks for neearby (non-diagonal, 1 pixel away) enemies and returns their GameObject
    * @status Complete with grid implementation. Untested. 
    */
    public GameObject CheckForCombat(int xPositionInDungeonMap, int yPositionInDungeonMap)
    {
        int x = xPositionInDungeonMap; 
        int y = yPositionInDungeonMap;   
        
        //check right
        if(dungeonMap[x + 1, y].Actor != null)
        {
            return dungeonMap[x + 1, y].Actor;
        }

        //check left
        if (dungeonMap[x - 1, y].Actor != null)
        {
            return dungeonMap[x - 1, y].Actor;
        }

        //check up
        if (dungeonMap[x, y + 1].Actor != null)
        {
            return dungeonMap[x, y + 1].Actor;
        }

        //check down
        if (dungeonMap[x + 1, y - 1].Actor != null)
        {
            return dungeonMap[x, y - 1].Actor;
        }

        return null;

        #region Deprecated pre-grid code (pixels)
        ////raidus to check for colliders
        //float radius = 1.0f;

        ////check right
        //Vector2 rightPos = new Vector2(entity.position.x + 1, entity.position.y);
        //Collider[] colliders_Right = Physics.OverlapSphere(rightPos, radius);
        //if (colliders_Right.Length > 0) //If we found colliders to our right, check if it's an enemy
        //{
        //    for (int i = 0; i < colliders_Right.Length; i++)
        //    {
        //        if (colliders_Right[i].gameObject.tag == "Enemy")
        //            return colliders_Right[i].gameObject as GameObject; //if we found a gameobject, return it 
        //    }
        //}

        ////check left
        //Vector2 leftPos = new Vector2(entity.position.x - 1, entity.position.y);
        //Collider[] colliders_Left = Physics.OverlapSphere(leftPos, radius);
        //if(colliders_Left.Length > 0)   //If we found colliders to our right, check if it's an enemy
        //{
        //    for (int i = 0; i < colliders_Left.Length; i++)
        //    {
        //        if (colliders_Left[i].gameObject.tag == "Enemy")
        //            return colliders_Left[i].gameObject as GameObject;  //if we found a gameobject, return it 
        //    }
        //}

        ////check up
        //Vector2 upPos = new Vector2(entity.position.x, entity.position.y + 1);  //If we found colliders to our right, check if it's an enemy
        //Collider[] colliders_Up = Physics.OverlapSphere(upPos, radius);
        //if(colliders_Up.Length > 0)
        //{
        //    for (int i = 0; i < colliders_Up.Length; i++)
        //    {
        //        if (colliders_Up[i].gameObject.tag == "Enemy")
        //            return colliders_Up[i].gameObject as GameObject;    //if we found a gameobject, return it 
        //    }
        //}

        ////check down
        //Vector2 downPos = new Vector2(entity.position.x, entity.position.y - 1);
        //Collider[] colliders_Down = Physics.OverlapSphere(downPos, radius);
        //if(colliders_Down.Length > 0)
        //{
        //    for (int i = 0; i < colliders_Down.Length; i++)
        //    {
        //        if (colliders_Down[i].gameObject.tag == "Enemy")
        //            return colliders_Down[i].gameObject as GameObject;
        //    }
        //}

        //return empty GameObject if none found
        //return null;
        #endregion
    }


    /*
    * @param Transform
    * @returns null/0
    * @desc Moves player via Lerp/Time.deltaTime
    * @status Complete with grid implementation. Untested.
    */
    public IEnumerator Move(int xPositionInDungeonMap, int yPositionInDungeonMap, Direction dir)
    {
        isMoving = true;
        DungeonManager.DungeonTile endTile;
        DungeonManager.DungeonTile startTile;
        startTile = dungeonMap[xPositionInDungeonMap, yPositionInDungeonMap];

        startPos = dungeonMap[xPositionInDungeonMap, yPositionInDungeonMap].WorldPosition;
        t = 0;
        switch (dir)
        {
            case Direction.North:
                endTile = dungeonMap[xPositionInDungeonMap, yPositionInDungeonMap + 1];
                endPos = dungeonMap[xPositionInDungeonMap, yPositionInDungeonMap + 1].WorldPosition;
                break;
            case Direction.East:
                endTile = dungeonMap[xPositionInDungeonMap + 1, yPositionInDungeonMap];
                endPos = dungeonMap[xPositionInDungeonMap + 1, yPositionInDungeonMap].WorldPosition;
                break;
            case Direction.South:
                endTile = dungeonMap[xPositionInDungeonMap, yPositionInDungeonMap - 1];
                endPos = dungeonMap[xPositionInDungeonMap, yPositionInDungeonMap - 1].WorldPosition;
                break;
            default:
                endTile = dungeonMap[xPositionInDungeonMap - 1, yPositionInDungeonMap];
                endPos = dungeonMap[xPositionInDungeonMap - 1, yPositionInDungeonMap].WorldPosition;
                break;
        }
            
        while(t < 1f)
        {
            t += Time.deltaTime * walkSpeed;
            gameObject.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        startTile.Actor = null; //set old spot's actor as null
        endTile.Actor = gameObject; //set new spot's actor as this gameobject
        isMoving = false;
        yield return 0;

        #region Deprecated before grid implementation
        //isMoving = true;
        //startPos = entity.position;
        //t = 0;

        //endPos = new Vector3(startPos.x + System.Math.Sign(input.x), startPos.y + System.Math.Sign(input.y), startPos.z);

        //while (t < 1f)
        //{
        //    t += Time.deltaTime * walkSpeed;
        //    entity.position = Vector3.Lerp(startPos, endPos, t);
        //    yield return null;
        //}

        //isMoving = false;
        //yield return 0;
        #endregion
    }
}

public enum Direction
{
    North,
    East,
    South,
    West,
    None
}
