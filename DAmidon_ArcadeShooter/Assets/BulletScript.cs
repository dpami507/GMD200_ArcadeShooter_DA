using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public int damage;

    private void Start()
    {
        Destroy(this.gameObject, 2);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Health>())
            collision.GetComponent<Health>().TakeDamage(damage);

        Destroy(this.gameObject);
    }
}
