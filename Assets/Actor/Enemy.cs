/*
*TODO
*   -Animations
*   -moveToPosition(..) function isn't working
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class Enemy : Actor {

    //Class variables
    public Actor player;
    public bool canMove = false;
    private Queue<AStar_Pathfinder.Node> pathToPlayer = new Queue<AStar_Pathfinder.Node>();
    int lastPlayerX = 0, lastPlayerY = 0;
    int movingToX, movingToY;
    public int engageDistance = 30;


    // Use this for initialization
    public void Start()
    {
        //initialize attributes
        level = 1;
        setStatsByLevel();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Actor>();
        updatePlayerCoords();
    }

    /*
    *@desc Checks to see if the player is 1 space away, then should engage in combat
    *@param null
    *@return bool should engage in combat or not
    *@status Working
    */
    public bool shouldEngageCombat()
    {
        int totalSpacesAway = Mathf.Abs((x - player.x) + (y - player.y));

        if (totalSpacesAway <= 1)
            return true;

        return false;
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
        attackPower = 10f + ((level-1) * 10f);
        defense = 0f + ((level - 1) * 10);
    }

    /*
    *@desc Checks if the enemy is in range of the player to begin moving towards
    *@param null
    *@return bool isInRange or not
    *@status Working
    */
    public bool isInRangeOfPlayer()
    {
        return (Mathf.Abs(player.x - x) + Mathf.Abs(player.y - y)) <= engageDistance;
    }

    /*
    *@desc Updates player coordinates
    *@param null
    *@return void
    *@status Working
    */
    public void updatePlayerCoords()
    {
        lastPlayerX = player.x; lastPlayerY = player.y;
    }

    /*
    *@desc Obtains a path to the player from the current position and sets the "pathToPlayer" property
    *@param int current 'x' position, int current 'y' position
    *@return void
    *@status Working
    */
    public void getPathToTarget(int x, int y)
    {
        AStar_Pathfinder pathFinder = GetComponent<AStar_Pathfinder>();
        pathFinder.init();
        pathFinder.setNodes(new Vector2(x, y), new Vector2(player.x, player.y));
        List<AStar_Pathfinder.Node> path = pathFinder.buildPath(pathFinder.startNode, pathFinder.destinationNode);
        pathToPlayer = new Queue<AStar_Pathfinder.Node>(path);
        path = null;
        
        Debug.Log("Queue len: " + pathToPlayer.Count);
    }

    /*
    *@desc Attacks the player
    *@param null
    *@return void
    *@status Working
    */
    public void attackPlayer()
    {
        //Attack animation & face the player
        gameObject.transform.LookAt(player.gameObject.transform);    //face the player

        //Combat stats
        Player playerComp = player.GetComponent<Player>();
        float totalAttack = attackPower - playerComp.defense;
        playerComp.health -= totalAttack;
    }

    // Update is called once per frame
    void Update()
    {
        canMove = true;

        //Not tested
        if (x == movingToX && y == movingToY)
            canMove = true; //We've completed our move and can move again

        //if is in range of player, and the player has moved, recalculate path and update player coordinates
        if (isInRangeOfPlayer() && (lastPlayerX != player.x || lastPlayerY != player.y))
        {
            getPathToTarget(x, y);
            updatePlayerCoords();
        }

        //if we can move and are not on top of the player and are in range of player, move towards the player
        if (canMove && pathToPlayer.Count > 0 && isInRangeOfPlayer())
        {
            AStar_Pathfinder.Node newpos = pathToPlayer.Dequeue();
            moveToPosition((int)newpos.Position.x, (int)newpos.Position.y);
            gameObject.transform.LookAt(new Vector3(newpos.Position.x, newpos.Position.y, gameObject.transform.position.z));    //always face direction moving
            movingToX = (int)newpos.Position.x;
            movingToY = (int)newpos.Position.y;
            canMove = false;
        }
	}

    /*
    * Used for unit testing only. Will not be used in final build
    */
    public void UNITTEST()
    {
        level = 2;
        setStatsByLevel();
        bool isRange = isInRangeOfPlayer();
        getPathToTarget(x, y);
        Queue<AStar_Pathfinder.Node> tempPath = pathToPlayer;
        bool shouldCombat = shouldEngageCombat();

        updatePlayerCoords();
        attackPlayer();
    }
}
