using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartScript : MonoBehaviour
{
    [SerializeField] ParticleSystem explosion;
    [SerializeField] Color color;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Add HP
        if(collision.GetComponent<Health>())
        {
            Health health = collision.GetComponent<Health>();

            if (health.isPlayer && health.currentHealth < health.maxHealth)
            {
                health.currentHealth += 20;

                FindFirstObjectByType<SoundManager>().PlaySound("Health");

                ParticleSystem explosion_ = Instantiate(explosion, transform.position, transform.rotation);
                explosion_.transform.localScale = transform.localScale;
                ParticleSystem.MainModule main = explosion_.main;
                main.startColor = color;
                Destroy(explosion_.gameObject, 2f);
                Destroy(this.gameObject);
            }
        }
    }
}
