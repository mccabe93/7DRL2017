/*
*TODO
*   -Animations for attacking
*   -Flipping sprites based on movement / direction facing
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Actor
{
    //class variables
<<<<<<< HEAD
=======
    private Direction currentDir;
    public Slider healthBar;
>>>>>>> 9afa0ef5ee2fe256a432f47b6e92c3f6a8e17433

    public void Start()
    {
        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
        currentDir = Direction.West;
        level = 1;
        setStatsByLevel();
        animator = GetComponent<Animator>();
    }

    public void Update()
    {
        setPlayerHealthBar();
    }

    /*
    * @status Unfinished (need animations)
    */
    public void playerControls()
    {
        int lastX = x;
        int lastY = y;

       
        if (Input.GetKeyDown(KeyCode.S))
        {
            currentDir = Direction.West;
            moveToPosition(x - 1, y);
            if(x == lastX && y == lastY)
            {
                //Then we weren't able to move, and have not moved. But need to flip the Sprite
                //Flip to West facing sprite
            }
        }
            
        else if (Input.GetKeyDown(KeyCode.W))
        {
            currentDir = Direction.East;
            moveToPosition(x + 1, y);
            if (x == lastX && y == lastY)
            {
                //Then we weren't able to move, and have not moved. But need to flip the Sprite
                //Flip to East facing sprite
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            currentDir = Direction.North;
            moveToPosition(x, y - 1);
            if (x == lastX && y == lastY)
            {
                //Then we weren't able to move, and have not moved. But need to flip the Sprite
                //Flip to North facing sprite
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            currentDir = Direction.South;
            moveToPosition(x, y + 1);
            if (x == lastX && y == lastY)
            {
                //Then we weren't able to move, and have not moved. But need to flip the Sprite
                //Flip to South facing sprite
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            //moveToPosition(x, y);
            attack();
        }
        /*
        // octodirectional controls
        if (Input.GetKeyDown(KeyCode.Z))
        {
            moveToPosition(x - 1, y);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            moveToPosition(x + 1, y);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            moveToPosition(x, y - 1);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            moveToPosition(x, y + 1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            moveToPosition(x + 1, y + 1);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            moveToPosition(x - 1, y + 1);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            moveToPosition(x - 1, y - 1);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            moveToPosition(x + 1, y - 1);
        }
        */
    }


    /*
    *@desc Gets actor in front of player (for enemy combat purposes)
    *@param null
    *@return GameObject actor in front
    *@status Working
    */
    public GameObject getActorInFront()
    {
        //Get position in front of player
        GameObject positionInFront = new GameObject();
        switch (currentDir)
        {
            case Direction.West:
                if ((x - 1) >= 0)
                    return DungeonManager.WorldGrid[x - 1, y].Actor;
                break;
            case Direction.East:
                if ((x + 1) < ApplicationConstants.DUNGEON_WIDTH)
                    return DungeonManager.WorldGrid[x + 1, y].Actor;
                break;
            case Direction.North:
                if ((y - 1) >= 0)
                    return DungeonManager.WorldGrid[x, y - 1].Actor;
                break;
            case Direction.South:
                if ((y + 1) < ApplicationConstants.DUNGEON_HEIGHT)
                    return DungeonManager.WorldGrid[x, y + 1].Actor;
                break;
            default:
                return null;
        }

        return null;
    }

    /*
    *@desc Attacks an enemy
    *@param null
    *@return void
    *@status Working BUT / Needs animation added
    */
    public void attack()
    {
        //Get enemy to combat with
        GameObject en = getActorInFront();
        if (en == null || en.gameObject.tag != "Enemy")
            return;

        //Attack animation
        string anim = null;
        switch (currentDir)
        {
            case Direction.South:
                anim = "attack_right";
                break;
            case Direction.West:
                anim = "attack_left";
                break;
            case Direction.East:
                anim = "attack_back_right";
                break;
            case Direction.North:
                anim = "attack_back_left";
                break;
        }
        animator.Play(anim);

        //Combat stats
        Enemy enemyComp = en.GetComponent<Enemy>();
        float totalAttack = attackPower - enemyComp.defense;
        enemyComp.health -= totalAttack;
    }

    /*
    *@desc Checks to see if an enemy is 1 space away, then should engage in combat
    *@param null
    *@return GameObject who we should engage with
    *@status Working
    */
    public GameObject canEngageCombat()
    {
        //check up (if inbounds)
        if((y+1) < ApplicationConstants.DUNGEON_HEIGHT)
        {
            var occupier = DungeonManager.WorldGrid[x, y+1].Actor;
            if (occupier != null && occupier.gameObject.tag == "Enemy")
                return occupier;
        }

        //check right
        if((x+1) < ApplicationConstants.DUNGEON_WIDTH)
        {
            var occupier = DungeonManager.WorldGrid[x + 1, y].Actor;
            if (occupier != null && occupier.gameObject.tag == "Enemy")
                return occupier;
        }

        //check down
        if((y-1) >= 0)
        {
            var occupier = DungeonManager.WorldGrid[x, y - 1].Actor;
            if (occupier != null && occupier.gameObject.tag == "Enemy")
                return occupier;
        }

        //check left
        if((x-1) >= 0)
        {
            var occupier = DungeonManager.WorldGrid[x - 1, y].Actor;
            if (occupier != null && occupier.gameObject.tag == "Enemey")
                return occupier;
        }

        //none found
        return null;
    }

    /*
    *@desc Sets the enemy's stats based on the current level
    *@param null
    *@return void
    *@status Working
    */
    public void setStatsByLevel()
    {
        health = 100f;
        attackPower = 10f + ((level - 1) * 10f);
        defense = 0f + ((level - 1) * 10);
    }

    /*
    *@desc Sets the health bar
    *@param null
    *@return void
    *@status Untested
    */
    public void setPlayerHealthBar()
    {
        if (healthBar.value != health)
            healthBar.value = health;        
    }


}
