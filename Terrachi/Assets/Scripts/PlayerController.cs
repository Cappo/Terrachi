using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float moveSpeed;
    public float jumpHeight;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, jumpHeight);
        }

        if (Input.GetKeyDown (KeyCode.D))
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
        }



    }
}
