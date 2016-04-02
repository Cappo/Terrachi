//Grappling Hook, based on the open source Grappling Hook 2D asset by Sykoo

using UnityEngine;
using System.Collections;

public class GrapplingHook : MonoBehaviour
{

    public Transform Player; //the object for player
    public Transform Anchor; //Anchor point
    public Transform ThisObject; //defining this object itself

    private HingeJoint2D PlayerHingeJoint; //the HingeJoint attached to the player object

    public bool Fired = false; //checking if the hook has been fired
    public bool Hooked = false; //checking if something has been hooked

    public float MovementSpeed = 0.2f; //movement speed for the hook object to fly to its destination
    public int MD = 10; //setting a limit to maximum distance, MD stands for "Max Distance"
    public float DFH; //calculating distance from hook holder, DFH stands for "Distance From HookHolder"

    private Vector3 lastPos; //calculating the last position of the mouse click in order to sen the hook object
    private Vector3 lastPosOO; //last position for OtherObject (lastPosOO stands for "last position other object"
    private Vector3 curPosOO; //current position for OtherObject (curPosOO stands for "current position other object"

    //public AudioClip ShootSound; //the sound effect when hook gets shot	

    public float Force;

    Controller2D controller;

    void Start()
    {
        controller = Player.GetComponent<Controller2D>();

    }

    void Update()
    {
        //calculating the mouse position
        var MousePosition = Input.mousePosition; //taking input for the positioning of the mouse
        MousePosition = Camera.main.ScreenToWorldPoint(MousePosition); //setting the position reading to local instead of global, meaning it'll only reach across your camera's space

        //taking input from player (default is left mouse button, you can change this to your willings)
        if (Input.GetMouseButtonDown(0) && !Fired && !Hooked)
        {
            Fired = true; //identifying that it's been fired
            lastPos = MousePosition; //change the values of lastPos to the mouse position
            Anchor.transform.position = lastPos;
        }

        //lets now let the player to press right mouse button to replace the hook
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 endVelocity = Player.GetComponent<Rigidbody2D>().velocity;
            Fired = false;
            Hooked = false;
            Destroy(PlayerHingeJoint);
            Anchor.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            Player.GetComponent<Rigidbody2D>().isKinematic = true;
            Player.GetComponent<Player>().enabled = true;
            Player.transform.rotation = Quaternion.identity;
            Player.GetComponent<Player>().velocity = endVelocity;
        }

        //by default, the hook will be following the Player object if not fired and if not hooked
        if (!Fired && !Hooked)
        {
            transform.position = Vector3.MoveTowards(transform.position, Player.position, 1);
        }

        //if fired, then it will go to Shot method
        if (Fired)
        {
            //GetComponent<AudioSource>().clip = ShootSound; //playing the sound when shot
            transform.position = Vector3.MoveTowards(transform.position, lastPos, MovementSpeed); //move the hook towards lastPos
            DFH = Vector3.Distance(ThisObject.position, Player.position);
        }

        //Reach the limit or the click point
        if ((DFH > MD) || transform.position.Equals(lastPos))
        {
            //return the hook to hook holder
            Fired = false;
            Hooked = false;
        }

        //lets now initialize what will happen when hooked
        if (Hooked)
        {
            Fired = false; //lets once again define that Fired will be false, so it does not have any chances of bugging out

            //we also will call the RopeMovement function
            RopeMovement();

        }


    }

    //here we initialize when we want the hook to be colliding
    void OnCollisionEnter2D(Collision2D other)
    {
        print("You hit something!");
        //you can change the name of the object which comes with the asset named "HOOK_PLACE", but you'll have to change it here too
        //or if you want to use your own object, please rename this to whatever the object you're using is
        if (other.gameObject.CompareTag("Hookable"))
        {
            Hooked = true; //defining that the hook object is colliding with the HOOK_PLACE, meaning that the hook will stop moving
            Fired = false; //setting Fired to false so that hook will stop moving and stay where it is

            PlayerHingeJoint = Player.gameObject.AddComponent<HingeJoint2D>();
            PlayerHingeJoint.anchor = Player.transform.position;
            Anchor.transform.position = other.contacts[0].point;
            Anchor.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            PlayerHingeJoint.connectedBody = Anchor.GetComponent<Rigidbody2D>();
            PlayerHingeJoint.connectedAnchor = Vector3.zero;
            //Make player not kinematic.
            Player.GetComponent<Rigidbody2D>().isKinematic = false;
            //Disable Player script because it screws with stuff
            Player.GetComponent<Player>().enabled = false;
        }
        else
        {
            Hooked = false;
            Fired = false;
        }
    }

    void RopeMovement()
    {
        print("You're in RopeMovement");
        //first we'll initialize the rigidbody of player
        Rigidbody2D rig;
        rig = Player.GetComponent<Rigidbody2D>();

        //let's set up our movement while hooked
        if (Input.GetKey(KeyCode.A))
        {
            rig.AddForce(Vector3.left * Force);
        }

        if (Input.GetKey(KeyCode.D))
        {
            rig.AddForce(Vector3.right * Force);
        }
    }

}
