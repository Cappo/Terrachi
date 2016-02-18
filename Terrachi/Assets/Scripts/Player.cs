using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))] //automically adds Controller2D upon attaching script to gameObject
public class Player : MonoBehaviour {

   
    public float jumpHeight = 4; //how high do we want our character to be able jump?
    public float timeToJumpApex = .4f; //how long do we want our character to take to reach the highest point in his/her jump?
    float moveSpeed = 6;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;

    //jumpHeight and timeToJumpApex will determine what gravity and jumpVelocity are set to.  
    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;


    

    Controller2D controller; //reference to the controller

	// Use this for initialization
	void Start () {
        controller = GetComponent<Controller2D>();

        //set up jump physics:
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
	}
	
	// Update is called once per frame
	void Update () {

        //this line prevents accumulation of the force of gravity while on the ground.
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        //if the "Space" key is pressed and the player is standing on something
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        float targetVelocityX = input.x * moveSpeed; //velocity.x = to this initially
        
        //last parameter meaning: if we are grounded (controller.collisions.below = true) use acceleration time grounded, otherwise use airborne.
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne); //SmoothDamp smoothes movement, (realistic acceleration)
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

	}
}


/* Math Stuffs for Jump Physics
 Known: jumpHeight, timeToJumpApex
 Solving For: gravity, jumpVelocity

    Solving For Gravity:
        // this kinematic equation solves for gravity:
            deltaMovement = velocityInitial * time + (acceleration*(time^2) / 2)

       // If we replace these values with our own variables.. (we can leave out velocityInitial * time because at the apex of the jump initial velocity will be zero)
       // deltaMovement becomes jumpHeight, acceleration becomes gravity, time becomes timeToJumpApex
            New Equation: jumpHeight = ( gravity * (timeToJumpApex^2) ) / 2
       // Now all we need to do is solve for gravity: multiply both sides by 2, divide both sides by gravity, divide 1 by the whole equation to get reciprocal, multiply both sides by 2 * jumpHeight and we get:

            gravity = (2 * jumpHeight) / ( timeToJumpApex^2 )
    
    Solving For Jump Velocity:
        //Now that we have gravity we can solve for jump velocity using the physics equation for final velocity.
            velocityFinal = velocityInitial + acceleration * time
        // replace these values with our own variables: (once again we leave out velocityInitial because it will be 0) 
        jumpVelocity = gravity * timeToJumpApex;


        


    */