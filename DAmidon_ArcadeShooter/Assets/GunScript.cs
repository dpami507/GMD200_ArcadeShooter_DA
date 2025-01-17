using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public Transform muzzle;
    public GameObject bullet;
    public float bulletSpeed;

    public float shotsPerSecond;
    float lastShot;

    private void Update()
    {
        lastShot += Time.deltaTime;
        if(Input.GetButton("Fire1") && lastShot > (1 / shotsPerSecond))
        {
            Fire();
            lastShot = 0;
        }
    }

    void Fire()
    {
        GameObject bullet_ = Instantiate(bullet, muzzle.position, muzzle.rotation);
        bullet_.GetComponent<Rigidbody2D>().velocity = bullet_.transform.up * bulletSpeed;
    }
}
