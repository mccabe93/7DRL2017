using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class Actor : MonoBehaviour {
    public int x, y;
    public Animator animator;
    public string currentAnimation = "idle_left";

    // Use this for initialization
    void Start () {
        animator.Play(currentAnimation);
	}
	
	// Update is called once per frame
	void Update () {
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
        animator.Play("idle_left");
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
        if(nx >= 0 && nx < ApplicationConstants.DUNGEON_WIDTH &&
            ny >= 0 && ny < ApplicationConstants.DUNGEON_HEIGHT)
        {
            var occupier = DungeonManager.WorldGrid[nx, ny].Actor;
            if (occupier == null && DungeonManager.WorldGrid[nx,ny].Cost < 10000)
            {
                if (DungeonManager.moveActor(this, nx, ny))
                {
                    x = nx;
                    y = ny;
                    animator.Play("run_left");
                    return 0;
                }
            }
            else if(occupier != null)
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
