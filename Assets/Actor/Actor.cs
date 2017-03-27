using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class Actor : MonoBehaviour
{
    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    public float health;
    public float attackPower;
    public float defense;
    public int level;
    public int x, y;
    public Animator animator;
    public string currentAnimation = "idle_left";
    protected Direction currentDir = Direction.West;
    // Use this for initialization
    void Start()
    {
        animator.Play(currentAnimation);
    }

    // Update is called once per frame
    void Update()
    {
        //       Debug.Log("actor playing anim " + currentAnimation);
        /*
        if(inMotion)
        {
            position = Vector3.Lerp(position, destination, Time.deltaTime);
            if(position == destination) {
                inMotion = false;
                GameManager.moveActor(currentGridX, currentGridY, lastGridX, lastGridY);
            }
            Debug.Log("motioning");
        }
        */
    }

    bool AnimatorIsPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length >
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void returnToIdle()
    {
        string anim = null;
        switch (currentDir) {
            case Direction.South:
                anim = "idle_right";
                break;
            case Direction.West:
                anim = "idle_left";
                break;
            case Direction.East:
                anim = "idle_back_right";
                break;
            case Direction.North:
                anim = "idle_back_left";
                break;
        }
        animator.Play(anim);
    }

    public void setCurrentAnimation(string anim)
    {
        currentAnimation = anim;
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
        if (nx >= 0 && nx < ApplicationConstants.DUNGEON_WIDTH &&
            ny >= 0 && ny < ApplicationConstants.DUNGEON_HEIGHT)
        {
            var occupier = DungeonManager.WorldGrid[nx, ny].Actor;
            if (occupier == null && DungeonManager.WorldGrid[nx, ny].Cost < 10000)
            {
                if (DungeonManager.moveActor(this, nx, ny))
                {
                    x = nx;
                    y = ny;
                    string anim = null;
                    switch (currentDir)
                    {
                        case Direction.South:
                            anim = "move_right";
                            break;
                        case Direction.West:
                            anim = "move_left";
                            break;
                        case Direction.East:
                            anim = "move_back_right";
                            break;
                        case Direction.North:
                            anim = "move_back_left";
                            break;
                    }
                    animator.Play(anim);
                    return 0;
                }
            }
            else if (occupier != null)
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
