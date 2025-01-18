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
    public float speed;
    public float lerpSpeed;
    public float moveDist;

    [Header("Gun")]
    public Transform muzzle;
    public GameObject bullet;
    public float bulletSpeed;
    public float bulletSize;
    public float firingAngle;
    [SerializeField] bool canFire;
    public float shotsPerSecond;
    float lastShot;

    [Header("Sprite")]
    public Color color;
    TrailRenderer trail;
    public SpriteRenderer sprite;

    [Header("Misc")]
    public int scoreWorth;
    Health health;
    public ParticleSystem explosion;

    private void Start()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        target = FindObjectOfType<PlayerMovement>().transform;

        sprite.color = color;
        trail = GetComponent<TrailRenderer>();
        trail.endColor = color;
        trail.startColor = color / 2;
        trail.startWidth = transform.localScale.x / 2;
    }

    private void Update()
    {
        float dist = Vector2.Distance(transform.position, target.position);

        Vector3 desiredVel;

        if(dist > moveDist)
            desiredVel = transform.up * speed;
        else desiredVel = Vector3.zero;

        rb.velocity = Vector3.Lerp(rb.velocity, desiredVel, lerpSpeed * Time.deltaTime);

        float x = target.position.x - transform.position.x;
        float y = target.position.y - transform.position.y;
        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        Quaternion desiredRot = Quaternion.Euler(0, 0, angle - 90);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, rotSpeed * Time.deltaTime);

        float zRot = transform.rotation.eulerAngles.z;
        float desZRot = desiredRot.eulerAngles.z;

        if (zRot > desZRot - firingAngle && zRot < desZRot + firingAngle)
            canFire = true;
        else canFire = false;

        lastShot += Time.deltaTime;
        if (canFire && lastShot > (1 / shotsPerSecond))
        {
            lastShot = 0;
            Fire();
        }

        void Fire()
        {
            FindFirstObjectByType<SoundManager>().PlaySound("Shoot");
            GameObject bullet_ = Instantiate(bullet, muzzle.position, muzzle.rotation);
            bullet_.GetComponent<SpriteRenderer>().color = color;
            bullet_.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
            bullet_.GetComponent<Rigidbody2D>().velocity = bullet_.transform.up * bulletSpeed;
        }

        if (health.currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        FindFirstObjectByType<SoundManager>().PlaySound("Explosion");
        FindFirstObjectByType<GameManager>().UpdateScore(scoreWorth);
        ParticleSystem explosion_ = Instantiate(explosion, transform.position, transform.rotation);
        explosion_.transform.localScale = transform.localScale;
        explosion_.startColor = color;
        Destroy(explosion_, 2f);
        Destroy(this.gameObject);
    }
}
