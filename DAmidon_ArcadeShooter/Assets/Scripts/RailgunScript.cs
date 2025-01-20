using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RailgunScript : EnemyScript
{
    [Header("Firing")]
    public GameObject rocket;
    public float chargeTime;
    public ParticleSystem chargePart;
    public ParticleSystem firePart;
    bool firing;

    private void Start()
    {
        health = GetComponent<Health>();

        chargePart.Stop();
        firePart.Stop();
        firing = false;
        target = FindObjectOfType<PlayerManager>().transform;
        lastShot = 1 / shotsPerSecond;
    }

    private void Update()
    {
        //Kill
        if (health.currentHealth <= 0)
            Die();

        //Rotate to Target
        transform.rotation = Quaternion.Lerp(transform.rotation, GetRotation(180), rotSpeed * Time.deltaTime);

        //Shooting Logic
        lastShot += Time.deltaTime;
        if (CheckIfCanFire() && lastShot > (1 / shotsPerSecond) && !firing)
        {
            lastShot = 0;
            StartCoroutine(StartFire());
        }
    }

    IEnumerator StartFire()
    {
        firing = true;
        chargePart.Play(); //Play Charge
        yield return new WaitForSeconds(chargeTime); //Wait
        chargePart.Stop(); //Stop Charge
        Fire(); //Fire
    }

    void Fire()
    {
        //Fire Rocket
        firePart.Play();
        FindFirstObjectByType<SoundManager>().PlaySound("Shoot");
        GameObject ro = Instantiate(rocket, muzzle.position, muzzle.rotation);
        ro.GetComponent<RocketScript>().launcher = this;
        firing = false;
    }
}
