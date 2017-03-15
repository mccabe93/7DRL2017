using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;

    // Use this for initialization
    public void init()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    public void updateCamera()
    {
        if (player == null)
            return;
        Vector3 playerpos = player.transform.position;
        playerpos.z -= 200;
        transform.position = playerpos;
    }
}
