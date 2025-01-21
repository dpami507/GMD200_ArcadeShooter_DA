using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour
{
    [Header("Movement")]
    public Transform target;
    public float rotSpeed;
    public float speed;

    [Header("Explosion")]
    public int damage;
    public float explodeDist;
    public float harmDist;
    public ParticleSystem explosion;
    public Color color;

    Rigidbody2D rb;
    Health health;
    TrailRenderer trail;
    [HideInInspector] public RailgunScript launcher;

    void Start()
    {
        target = FindObjectOfType<PlayerManager>().transform;
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();

        trail = GetComponent<TrailRenderer>();
        trail.endColor = color;
        trail.startColor = color / 2;
        trail.startWidth = transform.localScale.x / 2;
    }

    void Update()
    {
        Move();

        //Get Distance to player
        float dist = Vector2.Distance(transform.position, target.position);

        //If health is below zero, close enough to player, or launcher has been killed: Explode
        if (health.currentHealth <= 0 || dist < explodeDist || launcher == null)
            Explode();

        //Rotate to Target
        transform.rotation = Quaternion.Lerp(transform.rotation, GetRotation(), rotSpeed * Time.deltaTime);
    }

    Quaternion GetRotation()
    {
        //Trig!
        float x = target.position.x - transform.position.x;
        float y = target.position.y - transform.position.y;
        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        Quaternion desiredRot = Quaternion.Euler(0, 0, angle - 90);
        return desiredRot;
    }

    //Move if far away
    void Move()
    {
        float dist = Vector2.Distance(transform.position, target.position);

        Vector3 desiredVel;
        desiredVel = transform.up * speed;

        rb.velocity = transform.up * speed;
    }

    void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, harmDist);

        foreach (var item in colliders)
        {
            if(item.GetComponent<Health>())
            {
                item.GetComponent<Health>().TakeDamage(damage);
            }
        }

        health.Die(color, "Explosion");
    }
}
