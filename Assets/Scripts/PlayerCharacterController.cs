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
    private InteractablePuzzle currentInteractable = null; // Reference to the interactable puzzle in range

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
    }

    void Update()
    {
        HandleInteractionInput(); // Check for interact input every frame
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

        moveY = 0;
        moveX = 0;

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
        //Debug.Log("Movement disallowed.");
        rb.linearVelocity = Vector2.zero; // Ensure any existing velocity is stopped
        speed = 0;
        isMovementAllowed = false;
        
        //Debug.Log("speed at 0.");
    }

    public void EnableMovement()
    {
        isMovementAllowed = true;
        speed = 3.5f; // Restore speed
        //Debug.Log("speed back to 3.5.");
    }

    // Interactions handled in Update
    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(InputManager.Instance.GetKey(GameAction.Interact)) && currentInteractable != null && isMovementAllowed)
        {
            //Debug.Log("Interact key pressed while near an interactable.");
            currentInteractable.ActivatePuzzle();
            currentInteractable = null; // Optionally clear the reference after interaction
            DisableMovement();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        InteractablePuzzle puzzle = other.GetComponent<InteractablePuzzle>();
        if (puzzle != null)
        {
            currentInteractable = puzzle;
            //Debug.Log($"Player entered interaction range of {puzzle.gameObject.name}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        InteractablePuzzle puzzle = other.GetComponent<InteractablePuzzle>();
        if (puzzle != null && puzzle == currentInteractable)
        {
            currentInteractable = null;
            //Debug.Log($"Player exited interaction range of {puzzle.gameObject.name}");
        }
    }
}