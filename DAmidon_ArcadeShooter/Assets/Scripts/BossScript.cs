using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script for Boss that is spawned every 2500 points. This boss can fire lasers and shoot rockets at the player
public class BossScript : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] int scoreWorth;

    [Header("Movement")]
    [SerializeField] Transform target;
    [SerializeField] float speed;
    [SerializeField] float lerpSpeed;
    [SerializeField] float distToMove;

    [Header("Rotation")]
    [SerializeField] float rotationAngleOffset;
    [SerializeField] float lerpRotSpeed;

    [Header("Lasers")]
    [SerializeField] LayerMask laserMask;
    [SerializeField] int laserDamage;
    [SerializeField] Laser[] lasers;
    float laserDmgCooldown = 0.05f;
    float lastLsrDmg;

    [Header("Rockets")]
    [SerializeField] GameObject rocket;
    [SerializeField] Transform launchPoint;
    [SerializeField] int rocketCount;
    float rocketCooldown;
    float lastRocket;
    int rocketsLaunched;

    [Header("Attack Cycles")]
    [SerializeField] float laserTime;
    [SerializeField] float rocketTime;
    [SerializeField] float cooldown;
    [SerializeField] bool[] phases;
    [SerializeField] int attackIndex;

    Health health;
    Rigidbody2D rb;
    float angle;

    private void Start()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        target = FindFirstObjectByType<PlayerManager>().transform;

        rocketCooldown = rocketTime / rocketCount;

        for (int i = 0; i < phases.Length; i++)
            phases[i] = false;

        StartCoroutine(StartAttack());
    }

    private void Update()
    {
        if (health.currentHealth <= 0 || GameManager.instance.dead)
            Die();

        transform.rotation = Quaternion.Lerp(transform.rotation, GetRotation(target.position), lerpRotSpeed * Time.deltaTime);
        float dist = Vector2.Distance(transform.position, target.position);

        //Move when far away
        if (dist > distToMove)
            Move();
        else rb.velocity = Vector2.zero;

        if (phases[0])
            for (int i = 0; i < lasers.Length; i++)
                StartLasers(lasers[i], 100);
        else
            for (int i = 0; i < lasers.Length; i++)
                StopLasers(lasers[i]);

        if (phases[1])
        {
            LaunchRockets();
        }
        else StopRockets();
    }

    void Move()
    {
        rb.velocity = Vector3.Lerp(rb.velocity, transform.up * speed, lerpSpeed * Time.deltaTime);
    }

    IEnumerator StartAttack()
    {
        //Cooldown
        yield return new WaitForSeconds(cooldown);

        phases[attackIndex] = true;
        yield return new WaitForSeconds(laserTime);
        phases[attackIndex] = false;

        //Next Attack - Cycle through
        if(attackIndex < phases.Length - 1)
            attackIndex++;
        else
            attackIndex = 0;

        //Restart
        StartCoroutine(StartAttack());
    }

    #region Lasers
    void StartLasers(Laser laser, float dist)
    {
        if (!laser.active)
        {
            //Set Enabled
            laser.lineRenderer.enabled = true;
            laser.hitParticle.Play();

            laser.laserSound.volume *= SoundManager.instance.soundSlider.value;
            laser.laserSound.Play();
            laser.active = true;
        }

        //Set LR start points
        laser.lineRenderer.SetPosition(0, laser.muzzle.position);

        //Raycasts to get hit
        RaycastHit2D hit = Physics2D.Raycast(laser.muzzle.position, laser.muzzle.up, dist, laserMask);

        lastLsrDmg += Time.deltaTime; //Stops damage from being taken every frame
        if (hit && hit.transform.GetComponent<Health>() && lastLsrDmg > laserDmgCooldown)
        {
            FindFirstObjectByType<CameraFollowScript>().Shake();
            hit.transform.GetComponent<Health>().TakeDamage(laserDamage);
            lastLsrDmg = 0;
        }

        //Set end point
        Vector2 point;
        if (!hit)
        {
            point = laser.muzzle.position + laser.muzzle.up * dist;
        }
        else
        {
            point = hit.point;
        }

        //Set LR end points
        laser.lineRenderer.SetPosition(1, point);

        //Set hit particle pos
        laser.hitParticle.transform.position = point;

        //Set rotation for lasers
        angle = Mathf.Sin(Time.time * laser.freq) * laser.amp * laser.dir;
        laser.barrel.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void StopLasers(Laser laser)
    {
        //Stop visuals
        laser.active = false;
        laser.lineRenderer.enabled = false;
        laser.hitParticle.Stop();
        laser.laserSound.Stop();
    }
    #endregion

    #region Rockets
    void LaunchRockets()
    {
        lastRocket += Time.deltaTime;
        if(lastRocket > rocketCooldown && rocketCount > rocketsLaunched)
        {
            //Launch
            SoundManager.instance.PlaySound("Shoot");
            lastRocket = 0;
            GameObject rocket_ = Instantiate(rocket, launchPoint.position, Quaternion.identity);
            rocket_.GetComponent<RocketScript>().launcher = this.gameObject;
            rocketsLaunched++;
        }
    }

    void StopRockets()
    {
        rocketsLaunched = 0;
    }
    #endregion

    Quaternion GetRotation(Vector2 target)
    {
        //Trig!
        float x = target.x - transform.position.x;
        float y = target.y - transform.position.y;
        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        Quaternion desiredRot = Quaternion.Euler(0, 0, angle - rotationAngleOffset);
        return desiredRot;
    }

    public void Die()
    {
        //Make sure isnt dead then add points
        if (!GameManager.instance.dead)
            GameManager.instance.UpdateScore(scoreWorth);

        health.Die("Explosion");
    }
}

[System.Serializable]
public class Laser
{
    public bool active;
    public AudioSource laserSound;
    public Transform muzzle;
    public Transform barrel;
    public ParticleSystem hitParticle;
    public LineRenderer lineRenderer;
    public float amp;
    public float freq;
    public int dir;
}