using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour {

    private Animator myAnimator;
    public float moveSpeed;
    // Use this for initialization
    void Start () {
        myAnimator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector3(moveSpeed, 0, 0) * Time.deltaTime);
        myAnimator.SetFloat("animSpeed", Mathf.Abs(moveSpeed));
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Obstacle")
        {
            moveSpeed *= -1;
            Vector3 new_scale = transform.localScale;
            new_scale.x *= -1;
            transform.localScale = new_scale;

        }
        if (col.gameObject.tag == "Player")
        {
            //Destroy(col.gameObject);
            Player p = GameObject.Find("KodamaPlayer").GetComponent<Player>();
            p.Respawn();

        }
    }
}
