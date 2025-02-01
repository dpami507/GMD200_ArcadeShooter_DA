using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Shoots rockets at player
public class RocketLauncher : EnemyBaseScript
{
    [Header("Firing")]
    [SerializeField] GameObject rocket;
    [SerializeField] float chargeTime;
    [SerializeField] float shotsPerSecond;
    float lastShot;
    [SerializeField] ParticleSystem chargePart;
    [SerializeField] ParticleSystem firePart;
    [SerializeField] LayerMask blocksLOS;

    LineRenderer lineLOS;
    bool firing;

    private void Start()
    {
        health = GetComponent<Health>();
        lineLOS = GetComponent<LineRenderer>();
        chargePart.Stop();
        firePart.Stop();
        firing = false;
        target = FindObjectOfType<PlayerManager>().transform;
        lastShot = 1 / shotsPerSecond;
    }

    private void Update()
    {
        //If has LOS set the LR and bool
        if(HasLOS())
        {
            lineLOS.enabled = true;
            lineLOS.SetPosition(0, transform.position);
            lineLOS.SetPosition(1, target.position);
        }
        else lineLOS.enabled = false;

        //Die
        if (health.currentHealth <= 0 || GameManager.instance.dead)
            Die();

        //Rotate to Target
        transform.rotation = Quaternion.Lerp(transform.rotation, GetRotation(180), rotSpeed * Time.deltaTime);

        //Shooting Logic
        lastShot += Time.deltaTime;
        if (lastShot > (1 / shotsPerSecond) && !firing && HasLOS())
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
        SoundManager.instance.PlaySound("RocketLaunch");
        GameObject ro = Instantiate(rocket, muzzle.position, muzzle.rotation);
        ro.GetComponent<RocketScript>().launcher = this.gameObject;
        firing = false;
    }

    bool HasLOS()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, blocksLOS);
        
        if(hit) //Will only hit layers that block it so if it gets anything there is no LOS :(
            return false;

        return true;
    }
}
