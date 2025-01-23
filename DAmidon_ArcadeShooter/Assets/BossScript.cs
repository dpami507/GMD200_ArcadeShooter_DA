using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] int scoreWorth;
    [SerializeField] float lerpSpeed;
    [SerializeField] Color color;

    [Header("Movement")]
    [SerializeField] Transform target;
    [SerializeField] float speed;
    [SerializeField] float distToMove;

    [Header("Rotation")]
    [SerializeField] float rotationAngleOffset;
    [SerializeField] float lerpRotSpeed;

    [Header("Lasers")]
    [SerializeField] LayerMask laserMask;
    [SerializeField] int laserDamage;
    [SerializeField] Laser[] lasers;
    float laserDmgCooldown = .05f;
    float lastDmg;

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
        if (health.currentHealth <= 0 || FindFirstObjectByType<GameManager>().dead)
        {
            Die();
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, GetRotation(target.position), lerpRotSpeed * Time.deltaTime);

        if (GetDistance(target.position) > distToMove)
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
        phases[attackIndex] = true;
        yield return new WaitForSeconds(laserTime);
        phases[attackIndex] = false;

        //Cooldown
        yield return new WaitForSeconds(cooldown);

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
        //Set Enabled
        laser.lineRenderer.enabled = true;
        laser.hitParticle.Play();

        //Set LR start points
        laser.lineRenderer.SetPosition(0, laser.muzzle.position);

        //Raycasts to get hit
        RaycastHit2D hit = Physics2D.Raycast(laser.muzzle.position, laser.muzzle.up, dist, laserMask);

        lastDmg += Time.deltaTime; //Stops damage from being taken every frame
        if (hit && hit.transform.GetComponent<Health>() && lastDmg > laserDmgCooldown)
        {
            hit.transform.GetComponent<Health>().TakeDamage(laserDamage);
            lastDmg = 0;
        }

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

        angle = Mathf.Sin(Time.time * laser.freq) * laser.amp * laser.dir;
        laser.barrel.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void StopLasers(Laser laser)
    {
        laser.lineRenderer.enabled = false;
        laser.hitParticle.Stop();
    }
    #endregion

    #region Rockets
    void LaunchRockets()
    {
        lastRocket += Time.deltaTime;
        if(lastRocket > rocketCooldown && rocketCount > rocketsLaunched)
        {
            //Launch
            FindFirstObjectByType<SoundManager>().PlaySound("Shoot");
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

    float GetDistance(Vector2 target)
    {
        return Vector2.Distance(transform.position, target);
    }

    public void Die()
    {
        //Make sure isnt dead then add points
        if (!FindFirstObjectByType<GameManager>().dead)
            FindFirstObjectByType<GameManager>().UpdateScore(scoreWorth);

        health.Die(color, "Explosion");
    }
}

[System.Serializable]
public class Laser
{
    public Transform muzzle;
    public Transform barrel;
    public ParticleSystem hitParticle;
    public LineRenderer lineRenderer;
    public float amp;
    public float freq;
    public int dir;
}