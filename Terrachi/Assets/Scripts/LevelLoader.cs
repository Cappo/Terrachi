using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    private bool playerInZone;

    public string levelToLoad;

	// Use this for initialization
	void Start () {
        playerInZone = false; 
	}
	
	// Update is called once per frame
	void Update () {
        if ( playerInZone ) {
            SaveLoad.save.checkpoint = "";
            SaveLoad.save.currentLevel = levelToLoad;
            SaveLoad.Save();
            SceneManager.LoadScene(levelToLoad);
        }
	}

    void OnTriggerEnter2D(Collider2D other) {

        if (other.tag == "Player")
        {
            playerInZone = true; 
        }

        
    }

    void OnTriggerExit2D(Collider2D other)
    {

        if (other.name == "Player")
        {
            playerInZone = false;
        }


    }
}
