using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour {

    readonly public string startLevel = "LevelOneAlpha";

    public void Start()
    {
        SaveLoad.Load();
        Button loadButton = GameObject.Find("Load Game").GetComponent<Button>();

        if (SaveLoad.save.currentLevel == "NA")
        {
            loadButton.interactable = false;
        }
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
        Application.Quit();
    }

}
