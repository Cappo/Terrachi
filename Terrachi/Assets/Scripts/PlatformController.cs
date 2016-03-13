using UnityEngine;
using System.Collections;
using System.Collections.Generic; //gives us HashSet

public class PlatformController : RaycastController {

	public LayerMask passengerMask;

	public Vector3[] localWaypoints;
	Vector3[] globalWaypoints;

	public float speed;
	public bool cyclic;
	public float waitTime;
	[Range(0,2)]
	public float easeAmount;

	int fromWaypointIndex;
	float percentBetweenWaypoints;
	float nextMoveTime;

	List<PassengerMovement> passengerMovement; //List to store PassengerMovement struct info each time we calculate passenger movement.
    //By using a dictionary we reduce the number of GetComponent<> calls. 
	Dictionary<Transform,Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>(); //takes in transform as its key, controller2D as its value
	
	public override void Start () {
		base.Start (); //calls the start function from RayCast controller

		globalWaypoints = new Vector3[localWaypoints.Length];
		for (int i =0; i < localWaypoints.Length; i++) {
			globalWaypoints[i] = localWaypoints[i] + transform.position;
		}
	}

	void Update () {

		UpdateRaycastOrigins ();

		Vector3 velocity = CalculatePlatformMovement();

		CalculatePassengerMovement(velocity);

		MovePassengers (true);
		transform.Translate (velocity);
		MovePassengers (false);
	}

	float Ease(float x) {
		float a = easeAmount + 1;
		return Mathf.Pow(x,a) / (Mathf.Pow(x,a) + Mathf.Pow(1-x,a));
	}
	
	Vector3 CalculatePlatformMovement() {

		if (Time.time < nextMoveTime) {
			return Vector3.zero;
		}

		fromWaypointIndex %= globalWaypoints.Length;
		int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
		float distanceBetweenWaypoints = Vector3.Distance (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex]);
		percentBetweenWaypoints += Time.deltaTime * speed/distanceBetweenWaypoints;
		percentBetweenWaypoints = Mathf.Clamp01 (percentBetweenWaypoints);
		float easedPercentBetweenWaypoints = Ease (percentBetweenWaypoints);

		Vector3 newPos = Vector3.Lerp (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex], easedPercentBetweenWaypoints);

		if (percentBetweenWaypoints >= 1) {
			percentBetweenWaypoints = 0;
			fromWaypointIndex ++;

			if (!cyclic) {
				if (fromWaypointIndex >= globalWaypoints.Length-1) {
					fromWaypointIndex = 0;
					System.Array.Reverse(globalWaypoints);
				}
			}
			nextMoveTime = Time.time + waitTime;
		}

		return newPos - transform.position;
	}

    //Passengers refer to anything being moved by the platform.
    void MovePassengers(bool beforeMovePlatform) {
		foreach (PassengerMovement passenger in passengerMovement) {
            // If passenger is not already contained in our dictionary
			if (!passengerDictionary.ContainsKey(passenger.transform)) {
				passengerDictionary.Add(passenger.transform,passenger.transform.GetComponent<Controller2D>()); //add passenger to the dictionary, should help ensure only one GetComponent<> call per passenger
			}

            //if we want to move the player before we move the platform (beforeMovePlatform == true) 
			if (passenger.moveBeforePlatform == beforeMovePlatform) {
				passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
			}
		}
	}

	void CalculatePassengerMovement(Vector3 velocity) {
  
        HashSet<Transform> movedPassengers = new HashSet<Transform> (); //It's possible that there is more than one passenger interacting with the platform (i.e. enemies), so we must store all the passengers (transforms) we've already moved this frame.
		passengerMovement = new List<PassengerMovement> (); 

        //get the X and Y directions are platform is moving
        float directionX = Mathf.Sign (velocity.x);
		float directionY = Mathf.Sign (velocity.y);

        // CASE 1: Vertically moving platform
        // If platforms moving up, rays are cast upwards: meaning if it hits a passenger then that means that the passenger is standing on the platform
        // If platform is moving down, rays are cast downwards: meaning if it hits a passenger then that means the passenger is below the platform
		if (velocity.y != 0) {
			float rayLength = Mathf.Abs (velocity.y) + skinWidth;

            //*Note*: Since we are casting multiple rays out, a single passenger could be hit multiple times by a single frame
			for (int i = 0; i < verticalRayCount; i ++) {
				Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                //if a passenger is found
                if (hit && hit.distance != 0) {
                    //each time we hit something, if the movedPassengers Hashset does not contain the hit.transform, only then we will actually move that transform.  Once we've moved it, we will add it to the HashSet. (Prevents passenger from being moved more than once per frame)
					if (!movedPassengers.Contains(hit.transform)) {
						movedPassengers.Add(hit.transform); 

                        //Passenger should only be affected by the x velocity if that passenger is actually standing on the platform, if the passenger is below the platform we don't want him to be affected by X velocity
						float pushX = (directionY == 1)?velocity.x:0; //if platform moving up, then pushX can be velocity.x. Otherwise set it == to 0
                        
                        //This closes the gap between the platform and the passenger, and only moves the platform by the rest of the velocity
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY; //hit.distance is the distance between the platform and the passenger, we subtract skinWidth because rays are being cast from skinWidth inside of the collider. 
                        //add a new passengerMovement to the passenger movement list
                        passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY), directionY == 1, true)); //standingOnPlatform can be true if directionY = true (meaning rays are being cast up.), and we've already detected a hit
                        
					}
				}
			}
		}

		//CASE 2: Horizontally moving platform
		if (velocity.x != 0) {
			float rayLength = Mathf.Abs (velocity.x) + skinWidth;
			
			for (int i = 0; i < horizontalRayCount; i ++) {
				Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
				rayOrigin += Vector2.up * (horizontalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

				if (hit && hit.distance != 0) {
					if (!movedPassengers.Contains(hit.transform)) {
						movedPassengers.Add(hit.transform);
						float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
						float pushY = -skinWidth;
						
						passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY), false, true)); //here passenger is being pushed from the side, so its impossible for standingOnPLatform to be true. 
					}
				}
			}
		}

		//CASE 3: Passenger on top of a horizontally or downward moving platform
		if (directionY == -1 || velocity.y == 0 && velocity.x != 0) { 
			float rayLength = skinWidth * 2; //casts small rays (just barely longer than skinWidth) above the platform in order to detect someone atop it.
			
			for (int i = 0; i < verticalRayCount; i ++) {
				Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask); //notice here Vector2.up not multiplied by directionY, this is because we always want the way to be case up in this scenario.
				
				if (hit && hit.distance != 0) {
					if (!movedPassengers.Contains(hit.transform)) {
						movedPassengers.Add(hit.transform);
						float pushX = velocity.x;
						float pushY = velocity.y;
						
						passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY), true, false));
					}
				}
			}
		}
	}

	struct PassengerMovement {
		public Transform transform; //transform of the passenger
		public Vector3 velocity; //desired velocity of the passenger
		public bool standingOnPlatform;
		public bool moveBeforePlatform; //whether or not we must move the passenger before the platform is moved

		public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform) {
			transform = _transform;
			velocity = _velocity;
			standingOnPlatform = _standingOnPlatform;
			moveBeforePlatform = _moveBeforePlatform;
		}
	}

	void OnDrawGizmos() {
		if (localWaypoints != null) {
			Gizmos.color = Color.red;
			float size = .3f;

			for (int i =0; i < localWaypoints.Length; i ++) {
				Vector3 globalWaypointPos = (Application.isPlaying)?globalWaypoints[i] : localWaypoints[i] + transform.position;
				Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
				Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
			}
		}
	}
	
}
