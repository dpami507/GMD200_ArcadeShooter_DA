using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script that shoots bullets for the player
public class GunScript : MonoBehaviour
{
    [SerializeField] Transform muzzle;
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletSpeed;
    [SerializeField] float bulletSize;

    [SerializeField] float shotsPerSecond;
    float lastShot;

    private void Update()
    {
        //Shooting Logic
        lastShot += Time.deltaTime;
        if(Input.GetButton("Fire1") && lastShot > (1 / shotsPerSecond) && GameManager.instance.gameStarted)
        {
            Fire();
            lastShot = 0;
        }
    }

    //Shoot
    void Fire()
    {
        SoundManager.instance.PlaySound("Shoot");
        GameObject bullet_ = Instantiate(bullet, muzzle.position, muzzle.rotation);
        bullet_.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
        bullet_.GetComponent<Rigidbody2D>().velocity = bullet_.transform.up * bulletSpeed;
    }
}
