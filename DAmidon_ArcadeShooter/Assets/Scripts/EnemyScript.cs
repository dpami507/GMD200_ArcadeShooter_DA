using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Movement")]
    Rigidbody2D rb;
    public Transform target;
    public float rotSpeed;
    [SerializeField] float speed;
    [SerializeField] float lerpSpeed;
    [SerializeField] float moveDist;

    [Header("Gun")]
    public Transform muzzle;
    public GameObject bullet;
    public float bulletSpeed;
    public float bulletSize;
    public float firingAngle;
    public int damage;
    [SerializeField] bool canFire;
    public float shotsPerSecond;
    [HideInInspector] public float lastShot;

    [Header("Sprite")]
    public Color color;
    TrailRenderer trail;
    public SpriteRenderer sprite;

    [Header("Misc")]
    public int scoreWorth;
    [HideInInspector] public Health health;

    private void Start()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        target = FindObjectOfType<PlayerManager>().transform;

        //Set up trail
        sprite.color = color;
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
        if (CheckIfCanFire() && lastShot > (1 / shotsPerSecond))
        {
            lastShot = 0;
            Fire();
        }

        if (health.currentHealth <= 0 || FindFirstObjectByType<GameManager>().dead)
        {
            Die();
        }
    }

    void Fire()
    {
        FindFirstObjectByType<SoundManager>().PlaySound("Shoot");
        GameObject bullet_ = Instantiate(bullet, muzzle.position, muzzle.rotation);
        bullet_.GetComponent<BulletScript>().damage = damage;
        bullet_.GetComponent<SpriteRenderer>().color = color;
        bullet_.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
        bullet_.GetComponent<Rigidbody2D>().velocity = bullet_.transform.up * bulletSpeed;
    }

    public bool CheckIfCanFire()
    {
        //See if angle is within fire range
        float zRot = transform.rotation.eulerAngles.z;
        float desZRot = GetRotation(90).eulerAngles.z;

        if (zRot > desZRot - firingAngle && zRot < desZRot + firingAngle)
            return true;
        else return false;
    }

    public Quaternion GetRotation(float offSet)
    {
        //Trig!
        float x = target.position.x - transform.position.x;
        float y = target.position.y - transform.position.y;
        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        Quaternion desiredRot = Quaternion.Euler(0, 0, angle - offSet);
        return desiredRot;  
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

    public void Die()
    {
        //Make sure isnt dead then add points
        if (!FindFirstObjectByType<GameManager>().dead)
            FindFirstObjectByType<GameManager>().UpdateScore(scoreWorth);

        health.Die(color, "Explosion");
    }
}
