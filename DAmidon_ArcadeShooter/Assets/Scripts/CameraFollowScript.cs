using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    public Transform target;
    public float speed;
    Vector3 vel = Vector2.zero;
    public Vector3 offset;

    private void FixedUpdate()
    {
        Vector3 desiredPos = target.position + offset;

        float smoothTime = 1 / (speed);

        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref vel, smoothTime);
    }
}
