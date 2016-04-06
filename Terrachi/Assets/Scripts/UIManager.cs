using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

    GameObject[] pauseObjects;
    public Transform player;
    public Transform grapplingHook;

    Player playerScript;
    GrapplingHook grappleScript;

    // Use this for initialization
    void Start()
    {
        Time.timeScale = 1;
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        hidePaused();
        playerScript = player.GetComponent<Player>();
        grappleScript = grapplingHook.GetComponent<GrapplingHook>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                pause();
            }
            else if (Time.timeScale == 0)
            {
                resume();
            }
        }
    }

    //pauses the scene
    public void pause()
    {
        Time.timeScale = 0;
        showPaused();
        //Disable user input
        playerScript.enabled = false;
        grappleScript.enabled = false;
    }

    //resumes the scene
    public void resume()
    {
        Time.timeScale = 1;
        hidePaused();
        //Enable user input
        playerScript.enabled = true;
        grappleScript.enabled = true;
    }

    //shows objects with ShowOnPause tag
    public void showPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
        }
    }

    //hides objects with ShowOnPause tag
    public void hidePaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
    }

    public void quitToDesktop()
    {
        //If we are running in a standalone build of the game
        #if UNITY_STANDALONE
            //Quit the application
            Application.Quit();
        #endif

        //If we are running in the editor
        #if UNITY_EDITOR
            //Stop playing the scene
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
