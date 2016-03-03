using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class RayCastController : MonoBehaviour {
    //use this layermask to determine whether a layer is collidable
    public LayerMask collisionMask;

    //skin width inset for raycasting
    public const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    //define the spacing between each horizontal/vertical ray, depending on how many we've chosen to fire + size of the bounds
    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    public BoxCollider2D collider;
    public RaycastOrigins raycastOrigins;

    public virtual void Start() //allows us to override (and extend this start method by calling base.start)
    {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2); //shrinks bounds on all sides by skinWidth

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
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
    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
