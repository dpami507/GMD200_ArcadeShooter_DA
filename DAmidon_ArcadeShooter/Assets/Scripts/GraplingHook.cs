using UnityEngine;

//Starts and Stops Grappling Hook and sets visuals and spring
public class GraplingHook : MonoBehaviour
{
    [SerializeField] Transform point;
    [SerializeField] Transform closestPoint;
    [SerializeField] float searchDist;

    public bool isGrappling;
    LineRenderer lineRenderer;
    SpringJoint2D springJoint;

    [SerializeField] LayerMask points;

    Rigidbody2D playerRb;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        springJoint = GetComponent<SpringJoint2D>();
        lineRenderer = GetComponent<LineRenderer>();

        StopGrapple();
        closestPoint = null;
    }

    private void Update()
    {
        //Stop if Game Not Running
        if (GameManager.instance.dead || !GameManager.instance.gameStarted)
        {
            StopGrapple();
            closestPoint = null;

            return;
        }

        if(!isGrappling)
        {
            SetClosest();

            if (closestPoint == null)
                point.gameObject.SetActive(false);
            else point.gameObject.SetActive(true);
        }

        if (closestPoint) //Set visual to closest point to mouse
            point.position = closestPoint.position;

        if (Input.GetButton("Fire2") && closestPoint)
        {
            StartGrapple();
        }
        else //No grapple
        {
            StopGrapple();
        }
    }

    void StartGrapple()
    {
        //Play sound
        if (!isGrappling)
            SoundManager.instance.PlaySound("Grapple");

        isGrappling = true;

        //Set Point
        Vector3 closestPos = closestPoint.position;

        //Start Spring and Line Render and set it up
        springJoint.enabled = true;
        lineRenderer.enabled = true;

        springJoint.connectedAnchor = closestPos;
        springJoint.distance = Vector2.Distance(transform.position, closestPos) * .6f;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, closestPos);

        point.gameObject.SetActive(true);
    }

    void StopGrapple()
    {
        //Disable all grapple stuff
        springJoint.enabled = false;
        lineRenderer.enabled = false;
        isGrappling = false;
    }

    //Get closest point to grapple
    void SetClosest()
    {
        //Set noMouse
        Vector3 vel = playerRb.velocity.normalized;

        //Change point of interest based on if player has a mouse
        Vector2 searchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(searchPos, searchDist, points);

        if (colliders.Length <= 0)
        {
            closestPoint = null;
        }

        foreach (Collider2D collider in colliders)
        {
            if (!collider.CompareTag("point")) return;

            if (closestPoint == null)
            {
                closestPoint = collider.transform;
            }
            else
            {
                float pointDist = Vector2.Distance(collider.transform.position, searchPos);
                float currentDist = Vector2.Distance(closestPoint.position, searchPos);

                if (pointDist < currentDist)
                {
                    closestPoint = collider.transform;
                }
            }
        }
    }
}
