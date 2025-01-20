using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PlayerManager : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    Rigidbody2D rb;
    public Transform groundCheck;
    public Vector2 groundCheckSize;
    public LayerMask groundLayer;
    public float jumpForce;
    public float extraGravity;
    float jumpCooldown;

    [Header("Hands")]
    public Transform handPivot;

    [Header("Death Stuff")]
    public ParticleSystem explosion;
    public Color color;
    public GameObject body;

    //Other
    [Header("Other")]
    Health health;
    GameManager manager;
    public Transform sprite;

    private void Start()
    {
        manager = FindFirstObjectByType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        body.SetActive(true);
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if(manager.dead || !manager.gameStarted) { return; }

        FaceCursor();
        RotateAndStrechTowardVel();

        if(health.currentHealth <= 0)
        {
            Die();
        }
    }

    private void FixedUpdate()
    {
        if(manager.dead || !manager.gameStarted) { return; }
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
        float shrinkFactor = 0.1f;
        float yScale = Mathf.Max(0.5f, 1 - (rbMag * shrinkFactor));
        Vector2 scale = new Vector2(1, yScale);

        sprite.localScale = scale;
        transform.GetComponent<BoxCollider2D>().size = scale;
    }

    //Mooove
    void Move()
    {
        //Add Force
        float x = Input.GetAxisRaw("Horizontal") * speed;

        if (x > .01f || x < -.01f)
        {
            Vector2 vel = new Vector2(x, 0);
            rb.AddForce(vel, ForceMode2D.Impulse);

            //Counter Force
            Vector2 currentVel = new Vector2(rb.velocity.x, 0);
            rb.AddForce(-currentVel / 8, ForceMode2D.Impulse);
        }
        else if(rb.velocity.x > .01f || rb.velocity.x < -.01f)
        {
            if(IsGrounded())
            {
                Vector2 currentVel = new Vector2(rb.velocity.x, 0);
                rb.AddForce(-currentVel);
            }
        }

        //Extra Gravity
        rb.AddForce(Vector2.down * extraGravity);

        jumpCooldown += Time.deltaTime; //Stops lots of Jump SFX being spawned
        if(Input.GetButton("Jump") && IsGrounded() && jumpCooldown > 0.1f)
        {
            jumpCooldown = 0;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            FindFirstObjectByType<SoundManager>().PlaySound("Jump");
        }
    }

    //Face Cursor
    void FaceCursor()
    {
        //Get Mouse Pos
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Calculate Angle
        float x = mousePos.x - transform.position.x;
        float y = mousePos.y - transform.position.y;
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
        manager.dead = true;
        FindFirstObjectByType<SoundManager>().PlaySound("PlayerDeath");

        ParticleSystem explosion_ = Instantiate(explosion, transform.position, transform.rotation);
        explosion_.transform.localScale = transform.localScale;
        ParticleSystem.MainModule main = explosion_.main;
        main.startColor = color;
        Destroy(explosion_, 2f);

        body.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<EnemyScript>() && rb.velocity.magnitude > 16)
        {
            Debug.Log(rb.velocity.magnitude);
            collision.GetComponent<EnemyScript>().Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Take away all health / Die when touching Lava
        if(collision.transform.CompareTag("Lava"))
        {
            health.TakeDamage(9999);
        }
    }
}
