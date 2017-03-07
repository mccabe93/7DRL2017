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

    // Use this for initialization
    void Start ()
    {
        currentDir = Direction.North;
        toMove = true;
        isAllowedToMove = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //able to move by default unless specified otherwise
        toMove = true;

        #region Movement handler
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
                StartCoroutine(Move(transform));
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

        #region Combat handler
        GameObject combatanant = CheckForCombat(gameObject.transform);
        if(combatanant != null)
        {
            //then we have some combat-ing to do
            InitiateCombat(gameObject, combatanant);
        }
        #endregion

    }

    /*
    * @param GameObject attacker who is 'attacking" (player), GameObject enemy who is "being attacked
    * @returns null
    * @desc Begins combat of two GameObjects
    * @status Incomplete
    */
    public void InitiateCombat(GameObject attacker, GameObject enemy)
    {
        //Initiate combat?

    }


    /*
    * @param Transform 'entity' holding the transform (probably not necessary anymore)
    * @returns GameObject 'Combatanant"
    * @desc Checks for neearby (non-diagonal, 1 pixel away) enemies and returns their GameObject
    * @status Completed / Untested.
    */
    public GameObject CheckForCombat(Transform entity)
    {

        //raidus to check for colliders
        float radius = 1.0f;

        //check right
        Vector2 rightPos = new Vector2(entity.position.x + 1, entity.position.y);
        Collider[] colliders_Right = Physics.OverlapSphere(rightPos, radius);
        if (colliders_Right.Length > 0) //If we found colliders to our right, check if it's an enemy
        {
            for (int i = 0; i < colliders_Right.Length; i++)
            {
                if (colliders_Right[i].gameObject.tag == "Enemy")
                    return colliders_Right[i].gameObject as GameObject; //if we found a gameobject, return it 
            }
        }

        //check left
        Vector2 leftPos = new Vector2(entity.position.x - 1, entity.position.y);
        Collider[] colliders_Left = Physics.OverlapSphere(leftPos, radius);
        if(colliders_Left.Length > 0)   //If we found colliders to our right, check if it's an enemy
        {
            for (int i = 0; i < colliders_Left.Length; i++)
            {
                if (colliders_Left[i].gameObject.tag == "Enemy")
                    return colliders_Left[i].gameObject as GameObject;  //if we found a gameobject, return it 
            }
        }

        //check up
        Vector2 upPos = new Vector2(entity.position.x, entity.position.y + 1);  //If we found colliders to our right, check if it's an enemy
        Collider[] colliders_Up = Physics.OverlapSphere(upPos, radius);
        if(colliders_Up.Length > 0)
        {
            for (int i = 0; i < colliders_Up.Length; i++)
            {
                if (colliders_Up[i].gameObject.tag == "Enemy")
                    return colliders_Up[i].gameObject as GameObject;    //if we found a gameobject, return it 
            }
        }

        //check down
        Vector2 downPos = new Vector2(entity.position.x, entity.position.y - 1);
        Collider[] colliders_Down = Physics.OverlapSphere(downPos, radius);
        if(colliders_Down.Length > 0)
        {
            for (int i = 0; i < colliders_Down.Length; i++)
            {
                if (colliders_Down[i].gameObject.tag == "Enemy")
                    return colliders_Down[i].gameObject as GameObject;
            }
        }

        //return empty GameObject if none found
        return null;
    }


    /*
    * @param Transform
    * @returns null/0
    * @desc Moves player via Lerp/Time.deltaTime
    * @status Working
    */
    public IEnumerator Move(Transform entity)
    {
        isMoving = true;
        startPos = entity.position;
        t = 0;

        endPos = new Vector3(startPos.x + System.Math.Sign(input.x), startPos.y + System.Math.Sign(input.y), startPos.z);

        while (t < 1f)
        {
            t += Time.deltaTime * walkSpeed;
            entity.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        isMoving = false;
        yield return 0;
    }
}

enum Direction
{
    North,
    East,
    South,
    West,
    None
}
