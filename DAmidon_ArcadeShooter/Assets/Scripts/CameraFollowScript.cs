using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Camera follow script that zooms out based on speed, smoothly follows the player, and trails ahead when fast
public class CameraFollowScript : MonoBehaviour
{
    [Header("Camera Follow")]
    [SerializeField] Transform target;
    [SerializeField] float speedPos;
    [SerializeField] float speedRot;
    [SerializeField] Vector3 offset;
    [SerializeField] float velocityMultiplier;

    [Header("Caemra Shake")]
    [SerializeField] float rotShake;
    [SerializeField] float moveShake;

    [Header("Caemra Zoom")]
    [SerializeField] int defaultCamZoom;
    [SerializeField] float camZoomMultiplyer;
    [SerializeField] float camZoomSpeed;

    Vector3 vel = Vector2.zero;
    float velFloat = 0;
    Rigidbody2D playerRb;
    Camera thisCam;

    private void Start()
    {
        playerRb = target.GetComponent<Rigidbody2D>();
        thisCam = transform.GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        //Add the players y velocity as to see more
        Vector3 velPos = new Vector3(0, playerRb.velocity.y * velocityMultiplier, 0);
        Vector3 desiredPos = target.position + offset + velPos;

        //Zoom out with speed;
        float desiredZoom = defaultCamZoom + (playerRb.velocity.magnitude * camZoomMultiplyer);
        thisCam.orthographicSize = Mathf.SmoothDamp(thisCam.orthographicSize, desiredZoom, ref velFloat, camZoomSpeed);

        //Smoothly translate and rotate camera to desired setting
        float smoothTime = 1 / (speedPos);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref vel, smoothTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, speedRot * Time.deltaTime);
    }

    //Camera Shake
    public void Shake(float intensity = 1)
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
