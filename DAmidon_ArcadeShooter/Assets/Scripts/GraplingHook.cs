using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GraplingHook : MonoBehaviour
{
    public Transform point;
    [SerializeField] Transform closestPoint;
    public float searchDist;

    public bool isGrappling;
    LineRenderer lineRenderer;
    SpringJoint2D springJoint;

    public LayerMask points;
    GameManager manager;

    private void Start()
    {
        manager = FindFirstObjectByType<GameManager>();

        springJoint = GetComponent<SpringJoint2D>();
        lineRenderer = GetComponent<LineRenderer>();

        springJoint.enabled = false;
        lineRenderer.enabled = false;

        closestPoint = null;
        isGrappling = false;
    }

    private void Update()
    {
        if(manager.dead || !manager.gameStarted)
        {
            springJoint.enabled = false;
            lineRenderer.enabled = false;

            closestPoint = null;
            isGrappling = false;

            return;
        }

        if(!isGrappling)
        {
            if (closestPoint == null)
                point.gameObject.SetActive(false);
            else
                point.gameObject.SetActive(true);

            SetClosest();
        }

        //If click and there is a point Grapple
        if (Input.GetMouseButton(1) && closestPoint)
        {
            if(!isGrappling)
                FindFirstObjectByType<SoundManager>().PlaySound("Grapple");

            springJoint.enabled = true;
            lineRenderer.enabled = true;
            isGrappling = true;

            springJoint.connectedAnchor = closestPoint.position;
            springJoint.distance = Vector2.Distance(transform.position, closestPoint.position) * .6f;

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, closestPoint.position);
        }
        else //No grapple
        {
            springJoint.enabled = false;
            lineRenderer.enabled = false;
            isGrappling = false;
        }
    }

    //Get closest point to grapple
    void SetClosest()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePos, searchDist, points);

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
                float pointDist = Vector2.Distance(collider.transform.position, mousePos);
                float currentDist = Vector2.Distance(closestPoint.position, mousePos);

                if (pointDist < currentDist)
                {
                    closestPoint = collider.transform;
                }
            }
        }

        if(closestPoint)
            point.position = closestPoint.position;
    }
}
