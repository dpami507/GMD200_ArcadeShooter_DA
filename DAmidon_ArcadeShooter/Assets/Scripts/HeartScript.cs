using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Heart Pickup, adds health to player when collected and moves towards player when in range
public class HeartScript : MonoBehaviour
{
    [SerializeField] ParticleSystem explosion;
    [SerializeField] Color color;

    [SerializeField] float speed;
    [SerializeField] float minSpeed;
    [SerializeField] float radius;
    [SerializeField] float collectDist;

    private void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var collider in colliders)
        {
            if(collider.GetComponent<PlayerManager>())
            {
                PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();

                if(playerHealth.currentHealth != playerHealth.maxHealth)
                {
                    float dist = Vector2.Distance(transform.position, collider.transform.position);

                    if (dist < collectDist)
                        BeCollected(collider);

                    float moveSpeed = speed * dist * Time.deltaTime;

                    moveSpeed = Mathf.Max(moveSpeed, minSpeed * Time.deltaTime);

                    transform.position = Vector2.MoveTowards(transform.position, collider.transform.position, moveSpeed);
                }
            }
        }
    }

    void BeCollected(Collider2D collision)
    {
        //Add HP
        if (collision.GetComponent<PlayerHealth>())
        {
            PlayerHealth health = collision.GetComponent<PlayerHealth>();

            if (health.currentHealth < health.maxHealth)
            {
                health.currentHealth += 20;

                SoundManager.instance.PlaySound("Health");

                ParticleSystem explosion_ = Instantiate(explosion, transform.position, transform.rotation);
                explosion_.transform.localScale = transform.localScale;
                ParticleSystem.MainModule main = explosion_.main;
                main.startColor = color;
                Destroy(explosion_.gameObject, 2f);
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BeCollected(collision);
    }
}
