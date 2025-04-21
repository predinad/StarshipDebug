using System;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    private float speed = 3.5f; // Default speed of the character
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 lastMoveDirection = Vector2.down;
    private Vector2 movement = Vector2.zero; //added a class-level variable for this to debug player movement
    private float moveX = 0f;
    private float moveY = 0f;
    private bool isMovementAllowed = true; // Toggle for allowing/disabling movement

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (isMovementAllowed)
        {
            Move();
        }
        else
        {
           // rb.linearVelocity = Vector2.zero; // Ensure velocity is zeroed when movement is disabled
           // animator.SetBool("IsMoving", false); // Update animator state
           return;
        }
    }

    private void Move()
    {
        if (!isMovementAllowed)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveY", 0f);
            animator.SetBool("IsMoving", false);
            return;
        }

        moveY=0;
        moveX=0;

        // Handle input for movement directions
        if (Input.GetKey(InputManager.Instance.GetKey(GameAction.MoveUp)))
        {
                moveY = 1; // Move up
        }
        if (Input.GetKey(InputManager.Instance.GetKey(GameAction.MoveDown)))
        {
            moveY = -1; // Move down
        }
        if (Input.GetKey(InputManager.Instance.GetKey(GameAction.MoveLeft)))
        {
            moveX = -1; // Move left
        }
        if (Input.GetKey(InputManager.Instance.GetKey(GameAction.MoveRight)))
        {
            moveX = 1; // Move right
        }

        movement = new Vector2(moveX, moveY);

        // Normalize the vector to prevent faster diagonal movement
        if (movement.magnitude > 1f)
        {
            movement = movement.normalized;
        }

        // Set Rigidbody2D velocity
        rb.linearVelocity = movement * speed;

        // Update animator parameters
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

    public void DisableMovement()
    {
        Debug.Log("Movement disallowed.");
        rb.linearVelocity = Vector2.zero; // Ensure any existing velocity is stopped
        speed=0;
        isMovementAllowed = false;
    }

    public void EnableMovement()
    {
        isMovementAllowed = true;
        speed = 4f; // Restore speed
    }


}