using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public int damage;

    private void Start()
    {
        //Destroy to stop lag
        Destroy(this.gameObject, 5);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Take Damage from collision
        if (collision.GetComponent<Health>())
            collision.GetComponent<Health>().TakeDamage(damage);

        Destroy(this.gameObject);
    }
}
