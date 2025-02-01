using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Deals with player aiming, moving, stretching and death
public class PlayerManager : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float jumpForce;
    [SerializeField] float extraGravity;
    Rigidbody2D rb;
    float jumpCooldown;

    [Header("Hands")]
    [SerializeField] Transform handPivot;
    [SerializeField] Transform nearest;
    [SerializeField] int searchRadius;
    [SerializeField] LayerMask enemyMask;

    [Header("Death Stuff")]
    [SerializeField] ParticleSystem explosion;
    [SerializeField] Color color;
    [SerializeField] GameObject body;

    //Other
    [Header("Other")]
    Health health;
    [SerializeField] Transform sprite;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        body.SetActive(true);
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if(GameManager.instance.dead || !GameManager.instance.gameStarted) { return; }

        FaceTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        RotateAndStrechTowardVel();

        if(health.currentHealth <= 0)
            Die();
    }

    private void FixedUpdate()
    {
        if(GameManager.instance.dead || !GameManager.instance.gameStarted) { return; }
        Move();
    }

    void RotateAndStrechTowardVel()
    {
        //Rotate
        Vector2 rbNorm = rb.velocity.normalized;
        float angle = Mathf.Atan2(rbNorm.y, rbNorm.x) * Mathf.Rad2Deg;
        sprite.rotation = Quaternion.Euler(0, 0, angle);

        //Stretch
        float rbMag = Mathf.Abs(rb.velocity.magnitude);
        float shrinkFactor = 0.05f;
        float yScale = Mathf.Max(0.5f, 1 - (rbMag * shrinkFactor));
        Vector2 scale = new Vector2(1, yScale);

        sprite.localScale = scale;
        transform.GetComponent<BoxCollider2D>().size = scale;
    }

    //Mooove
    void Move()
    {
        //Get Input
        float x = Input.GetAxisRaw("Horizontal") * speed;

        //AddForce Movement used as setting velocity destroys the grappling hook and that was bad >:(
        if (Mathf.Abs(x) > .01f) //Input detected
        {
            Vector2 vel = new Vector2(x, 0);
            rb.AddForce(vel, ForceMode2D.Impulse);

            //Counter Force
            Vector2 currentVel = new Vector2(rb.velocity.x, 0);
            rb.AddForce(-currentVel / 8, ForceMode2D.Impulse);
        }
        else if(Mathf.Abs(rb.velocity.x) > .01f) //Still Moving
        {
            if(IsGrounded()) //And Grounded
            {
                Vector2 currentVel = new Vector2(rb.velocity.x, 0);
                rb.AddForce(-currentVel / 4, ForceMode2D.Impulse);
            }
        }

        //Extra Gravity
        rb.AddForce(Vector2.down * extraGravity);

        jumpCooldown += Time.deltaTime; //Stops lots of Jump SFX being spawned

        if(Input.GetButton("Jump") && IsGrounded() && jumpCooldown > 0.1f)
        {
            jumpCooldown = 0;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            SoundManager.instance.PlaySound("Jump");
        }
    }

    //Face Cursor
    void FaceTarget(Vector2 pos)
    {
        //Calculate Angle
        float x = pos.x - transform.position.x;
        float y = pos.y - transform.position.y;
        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        //Rotate
        handPivot.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    //Check if grounded
    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, .02f, groundLayer);
    }

    void Die()
    {
        GameManager.instance.dead = true;
        SoundManager.instance.PlaySound("PlayerDeath");

        ParticleSystem explosion_ = Instantiate(explosion, transform.position, transform.rotation);
        explosion_.transform.localScale = transform.localScale;
        ParticleSystem.MainModule main = explosion_.main;
        main.startColor = color;
        Destroy(explosion_.gameObject, 2f);
        rb.velocity = Vector3.zero;

        body.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<EnemyBaseScript>())
        {
            //The faster you go the more damage you do
            int damage = Mathf.RoundToInt(rb.velocity.magnitude / 4);
            collision.GetComponent<Health>().TakeDamage(damage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Take away all health / Die when touching Lava
        if(collision.transform.CompareTag("Lava"))
        {
            health.TakeDamage(health.currentHealth);
        }
    }
}
