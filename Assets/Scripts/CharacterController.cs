using System;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float speed = 300f; // Speed of the character
    public Rigidbody2D rb;
    public Animator animator;
    private Vector2 lastMoveDirection = Vector2.down;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) {
            moveY = 1; // Move up
        }
        if (Input.GetKey(KeyCode.S)) {
            moveY = -1; // Move down
        }
        if (Input.GetKey(KeyCode.A)) {
            moveX = -1; // Move left
        }
        if (Input.GetKey(KeyCode.D)) {
            moveX = 1; // Move right
        }

        Vector2 movement = new Vector2(moveX, moveY);

        // Normalize the vector to prevent faster diagonal movement
        if (movement.magnitude > 1f) {
            movement = movement.normalized;
        }

        rb.linearVelocity = movement * speed * Time.fixedDeltaTime;

        //Update animator parameters
        animator.SetFloat("MoveX", movement.x);
        animator.SetFloat("MoveY", movement.y);
        animator.SetBool("IsMoving", movement.magnitude > 0);
        
        // Update last movement direction only when there is movement
        if (movement.magnitude > 0)
        {
            lastMoveDirection = movement;
        }

        // Store last move direction for idle animation
        animator.SetFloat("LastX", lastMoveDirection.x);
        animator.SetFloat("LastY", lastMoveDirection.y);
    }
}
