using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]

/*The Player script is responsible for receiving input. Which it then sends to the controller script.
The controller scripts task is to move the player, while constraining to collisions.
It does this by sending out rays first horizontally, and then vertically in the direction the play is moving.
*/

public class Player : MonoBehaviour {

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f; //how long do we want our character to take to reach the highest point in his/her jump?
    float accelerationTimeAirborne = .075f;
	float accelerationTimeGrounded = .05f;
	float moveSpeed = 7;

    //jumpHeight and timeToJumpApex will determine what gravity and jumpVelocity are set to.
    float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	public Vector3 velocity;
	float velocityXSmoothing;

	Controller2D controller; //reference to the controller component attached to our player GameObject.
    private Animator animator; //create a variable to store a reference the Animator on the player

    void Awake() {
        
    }

    void Start() {
        controller = GetComponent<Controller2D>();
        SaveLoad.Load();
        Checkpoint.CheckpointsList = GameObject.FindGameObjectsWithTag("Checkpoint");

        Debug.Log("Player loaded checkpoint: " + SaveLoad.save.checkpoint);

        if (SaveLoad.save.checkpoint != "")
        {
            Checkpoint cp = GameObject.Find(SaveLoad.save.checkpoint).GetComponent<Checkpoint>();

            //cp.activated = true;
            cp.ActivateCheckpoint();

            this.Respawn();
        } 

        animator = GetComponent<Animator>(); //assign the Animator component on the player gameobject to our new reference var. 

        //set up jump physics
		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
		print ("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);
	}

    //Update is called once per frame
    void Update() {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        float targetVelocityX = input.x * moveSpeed; //initially set this to just velocity.x

        // if we are grounded (controller.collisions.below = true) use acceleration time grounded, otherwise use airborne.
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);







        //if the "Space" key is pressed 
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (controller.collisions.below) {
                velocity.y = maxJumpVelocity;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            if (velocity.y > minJumpVelocity) {
                velocity.y = minJumpVelocity;
            }
        }



        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime, input);

        //this line prevents accumulation of the force of gravity while on the ground.
        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }

        //Test "death" scenario, can be removed later
        if (transform.position.y < -100)
        {
            this.Respawn();

        }

        animator.SetFloat("speed", velocity.magnitude);
        animator.SetBool("grounded", controller.collisions.below);

        //Flip animation
        if (input.x != 0 && Mathf.Sign(input.x) != Mathf.Sign(transform.localScale.x))
        {
            Vector3 new_scale = transform.localScale;
            new_scale.x *= -1;
            transform.localScale = new_scale;
        }
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.collider.tag == "Hazard")
        {
            Respawn();
            //print("hi");

        }
    }

    public void Respawn()
    {
        Vector3 respawnPoint = Checkpoint.GetActiveCheckpointPosition();
        respawnPoint.y += 5;
        velocity = Vector3.zero;
        transform.position = respawnPoint;
    }
}

/*
 void flip ( ref Controller2D controller, Vector2 input )
{
   
    //If moving right (input.x > 0) and currently facing left (facedir == -1)  OR  If moving left (input.x < 0) and currently facing right (faceDir == 1) THEN:
    if (input.x > 0 && controller.collisions.faceDir == -1 || input.x < 0 && controller.collisions.faceDir == 1)
    {
        //flip the player
        Vector3 theScale = controller.transform.localScale; //grab the local scale from the player and assign it to a Vector3 var
        theScale.x = theScale.x * -1; //assign localScale to the opposite of it's current value 
        controller.transform.localScale = theScale; //set player's scale to be theScale 
                                         //move player.
    }

}
*/

/* How the jump physics work:
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
