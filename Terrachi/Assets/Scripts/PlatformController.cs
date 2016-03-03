using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : RayCastController
{
    public LayerMask passengerMask;
    public Vector3 move;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRaycastOrigins();
        

        Vector3 velocity = move * Time.deltaTime;
        MovePassengers(velocity);
        transform.Translate(velocity);
    }

    //Passengers refer to any Controller2D, anything being moved by the platform.
    void MovePassengers(Vector3 velocity)
    {
        //This hash set of transforms stores all the passengers we've already moved this frame.
        

        //get the X and Y directions are platform is moving
        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        //Vertically Moving Platform (Up or down)
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            //*Note*: Since we are casting multiple rays out, a single passenger could be hit multiple times by a single frame
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);
           
                //if a passenger is found
                if (hit) {
                    //passenger should only be affected by push X if its standing atop the platform
                    
                    float pushX = (directionY == 1) ? velocity.x : 0; //if platform moving up, then pushX can = velocity.x otherwise set it equal to 0.
                    float pushY = velocity.y - (hit.distance - skinWidth) * directionY; //close the gap between the passenger and the platform

                    hit.transform.Translate(new Vector3(pushX, pushY));
                }

            }
        }
    }

} //end Script
