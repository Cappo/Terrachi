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
    public Texture2D btnBGClick;
    public Texture2D background;
    public Font btnFont;

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

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background, ScaleMode.ScaleAndCrop, true);

        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), ""); // Transparent box to make the background a little darker

        GUI.DrawTexture(new Rect(10, 10, Screen.width-10, Screen.height/2 - 20), logoTex, ScaleMode.ScaleToFit, true);


        GUIStyle btnStyle = new GUIStyle();
        btnStyle.normal.background = btnBGNormal;
        btnStyle.normal.textColor = Color.white;
        btnStyle.hover.background = btnStyle.focused.background = btnBGHover;
        btnStyle.hover.textColor = btnStyle.focused.textColor = Color.white;
        btnStyle.active.background = btnBGClick;
        btnStyle.active.textColor = Color.white;
        btnStyle.alignment = TextAnchor.MiddleCenter;
        btnStyle.font = btnFont;

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
