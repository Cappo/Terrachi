using UnityEngine;
using System.Collections;

public class KillPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    //nothing
	}
	
	// Update is called once per frames
	void Update () {
	    //Nothing
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Player p = GameObject.Find("KodamaPlayer").GetComponent<Player>();
            p.Respawn();
        }
    }
}
