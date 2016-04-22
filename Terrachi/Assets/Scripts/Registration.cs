using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Registration : MonoBehaviour {

    private string key = "";

    private bool error;
    private string errorText = "Game could not be registered with the given key; either a game has already been registered with this key or you do not have an internet connection.";

    private bool checkedYet;

    readonly private string startMenuScene = "StartMenu";

    readonly private string RegisterURL = "http://terrachi.azurewebsites.net/activate.php";

    void Awake()
    {
        checkedYet = false;
    }

	// Use this for initialization
	void Start () {
        error = false;

        SaveLoad.Load();
        if (SaveLoad.save.registered == true && SaveLoad.save.uid == SystemInfo.deviceUniqueIdentifier)
        {
            SceneManager.LoadScene(startMenuScene);
        }

        checkedYet = true;
	}
	
	// Display our registration form
    void OnGUI () {
        if (checkedYet)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

            if (GUI.Button(new Rect((Screen.width / 2) - 30, Screen.height * 2 / 3, 60, 30), "Register"))
            {
                error = false;
                StartCoroutine("RegisterKey");
            }

            key = GUI.TextField(new Rect(50, Screen.height / 3, Screen.width - 100, 20), key);

            if (error)
            {
                GUI.Label(new Rect(50, Screen.height / 2, Screen.width - 100, Screen.height / 5), this.errorText);
            }
        }
    }

    IEnumerator RegisterKey() {
        WWWForm form = new WWWForm();
        form.AddField("key", key);

        WWW w = new WWW(RegisterURL, form);
        yield return w;
        if (!string.IsNullOrEmpty(w.error))
        {
            error = true;
        }
        else {
            string msg = w.text;

            if (msg == "0")
            {
                error = true;
            }
            else {
                SaveLoad.save.registered = true;
                SaveLoad.save.uid = SystemInfo.deviceUniqueIdentifier;
                //SaveLoad.save.currentLevel = startMenuScene;
                SaveLoad.Save();
                SceneManager.LoadScene(startMenuScene);
            }
        }
    }
}
