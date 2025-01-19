using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public Transform muzzle;
    public GameObject bullet;
    public float bulletSpeed;
    public float bulletSize;

    public float shotsPerSecond;
    float lastShot;

    private void Update()
    {
        //Shooting Logic
        lastShot += Time.deltaTime;
        if(Input.GetButton("Fire1") && lastShot > (1 / shotsPerSecond) && FindFirstObjectByType<GameManager>().gameStarted)
        {
            Fire();
            lastShot = 0;
        }
    }

    //Shoot
    void Fire()
    {
        FindFirstObjectByType<SoundManager>().PlaySound("Shoot");
        GameObject bullet_ = Instantiate(bullet, muzzle.position, muzzle.rotation);
        bullet_.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
        bullet_.GetComponent<Rigidbody2D>().velocity = bullet_.transform.up * bulletSpeed;
    }
}
