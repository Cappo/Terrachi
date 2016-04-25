using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

    public float moveSpeed;
    public Transform target;
    public int rotationSpeed;

	// Use this for initialization
	void Start () {
        target = GameObject.Find("Player").transform;
	}

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            // Only needed if objects don't share 'z' value.
            dir.z = 0.0f;
            /*
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.FromToRotation(Vector3.right, dir),
                    rotationSpeed * Time.deltaTime);
            */
            //Move Towards Target
            transform.position += (target.position - transform.position).normalized
                * moveSpeed * Time.deltaTime;
        }
        //rigidbody2D.velocity = Vector3.zero;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Destroy(col.gameObject);
        }
    }
}

