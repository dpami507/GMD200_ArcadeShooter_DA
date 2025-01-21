using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    [Header("Camera Follow")]
    public Transform target;
    public float speedPos;
    public float speedRot;
    Vector3 vel = Vector2.zero;
    public Vector3 offset;
    public float velocityMultiplier;

    [Header("Caemra Shake")]
    public float rotShake;
    public float moveShake;

    Rigidbody2D playerRb;

    private void Start()
    {
        playerRb = target.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Shake(1);
        }
    }

    private void FixedUpdate()
    {
        //Add the players y velocity as to see more
        Vector3 velPos = new Vector3(0, playerRb.velocity.y * velocityMultiplier, 0);
        Vector3 desiredPos = target.position + offset + velPos;

        float smoothTime = 1 / (speedPos);

        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref vel, smoothTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, speedRot * Time.deltaTime);
    }

    public void Shake(float intensity)
    {
        //Pos
        float sideX = (Random.Range(0, 2) == 0) ? 1 : -1;
        float sideY = (Random.Range(0, 2) == 0) ? 1 : -1;

        float move = moveShake * intensity;
        move = Mathf.Min(move, 10); //Keep position within 5
        Vector3 newPos = new Vector3(move * sideX, move * sideY, 0) + transform.position;
        transform.position = newPos;

        //Rot
        int rotZ = (Random.Range(0, 2) == 0) ? 1 : -1;
        float angle = rotShake * intensity;
        angle = Mathf.Min(angle, 25); //Keep rot below 45
        Quaternion newRot = Quaternion.Euler(0, 0, angle * rotZ);
        transform.rotation = newRot;
    }
}
