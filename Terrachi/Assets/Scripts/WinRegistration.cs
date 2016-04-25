using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WinRegistration : MonoBehaviour {

    readonly private string RegisterURL = "http://terrachi.azurewebsites.net/win.php";
    readonly private string startMenuScene = "StartMenu";

    public bool error = false;

    public void Start()
    {
        Debug.Log("Gonna do this save here thing");
        SaveLoad.Load();

        SaveLoad.save.win = true;

        SaveLoad.Save();

        StartCoroutine("RegisterWin");
    }

    public void OnCollisionEnter2D(Collision2D obj)
    {

        if(obj.gameObject.tag == "Player")
        {
            Debug.Log("Gonna do this save here thing");
            SaveLoad.Load();

            SaveLoad.save.win = true;

            SaveLoad.Save();

            StartCoroutine("RegisterWin");

        }
    }

    public void RegisterWinInterface()
    {
        StartCoroutine("RegisterWin");
    }

    private IEnumerator RegisterWin()
    {
        Debug.Log("Attempting win registration");

        WWWForm form = new WWWForm();
        form.AddField("key", SaveLoad.save.license_key);

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
                SaveLoad.save.winAck = true;
                SaveLoad.Save();
                //SceneManager.LoadScene(startMenuScene);
            }
        }

        if (error)
        {
            Debug.Log("Attempt failed");
        } else
        {
            Debug.Log("Attempt successful");
        }
    }
}
