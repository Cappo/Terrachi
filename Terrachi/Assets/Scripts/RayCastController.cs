using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class RaycastController : MonoBehaviour {

    //assign a layer to this collision mask to make it collideable
    public LayerMask collisionMask;

    /* Skinwidth Explanation: Rays are cast from a small width inside the bounds of the collider. (Referred to as the skinWidth) The reason for this is so that when the player is resting on the ground,
    there is still a small amount of space within the collider where which we can effectively fire the rays. */
    public const float skinWidth = .015f; 
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

    //define the spacing between each horizontal/vertical ray, depending on how many we've chosen to fire + size of the bounds
    [HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;

	[HideInInspector]
	public BoxCollider2D collider;
	public RaycastOrigins raycastOrigins;

	public virtual void Awake() {
		collider = GetComponent<BoxCollider2D> ();
	}

	public virtual void Start()//allows us to override (and extend this start method by calling base.start in other scripts)
    { 
        
        CalculateRaySpacing ();
	}

	public void UpdateRaycastOrigins() {
		Bounds bounds = collider.bounds; //get the bounds of our boxCollider2D and store it. 
		bounds.Expand (skinWidth * -2); //This actually shrinks bounds on all sides so the collider is inset by skinWidth


        //Assign vector2s to there respective corners.
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y); 
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}
	
	public void CalculateRaySpacing() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

        //These 2 lines ensure that atleast 2 (the min value) rays are being fired in the horizontal & vertical directions
        //Clamping more or less means constraining a value between a min and max.
        horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);
		
        //calculate spacing between each ray.
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1); 
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}
	
    // Stores all the corners of the BoxCollider as Vector2s.
	public struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
}
