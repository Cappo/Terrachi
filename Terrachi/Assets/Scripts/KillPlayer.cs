using UnityEngine;
using System.Collections;

public class KillPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    //nothing
	}
	
	// Update is called once per frame
	void Update () {
	    //Nothing
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Destroy(col.gameObject);
        }
    }
}
