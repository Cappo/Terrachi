using UnityEngine;
using System.Collections;

public class DialogManager : MonoBehaviour {

    public string headerText;
    public TextAsset textFile;
    public string dialogText;

    GameObject[] pauseObjects;
    public Transform player;
    public Transform grapplingHook;
    public bool isPaused;

    public Font headerFont;
    public Font dialogFont;

    readonly private string startMenuScene = "StartMenu";
    
    public GUIStyle dialogStyle;
    public GUIStyle btnStyle;

    public bool triggered;

    Player playerScript;
    GrapplingHook grappleScript;

    // Use this for initialization
    void Start()
    {
        Time.timeScale = 1;
        triggered = false;
        isPaused = false;
        player = GameObject.Find("KodamaPlayer").GetComponent<Transform>();
        grapplingHook = GameObject.Find("Grappling Hook").GetComponent<Transform>();
        playerScript = GameObject.Find("KodamaPlayer").GetComponent<Player>();
        grappleScript = GameObject.Find("Grappling Hook").GetComponent<GrapplingHook>();
        dialogText = textFile.text;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (other.name == "KodamaPlayer")
        {
            Debug.Log("Player entered collider");
            triggered = true;
            pause();
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

        GUI.Label(new Rect(10, 10, Screen.width - 20, Screen.height / 3), headerText, headerStyle);

        /*GUIStyle dialogStyle = new GUIStyle();
        headerStyle.font = ;
        headerStyle.fontSize = 12;
        headerStyle.alignment = TextAnchor.UpperLeft;
        headerStyle.normal.textColor = Color.white;*/

        //GUI.Label(new Rect(10, Screen.height / 2 + 10, Screen.width - 20, Screen.height / 4), dialogText, dialogStyle);
        
        GUI.Label(new Rect(Screen.width/4, Screen.height / 3 + 10, Screen.width/2, Screen.height / 3), dialogText, dialogStyle);

        if (GUI.Button(new Rect((Screen.width / 2) - 60, Screen.height * 2 / 3 + 70, 120, 30), "Resume", btnStyle))
        {
            StartCoroutine("resume");
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
}
