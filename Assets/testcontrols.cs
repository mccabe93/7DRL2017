using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testcontrols : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.D))
            this.gameObject.transform.Translate(Vector3.right * ApplicationConstants.TILE_WIDTH);
	}
}
