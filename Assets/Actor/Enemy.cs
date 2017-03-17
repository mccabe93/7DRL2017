using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class Enemy : Actor {
    public Actor player;
    public bool canMove = false;
    // Use this for initialization
    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Actor>();
    }

    public void getPathToTarget(int x, int y)
    {
    }

    // Update is called once per frame
    void Update()
    {
	}
}
