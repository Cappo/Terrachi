using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StoryBoard : MonoBehaviour {

    public TextAsset storyTextFile;
    private string storyText;
    public Font textFont;
    private float textTop;
    public GUIStyle textStyle;
    public Texture2D background;
    public float movementScaler;
    public string nextScene = "LevelOneAlpha";

	// Use this for initialization
	void Start () {
        textTop = Screen.height;
        storyText = storyTextFile.text;
    }

    // Update is called once per frame
    void Update()
    {
        textTop -= movementScaler;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background, ScaleMode.ScaleAndCrop, true);

        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), ""); // Transparent box to make the background a little darker

        GUI.Label(new Rect(10, textTop, Screen.width - 10, 10000), storyText, textStyle);
    }
}
