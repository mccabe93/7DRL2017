using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor {
    

    public void playerControls()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            moveToPosition(x - 1, y);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            moveToPosition(x + 1, y);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            moveToPosition(x, y - 1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            moveToPosition(x, y + 1);
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            //moveToPosition(x, y);
            animator.Play("attack_left");
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
}
