using UnityEngine;
using Pathfinding;
using System.Collections;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]

public class EnemyAI : MonoBehaviour {

    public bool running;

    private Animator myAnimator;

    //what to chase
    public Transform target;

    //how many times each second we update our path
    public float updateRate = 2f;

    //caching
    private Seeker seeker;
    private Rigidbody2D rb;

    //reference to calculated path
    public Path path;

    //The AI speed per second
    public float speed = 300f;
    public ForceMode2D fMode;

    [HideInInspector]
    public bool pathIsEnded = false;

    //the max distance from an AI to a waypoint for it
    //to continue to the next waypoint
    public float nextWaypointDistance = 3;

    public float aggroRange = 5;

    //the waypoint we are currently moving towards.
    private int currentWaypoint = 0;

    Vector2 startPos;

    void Awake()
    {
        //Vector2 startPos = transform.position;

    }


    void Start()
    {
        float seperation = Vector3.Distance(transform.position, target.position);

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        myAnimator = GetComponent<Animator>();

        if(target == null)
        {
            Debug.LogError("No Player Found? Panic");
            return;
        }

        //start a new path to the target position (player) from transform position (current position), and return result to the OnPathComplete method;
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(UpdatePath());


    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Obstacle")
        {
            speed *= -1;
        }
        if (col.gameObject.tag == "Player")
        {
            Player p = GameObject.Find("KodamaPlayer").GetComponent<Player>();
            p.Respawn();
        }

        if (running)
        {
            rb.gameObject.layer = 0;
        }

    }

    IEnumerator UpdatePath()
    {    
        //start a new path to the target position (player) from transform position (current position), and return result to the OnPathComplete method;
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        yield return new WaitForSeconds(1f / updateRate);
        StartCoroutine(UpdatePath());

    }

    IEnumerator PauseMovement()
    {
        running = true;
        target.tag = "Untagged";
        //Backup and clear velocities
        Vector2 linearBackup = rb.velocity;
        rb.velocity = Vector2.zero;

        //Finally freeze the body in place so forces like gravity or movement won't affect it
        rb.constraints = RigidbodyConstraints2D.FreezeAll;


        Color tmp = GameObject.Find("KodamaPlayer").GetComponent<SpriteRenderer>().color;
        tmp.a = .5f;
        GameObject.Find("KodamaPlayer").GetComponent<SpriteRenderer>().color = tmp;

        myAnimator.enabled = false;
        //Wait for a bit (two seconds)
        yield return new WaitForSeconds(3);
        myAnimator.enabled = true;


        Color reg = GameObject.Find("KodamaPlayer").GetComponent<SpriteRenderer>().color;
        reg.a = 1f;
        GameObject.Find("KodamaPlayer").GetComponent<SpriteRenderer>().color = reg;

        running = false;
        //And unfreeze before restoring velocities
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;

        //restore the velocities
        rb.velocity = linearBackup;
        target.tag = "Player";
    }

    //onPathComplete checks to see if there was an error.
    //if no path error, sets path variable to p and currentWaypoint to 0 (starts at 0)
    public void OnPathComplete(Path p)
    {
        Debug.Log("We got a path. Did it have an error?" + p.error);
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    //Like update but instead of updating for every frame drawn
    //Do Physics calculations here, not update
    void FixedUpdate()
    {
        float seperation = Vector3.Distance(transform.position, target.position);
        float velocity;

        if (target == null) {
            transform.Translate(new Vector3(speed, 0, 0) * Time.deltaTime);

            return;
        }

        //TODO: Always look at player? Some enemies (missles) point towards player

        if(path == null) {
            return;
        }

        //if we've reached our final waypoint. path is ended is tue
        if(currentWaypoint >= path.vectorPath.Count) {
            if (pathIsEnded) {
                return;
            }
     
            pathIsEnded = true;
            return;
        }

        pathIsEnded = false;

        if (seperation <= aggroRange)
        {
           
            //Direction to next waypoint:
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            dir *= speed * Time.fixedDeltaTime;

            //Move AI in direction
            rb.AddForce(dir, fMode);

            velocity = rb.velocity.x;

            myAnimator.SetFloat("animSpeed", Mathf.Abs(velocity));

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(PauseMovement());
            }

            float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            //if close enough to next waypoint, follow waypoint
            if (dist < nextWaypointDistance)
            {
                currentWaypoint++;
                return;
            }

        }
        else
        { 
            //transform.position = startPos;
            myAnimator.SetFloat("animSpeed", 0);
            //Debug.Log("START: " + startPos);
            //Debug.Log("you are here");
            //myAnimator.CrossFade("EnemyIdle", 1);
            //myAnimator.Play("EnemyIdle");
            //myAnimator.speed = 0.05f;
        }
        
    }

}
