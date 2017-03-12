using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class Enemy : Actor {
    public Actor player;
    public bool canMove = false;
    private Queue<Vector2i> path = new Queue<Vector2i>();
    int lastPlayerX = 0, lastPlayerY = 0;
    public int engageDistance = 30;
    // Use this for initialization
    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Actor>();
        updatePlayerCoords();
    }

    public bool isInRangeOfPlayer()
    {
        return (Mathf.Abs(player.x - x) + Mathf.Abs(player.y - y)) <= engageDistance;
    }

    public void updatePlayerCoords()
    {
        lastPlayerX = player.x; lastPlayerY = player.y;
    }

    public void getPathToTarget(int x, int y)
    {
        Pathfinder pf = new Pathfinder(x, y);
        pf.findNewPath(this.x, this.y);
        path = new Queue<Vector2i>();
        foreach (Vector2i node in pf.path)
        {
            path.Enqueue(node);
        }
        pf = null;
        Debug.Log("Queue len: " + path.Count);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInRangeOfPlayer() && (lastPlayerX != player.x || lastPlayerY != player.y))
        {
            getPathToTarget(player.x, player.y);
            updatePlayerCoords();
        }
        if (canMove && path.Count > 0)
        {
            Vector2i newpos = path.Dequeue();
            moveToPosition(newpos.x, newpos.y);
            canMove = false;
        }
	}
}
