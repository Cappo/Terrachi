//Grappling Hook, based on the open source Grappling Hook 2D asset by Sykoo

using UnityEngine;
using System.Collections;

public class GrapplingHook : MonoBehaviour
{

    public Transform Player; //the object for player
    public Transform Anchor; //Anchor point
    public Transform ThisObject; //defining this object itself
    public Transform Vine; //The vine sprite object

    private HingeJoint2D PlayerHingeJoint; //the HingeJoint attached to the player object

    public bool Fired = false; //checking if the hook has been fired
    public bool Hooked = false; //checking if something has been hooked

    public float MovementSpeed = 0.2f; //movement speed for the hook object to fly to its destination
    public int MD = 10; //setting a limit to maximum distance, MD stands for "Max Distance"
    public float DFH; //calculating distance from hook holder, DFH stands for "Distance From HookHolder"
    public float ClimbRate = .1F; //Rate at which the player can climb or lower on the grappling hook

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
            exitRope();
        }

        //by default, the hook will be following the Player object if not fired and if not hooked
        if (!Fired && !Hooked)
        {
            transform.position = Vector3.MoveTowards(transform.position, Player.position, 1);
            Vine.transform.position = Vector3.MoveTowards(transform.position, Player.position, 1);
        }

        //if fired, then it will go to Shot method
        if (Fired)
        {
            //GetComponent<AudioSource>().clip = ShootSound; //playing the sound when shot
            transform.position = Vector3.MoveTowards(transform.position, lastPos, MovementSpeed); //move the hook towards lastPos

            //Deal with the vine sprite
            updateVineSprite();

            DFH = Vector2.Distance(ThisObject.position, Player.position);
        }

        //Reach the limit or the click point
        if ((DFH > MD) || transform.position.Equals(lastPos))
        {
            //return the hook to hook holder
            Fired = false;
            Hooked = false;

            resetSprite();
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

            //Save the initial velocity
            Vector2 initial_velocity = Player.GetComponent<Player>().velocity;
            Anchor.transform.position = other.contacts[0].point;
            Anchor.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            //Try to let the player grapple even when they're standing
            if (Player.GetComponent<Controller2D>().collisions.below)
            {
                Player.transform.Translate(Vector3.up * .2F);
            }
            //Fix the rotation
            //Get the anchor point we'll use later
            Vector3 anchor_point = Anchor.transform.position - Player.transform.position;
            float rotation_correction = Mathf.Rad2Deg * Mathf.Atan(anchor_point.x / anchor_point.y) * -1;
            Player.transform.Rotate(new Vector3(0, 0, rotation_correction));
            anchor_point.y = Vector2.Distance(Vector3.zero, anchor_point);
            anchor_point.x = 0;
            anchor_point.z = 0;
            PlayerHingeJoint = Player.gameObject.AddComponent<HingeJoint2D>();
            anchor_point.x /= Player.transform.localScale.x;
            anchor_point.y /= Player.transform.localScale.y;
            anchor_point.z /= Player.transform.localScale.z;
            PlayerHingeJoint.anchor = anchor_point;
            PlayerHingeJoint.connectedBody = Anchor.GetComponent<Rigidbody2D>();
            PlayerHingeJoint.connectedAnchor = Vector3.zero;
            Player.GetComponent<Rigidbody2D>().isKinematic = false;
            //Disable Player script because it screws with stuff
            Player.GetComponent<Player>().enabled = false;
            //Let's manually set collisions.below because it's acting screwy
            Player.GetComponent<Controller2D>().collisions.below = false;
            //print("Collisions below set");
            //Let's try to maintain some velocity
            Player.GetComponent<Rigidbody2D>().velocity = initial_velocity;
        }
        else
        {
            Hooked = false;
            Fired = false;
        }
    }

    void RopeMovement()
    {
        //Update the vine sprite
        updateVineSprite();

        //first we'll initialize the rigidbody of player
        Rigidbody2D rig;
        rig = Player.GetComponent<Rigidbody2D>();

        float horizontal_input = Input.GetAxisRaw("Horizontal");
        float vertical_input = Input.GetAxisRaw("Vertical");

        //Update the collisions
        controller.Invoke("UpdateRaycastOrigins", 0);
        controller.Invoke("VerticalCollisions", 0);

        //let's set up our movement while hooked
        if (horizontal_input < 0)
        {
            rig.AddForce(Vector3.left * Force * horizontal_input * -1);
        }

        if (horizontal_input > 0)
        {
            rig.AddForce(Vector3.right * Force * horizontal_input);
        }

        //Can we vary the rope length?
        Vector2 anchor_point = Player.GetComponent<HingeJoint2D>().anchor;
        if (vertical_input < 0)
        {
            //We should probably have a limit on how long this can be...
            if (anchor_point.y * Player.transform.localScale.y < MD)
            {
                anchor_point.y += ClimbRate;
                Player.GetComponent<HingeJoint2D>().anchor = anchor_point;
            }
            //Check if we're lowering through the floor, which is obviously not allowed
            //print(controller.collisions.below);
            if (Player.GetComponent<Controller2D>().collisions.below)
            {
                exitRope();
            }
        }
        if (vertical_input > 0)
        {
            if (anchor_point.y * Player.transform.localScale.y > 2F) //If we get too close to the anchor, weird things happen.
            {
                anchor_point.y -= ClimbRate;
                Player.GetComponent<HingeJoint2D>().anchor = anchor_point;
            }
        }

        //Jump off of the grappling hook
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 boost = new Vector2(0, 15);
            if (horizontal_input < 0)
            {
                boost.x -= 12F;
            }
            else if (horizontal_input > 0)
            {
                boost.x += 12F;
            }
            Player.GetComponent<Rigidbody2D>().velocity += boost;
            exitRope();
        }
    }

    void exitRope()
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
        resetSprite();
    }

    void resetSprite()
    {
        //Reset the sprite
        Vine.transform.position = Player.transform.position;
        Vine.transform.rotation = Quaternion.identity;
        Vine.transform.localScale = new Vector3(.5F, 0F, .5F);
    }

    void updateVineSprite()
    {
        Vector3 scale = Vine.transform.localScale;
        scale.y = ((Vector2.Distance(transform.position, Player.transform.position) / MD) * .55F);
        Vine.transform.localScale = scale;
        Vector3 pos = (transform.position + Player.transform.position) / 2;
        Vine.transform.position = pos;
        Vector3 rotation_vector = transform.position - Player.transform.position;
        rotation_vector = new Vector3(-rotation_vector.y, rotation_vector.x, rotation_vector.z); //Rotate 90 degrees counter-clockwise
        float rotation_offset = Mathf.Rad2Deg * Mathf.Atan(rotation_vector.y / rotation_vector.x);
        Vine.transform.Rotate(new Vector3(0, 0, rotation_offset - Vine.transform.rotation.eulerAngles.z));
    }
}