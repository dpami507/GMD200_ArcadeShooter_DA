using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Basic Enemy Ship that follows the player and fires bullets
public class BasicEnemy : EnemyBaseScript
{
    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] float lerpSpeed;
    [SerializeField] float moveDist;

    [Header("Gun")]
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletSpeed;
    [SerializeField] float bulletSize;
    [SerializeField] float firingAngle;
    [SerializeField] int damage;
    [SerializeField] float shotsPerSecond;
    float lastShot;

    [Header("Sprite")]
    [SerializeField] Color color;
    TrailRenderer trail;

    private void Start()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        target = FindObjectOfType<PlayerManager>().transform;

        //Set up trail
        trail = GetComponent<TrailRenderer>();
        trail.endColor = color;
        trail.startColor = color / 2;
        trail.startWidth = transform.localScale.x / 2;
    }

    private void Update()
    {
        Move();
        transform.rotation = Quaternion.Lerp(transform.rotation, GetRotation(90), rotSpeed * Time.deltaTime);

        //Shooting Logic
        lastShot += Time.deltaTime;
        if (CheckIfCanFire(firingAngle) && lastShot > (1 / shotsPerSecond))
        {
            lastShot = 0;
            Fire();
        }

        if (health.currentHealth <= 0 || GameManager.instance.dead)
        {
            Die();
        }
    }

    void Fire()
    {
        SoundManager.instance.PlaySound("Shoot");
        GameObject bullet_ = Instantiate(bullet, muzzle.position, muzzle.rotation);
        bullet_.GetComponent<BulletScript>().damage = damage;
        bullet_.GetComponent<SpriteRenderer>().color = color;
        bullet_.transform.localScale = Vector2.one * bulletSize;
        bullet_.GetComponent<Rigidbody2D>().velocity = bullet_.transform.up * bulletSpeed;
    }

    void Move()
    {
        float dist = Vector2.Distance(transform.position, target.position);

        Vector3 desiredVel;

        if (dist > moveDist) //Move if far away
            desiredVel = transform.up * speed;
        else desiredVel = Vector3.zero;

        rb.velocity = Vector3.Lerp(rb.velocity, desiredVel, lerpSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Take away all health / Die when touching Lava
        if (collision.transform.CompareTag("Lava"))
        {
            health.TakeDamage(health.currentHealth);
        }
    }
}
