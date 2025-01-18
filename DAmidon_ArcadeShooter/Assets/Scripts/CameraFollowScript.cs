using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    public Transform target;
    public float speed;
    Vector3 vel = Vector2.zero;
    public Vector3 offset;
    public float velocityMultiplier;

    Rigidbody2D playerRb;

    private void Start()
    {
        playerRb = target.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector3 velPos = new Vector3(0, playerRb.velocity.y * velocityMultiplier, 0);
        Vector3 desiredPos = target.position + offset + velPos;

        float smoothTime = 1 / (speed);

        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref vel, smoothTime);
    }
}
