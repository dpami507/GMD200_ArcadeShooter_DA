using UnityEngine;

//Script for the bullet to clean up if it hasn't hit anything and take away damage when it does
public class BulletScript : MonoBehaviour
{
    public int damage;
    [SerializeField] ParticleSystem explosion;
    Color color;

    private void Start()
    {
        //Destroy to stop lag
        Destroy(this.gameObject, 5);

        color = GetComponent<SpriteRenderer>().color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Take Damage from collision
        if (collision.GetComponent<Health>())
            collision.GetComponent<Health>().TakeDamage(damage);

        ParticleSystem explosion_ = Instantiate(explosion, transform.position, transform.rotation);

        ParticleSystem.MainModule main = explosion_.main;
        main.startColor = color;

        Destroy(explosion_.gameObject, 2f);
        Destroy(this.gameObject);
    }
}
