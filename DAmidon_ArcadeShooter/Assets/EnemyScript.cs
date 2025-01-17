using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    Rigidbody2D rb;
    public Transform target;
    public float rotSpeed;
    public float speed;

    public float moveDist;

    public int scoreWorth;

    Health health;

    private void Start()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        target = FindObjectOfType<PlayerMovement>().transform;
    }

    private void Update()
    {
        float dist = Vector2.Distance(transform.position, target.position);

        if(dist > moveDist)
            rb.velocity = transform.up * speed;
        else rb.velocity = Vector3.zero;

        float x = target.position.x - transform.position.x;
        float y = target.position.y - transform.position.y;
        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        Quaternion desiredRot = Quaternion.Euler(0, 0, angle - 90);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, rotSpeed * Time.deltaTime);

        if(health.currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        FindFirstObjectByType<GameManager>().UpdateScore(scoreWorth);
        Destroy(this.gameObject);
    }
}
