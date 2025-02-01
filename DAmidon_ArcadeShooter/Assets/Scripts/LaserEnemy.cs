using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemy : EnemyBaseScript
{
    [Header("Laser")]
    [SerializeField] bool firing;
    [SerializeField] int laserDamage;
    [SerializeField] int laserDuration;
    [SerializeField] float laserCooldown;
    [SerializeField] LineRenderer laserRender;
    [SerializeField] LineRenderer laserLOS;
    [SerializeField] ParticleSystem hitParticle;
    [SerializeField] LayerMask laserHit;
    AudioSource laserSound;
    float lastFired;
    float lastLsrDmg;
    float lsrDmgCooldown = 0.2f;
    int laserDist = 100;

    private void Start()
    {
        target = FindFirstObjectByType<PlayerManager>().transform;
        health = GetComponent<Health>();

        laserRender.enabled = false;
        hitParticle.Stop();
        laserSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, GetRotation(0), rotSpeed * Time.deltaTime);

        laserLOS.SetPosition(0, muzzle.position);
        laserLOS.SetPosition(1, muzzle.position + muzzle.right * laserDist);

        if (health.currentHealth <= 0 || GameManager.instance.dead)
            Die();

        lastFired += Time.deltaTime;
        if(lastFired >= laserCooldown && !firing) 
            StartCoroutine(StartLaser());

        if(firing) 
            Fire();
    }

    void Fire()
    {
        RaycastHit2D hit = Physics2D.Raycast(muzzle.position, muzzle.right, laserDist, laserHit);

        lastLsrDmg += Time.deltaTime; //Stops damage from being taken every frame
        if (hit && hit.transform.GetComponent<Health>() && lastLsrDmg > lsrDmgCooldown)
        {
            FindFirstObjectByType<CameraFollowScript>().Shake();
            hit.transform.GetComponent<Health>().TakeDamage(laserDamage);
            lastLsrDmg = 0;
        }

        Vector2 point;
        if (!hit)
            point = muzzle.position + muzzle.right * laserDist;
        else
            point = hit.point;

        hitParticle.transform.position = point;
        laserRender.SetPosition(0, muzzle.position);
        laserRender.SetPosition(1, point);
    }

    IEnumerator StartLaser()
    {
        laserSound.volume *= SoundManager.instance.soundSlider.value;
        laserSound.Play();
        laserRender.enabled = true;
        hitParticle.Play();

        firing = true;
        yield return new WaitForSeconds(laserDuration);
        firing = false;
        lastFired = 0;

        laserSound.Stop();
        laserRender.enabled = false;
        hitParticle.Stop();
    }
}
