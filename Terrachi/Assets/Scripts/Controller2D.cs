using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour {
    
    //use this layermask to determine whether a layer is collidable
    public LayerMask collisionMask;

    //skin width inset for raycasting
    const float skinWidth = .015f; 

    //define how many rays are being fired horizontally and vertically:
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    //maximum slope player can climb
    float maxClimbAngle = 80;

    //define the spacing between each horizontal/vertical ray, depending on how many we've chosen to fire + size of the bounds
    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D collider; 
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions; //this is our public reference to collision info

	// Use this for initialization
	void Start () {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }
	
	// Update is called once per frame

    public void Move(Vector3 velocity) {
        UpdateRaycastOrigins();
        collisions.Reset(); //call reset everytime we move, ensuring we have a blank slate each time. 

        //if player is moving, check for collisions
        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }

        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }



        transform.Translate(velocity);
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        //get the direction of our y velocity:
        float directionX = Mathf.Sign(velocity.x); // moving up = +1, down -1
        //float for the length of our ray, .Abs so we can force it to be positive
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {

            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                //if we hit something, we must first check the angle of the surface we've hit:
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                //if this is the bottom-most ray (i == not)
                if (i == 0  && slopeAngle <= maxClimbAngle) {
                    float distanceToSlopeStart = 0;

                    //if we are beginning to climb a slope
                    if (slopeAngle != collisions.slopeAngleOld) {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                    
                    //print the current slope angle to the console
                    //print(slopeAngle);       
                }

                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    //then check the rest of the rays for collisions

                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    collisions.left = directionX == -1; //if we are moving left (-1) when we collide with something, set collisions.left bool to true. 
                    collisions.right = directionX == 1;

                }            
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity) {
        //get the direction of our y velocity:
        float directionY = Mathf.Sign(velocity.y); // moving up = +1, down -1
        //float for the length of our ray, .Abs so we can force it to be positive
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {

            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit) {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = directionY == -1; //if we are moving down (-1) when we collide with something, set collisions.l bool to true. 
                collisions.right = directionY == 1;
            }
        }
    }
    
    void ClimbSlope (ref Vector3 velocity, float slopeAngle) {

        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y <= climbVelocityY) {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
       
        





    }

    void UpdateRaycastOrigins() {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2); //shrinks bounds on all sides by skinWidth

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

    }

    void CalculateRaySpacing() {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2); //shrinks bounds on all sides by skinWidth

        //these lines ensure that atleast 2 rays are being fired in the horizontal & vertical directions
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        //calculate spacing between each ray
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);

    }

    //struct to store the four corners of our box collider (at all times).
    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo {
        public bool above, below;
        public bool left, right;
        public bool climbingSlope;

        public float slopeAngle, slopeAngleOld; //slopeAngleOld == slopeAngle we had in the previous frame
        //function to set all boolean values to false
        public void Reset() {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }


    }
    
}
