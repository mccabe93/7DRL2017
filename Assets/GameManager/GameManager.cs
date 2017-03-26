using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject pauseMenu;
    public bool isPaused = false;

    // Use this for initialization
    void Start() {
       pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update() {

        //If pres Escape, then pause
        if (Input.GetKeyDown(KeyCode.Escape) && isPaused == false)
        {
            toPauseMenu();
        }

    }

    /*
    * Restarts the current level
    */
    public void toRestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /*
    * Brings up the pause menu
    */
    public void toPauseMenu()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
    }

    /*
    * Takes you to the main menu screen
    */
    public void toMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /*
    * Resumes the game from a paused context
    */
    public void toResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
    }

    /*
    * Quits the game
    */
    public void toQuit()
    {
        Application.Quit();
    }
}
