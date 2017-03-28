using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /*
    * Takes to first level
    */
    public void toGame()
    {
        SceneManager.LoadScene("Level1");
    }

    /*
    * Quits the game
    */
    public void toQuit()
    {
        Application.Quit();
    }
}
