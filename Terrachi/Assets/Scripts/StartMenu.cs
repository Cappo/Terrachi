using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour {

    readonly public string startLevel = "LevelOneAlpha";
    private bool saveExists = false;

    public Texture2D logoTex;
    public Texture2D btnBGNormal;
    public Texture2D btnBGHover;

    public void Start()
    {
        SaveLoad.Load();

        if (SaveLoad.save.currentLevel != "NA")
        {
            this.saveExists = true;
        }

        if (SaveLoad.save.win && !SaveLoad.save.winAck)
        {
            Debug.Log("Previous win was not acknowledged");

            WinRegistration wr = gameObject.AddComponent<WinRegistration>();
            wr.RegisterWinInterface();
        }

        if (SaveLoad.save.win)
        {
            Debug.Log("Player has won the game");
        }
        if (SaveLoad.save.winAck)
        {
            Debug.Log("Win has been acknowledged");
        }

        Debug.Log("Current Level: " + SaveLoad.save.currentLevel);
        Debug.Log("Checkpoint: " + SaveLoad.save.checkpoint);
    }

    public void NewGame()
    {
        //SaveLoad.Load();
        SaveLoad.save.currentLevel = startLevel;
        SaveLoad.save.checkpoint = "";
        SaveLoad.Save();
        SceneManager.LoadScene(startLevel);
    }

    public void LoadGame()
    {
        //SaveLoad.Load();
        SceneManager.LoadScene(SaveLoad.save.currentLevel);
    }

    public void QuitGame()
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

    public void OnGUI()
    {
        if (!btnBGNormal || !logoTex || !btnBGHover)
        {
            Debug.LogError("Please assign a texture on the inspector");
            return;
        }
        //GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

        //GUIStyle headerFont = new GUIStyle();
        //headerFont.normal.textColor = Color.green;
        //headerFont.fontSize = 24;
        //headerFont.alignment = TextAnchor.MiddleCenter;
        //GUI.Label(new Rect(0, Screen.height / 6, Screen.width, 50), "Terrachi", headerFont);

        GUIStyle logoStyle = new GUIStyle();
        logoStyle.alignment = TextAnchor.MiddleCenter;
        GUI.DrawTexture(new Rect(10, 10, Screen.width-10, Screen.height/2 - 20), logoTex, ScaleMode.ScaleToFit, true);


        GUIStyle btnStyle = new GUIStyle();
        btnStyle.normal.background = btnBGNormal;
        btnStyle.normal.textColor = Color.white;
        btnStyle.hover.background = btnBGHover;
        btnStyle.hover.textColor = Color.white;
        btnStyle.alignment = TextAnchor.MiddleCenter;

        if (GUI.Button(new Rect((Screen.width / 2) - 60, Screen.height * 2 / 3 - 20, 120, 30), "New Game", btnStyle))
        {
            StartCoroutine("NewGame");
        }

        if (saveExists)
        {
            if (GUI.Button(new Rect((Screen.width / 2) - 60, Screen.height * 2 / 3 + 20, 120, 30), "Load Game", btnStyle))
            {
                StartCoroutine("LoadGame");
            }
        }

        if (GUI.Button(new Rect((Screen.width / 2) - 60, Screen.height * 2 / 3 + 60, 120, 30), "Exit Game", btnStyle))
        {
            StartCoroutine("QuitGame");
        }
    }

}
