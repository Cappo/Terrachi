using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    GameObject[] pauseObjects;
    public Transform player;
    public Transform grapplingHook;
    public bool isPaused;

    public Font headerFont;

    readonly private string startMenuScene = "StartMenu";

    Player playerScript;
    GrapplingHook grappleScript;

    public Font btnFont;
    public Texture2D btnBGNormal;
    public Texture2D btnBGHover;
    public Texture2D btnBGClick;

    public GUIStyle btnStyle;

    // Use this for initialization
    void Start()
    {
        Time.timeScale = 1;
        isPaused = false;
        player = GameObject.Find("KodamaPlayer").GetComponent<Transform>();
        grapplingHook = GameObject.Find("Grappling Hook").GetComponent<Transform>();
        playerScript = GameObject.Find("KodamaPlayer").GetComponent<Player>();
        grappleScript = GameObject.Find("Grappling Hook").GetComponent<GrapplingHook>();
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
        isPaused = true;
        playerScript.enabled = false;
        grappleScript.enabled = false;
    }

    //resumes the scene
    public void resume()
    {
        Time.timeScale = 1;
        isPaused = false;
        playerScript.enabled = true;
        grappleScript.enabled = true;
    }

    public void OnGUI()
    {
        if (!isPaused)
        {
            return;
        }

        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

        GUIStyle headerStyle = new GUIStyle();
        headerStyle.font = headerFont;
        headerStyle.fontSize = 48;
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.normal.textColor = Color.white;

        GUI.Label(new Rect(10, 10, Screen.width - 20, Screen.height / 2), "Game Paused", headerStyle);

        /*GUIStyle btnStyle = new GUIStyle();
        btnStyle.normal.background = btnBGNormal;
        btnStyle.normal.textColor = Color.white;
        btnStyle.hover.background = btnStyle.focused.background = btnBGHover;
        btnStyle.hover.textColor = btnStyle.focused.textColor = Color.white;
        btnStyle.active.background = btnBGClick;
        btnStyle.active.textColor = Color.white;
        btnStyle.alignment = TextAnchor.MiddleCenter;
        btnStyle.font = btnFont;*/

        if (GUI.Button(new Rect((Screen.width / 2) - 60, Screen.height * 2 / 3 - 50, 120, 30), "Resume", btnStyle))
        {
            StartCoroutine("resume");
        }

        if (GUI.Button(new Rect((Screen.width / 2) - 60, Screen.height * 2 / 3 - 10, 120, 30), "Respawn", btnStyle))
        {
            StartCoroutine("loadCheckpoint");
        }

        if (GUI.Button(new Rect((Screen.width / 2) - 60, Screen.height * 2 / 3 + 30, 120, 30), "Main Menu", btnStyle))
        {
            SaveLoad.Save();
            StartCoroutine("quitToMainMenu");
        }

        if (GUI.Button(new Rect((Screen.width / 2) - 60, Screen.height * 2 / 3 + 70, 120, 30), "Exit Game", btnStyle))
        {
            SaveLoad.Save();
            StartCoroutine("quitToDesktop");
        }
           
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

    public void loadCheckpoint()
    {
        Vector3 checkpoint = Checkpoint.GetActiveCheckpointPosition();
        checkpoint.y += 5;
        player.GetComponent<Player>().velocity = Vector3.zero;
        player.transform.position = checkpoint;
        //Move the camera too because apparently it can't do it itself.
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        camera.transform.position = checkpoint;
        resume();
    }

    public void quitToDesktop()
    {
        //If we are running in a standalone build of the game
#if UNITY_STANDALONE
            //Quit the application
            //SaveLoad.Save();
            Application.Quit();
#endif

        //If we are running in the editor
#if UNITY_EDITOR
            //Stop playing the scene
            //SaveLoad.Save();
            UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void quitToMainMenu()
    {
        //SaveLoad.Save();
        SceneManager.LoadScene(startMenuScene);
    }
}
