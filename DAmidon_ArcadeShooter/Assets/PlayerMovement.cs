using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Movement
    public float speed;
    Rigidbody2D rb;
    public Transform groundCheck;
    public Vector2 groundCheckSize;
    public LayerMask groundLayer;
    public float jumpForce;
    public float extraGravity;

    //Hands
    public Transform handPivot;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Move();
        FaceCursor();
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal") * speed;
        rb.velocity = new Vector2 (x, rb.velocity.y);

        rb.AddForce(Vector2.down * extraGravity);

        if(Input.GetButton("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void FaceCursor()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float x = mousePos.x - transform.position.x;
        float y = mousePos.y - transform.position.y;
        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        handPivot.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    bool IsGrounded()
    {
        if(Physics2D.BoxCast(groundCheck.position, groundCheckSize, 0, Vector2.down, groundCheckSize.y, groundLayer))
        {
            return true;
        }

        return false;
    }
}
