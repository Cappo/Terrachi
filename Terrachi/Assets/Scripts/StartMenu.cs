using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

    public string startLevel;


    public void NewGame()
    {
        SceneManager.LoadScene(startLevel);
    }

    public void LoadGame()
    {
        //TO DO FOR NICK


    }

    public void QuitGame()
    {


        Application.Quit();
    }

}
