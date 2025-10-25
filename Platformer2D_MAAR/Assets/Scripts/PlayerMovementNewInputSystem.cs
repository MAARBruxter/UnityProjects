using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovementNewInputSystem : MonoBehaviour
{
    [Header("Move Settings")]
    [Tooltip("Base movement speed of the player.")]
    [Range(1, 10)]
    [SerializeField] private float baseMoveSpeed = 5f;

    [Tooltip("Medium movement speed of the player.")]
    [Range(1, 10)]
    [SerializeField] private float mediumMoveSpeed = 8f;

    [Tooltip("Maximum movement speed of the player.")]
    [Range(10, 20)]
    [SerializeField] private float maxMoveSpeed = 15f;

    [Tooltip("Time taken to reach maximum speed.")]
    [SerializeField] private float accelerationTime = 2f;

    [Tooltip("Rate of deceleration when no input is given.")]
    [SerializeField] private float decelerationRate = 3f;

    // Variables for acceleration and skidding
    private float accelerationTimer = 0f;
    private float lastDirection = 0f;
    private bool isSkidding = false;
    private float skidVelocity = 0f;

    [Tooltip("Friction applied during skidding.")]
    [Range(1, 20)]
    [SerializeField] private float skidFriction = 5f;  // How fast the skid slows down

    [Header("Jump Settings")]
    [Tooltip("Jump force applied to the player.")]
    [Range(1, 10)]
    [SerializeField] private float jumpForce = 8f;

    // Inputs
    private Vector2 movementInput;
    private bool jumpInput;
    private bool attackInput;

    // Input Actions
    private InputSystem_Actions playerControls;
    private PlayerHealth playerHealth;

    // Componentes
    private SpriteRenderer playerSprite;
    private Rigidbody2D rb;
    public GameObject playerFeet;
    public GameObject DustParticles;
    private Animator animator;

    // Ground check
    private LayerMask groundLayer;
    private float rayDistance = 0.5f;
    private bool wasGrounded = true;

    [Header("KnockedBack Propierties")]
    [Tooltip("Force applied to the player when knocked back.")]
    [SerializeField] private float knockbackForce = 8f;

    [Tooltip("Upward force applied to the player when knocked back.")]
    [SerializeField] private float knockbackUpForce = 4f;

    [Tooltip("Duration of the knockback effect.")]
    [SerializeField] private float knockbackDuration = 0.4f;

    private bool isKnockedBack = false;

    public float AccelerationTimer { get => accelerationTimer; set => accelerationTimer = value; }

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        // Inicialización de componentes
        playerSprite = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        groundLayer = LayerMask.GetMask("Ground");

        // Inicializa Input Actions
        playerControls = new InputSystem_Actions();
        playerControls.Enable();
    }

    /// <summary>
    /// Enables the player controls.
    /// </summary>
    private void OnEnable()
    {
        playerControls.Enable();
    }

    /// <summary>
    /// Disables the player controls when the object is disabled.
    /// </summary>
    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        GetInputs();
        FlipSprite();
        CalculateJump();

        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    /// <summary>
    /// Reads player inputs from the new Input System
    /// </summary>
    private void GetInputs()
    {
        movementInput = playerControls.Player.Move.ReadValue<Vector2>();
        jumpInput = playerControls.Player.Jump.triggered;
    }

    /// <summary>
    /// Moves the player based on input, handling acceleration, skidding, and inertia.
    /// </summary>
    /// <remarks> Handles acceleration when moving, skidding when changing direction, and inertia when no input is given.</remarks>
    private void MovePlayer()
    {

        if (isKnockedBack) return;

        float inputX = movementInput.x;

        // --- ACELERACIÓN ---
        if (Mathf.Abs(inputX) > 0.1f)
        {
            // Saves the last direction of movement
            lastDirection = Mathf.Sign(inputX);

            // Reload skid if changing direction
            if (Mathf.Sign(inputX) != Mathf.Sign(rb.linearVelocity.x) && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
            {
                isSkidding = true;
                animator.SetBool("IsSkidding", true);
                if (!animator.GetBool("IsJumping"))
                {
                    DustParticles.SetActive(true);
                }
                skidVelocity = rb.linearVelocity.x; // Saves current velocity for skid effect
            }


            // Increases the speed gradually
            AccelerationTimer += Time.fixedDeltaTime;
        }
        else
        {
            // If no input is given, decrease speed gradually
            AccelerationTimer -= Time.fixedDeltaTime * decelerationRate;
        }

        // It keeps the timer between 0 and the max acceleration time
        AccelerationTimer = Mathf.Clamp(AccelerationTimer, 0f, accelerationTime);

        // Calculates current speed based on acceleration timer
        float t = AccelerationTimer / accelerationTime;
        float currentSpeed = Mathf.Lerp(baseMoveSpeed, maxMoveSpeed, t);


        // --- SKIDDING ---
        if (isSkidding)
        {
            //If the player is jumping, stop dust particles
            if (animator.GetBool("IsJumping"))
            {
                DustParticles.SetActive(false);
            }

            // Reduces skid velocity over time

            skidVelocity = Mathf.MoveTowards(skidVelocity, 0f, skidFriction * Time.fixedDeltaTime); //Runs the current skid velocity towards 0
            rb.linearVelocity = new Vector2(skidVelocity, rb.linearVelocity.y);

            // When skid velocity is near 0, stop skidding
            if (Mathf.Abs(skidVelocity) < 0.01f)
            {
                isSkidding = false;
                animator.SetBool("IsSkidding", false);
                DustParticles.SetActive(false);
                AccelerationTimer = 0f; // Restarts acceleration timer after skid
            }

            // We exit the function to avoid other movement calculations.
            return;
        }


        // --- INERTIA ---
        // If the player is not giving input, we maintain movement based on last direction and current speed
        if (Mathf.Abs(inputX) > 0.1f)
        {
            rb.linearVelocity = new Vector2(inputX * currentSpeed, rb.linearVelocity.y);
        }
        else if (AccelerationTimer > 0f)
        {
            rb.linearVelocity = new Vector2(lastDirection * currentSpeed, rb.linearVelocity.y);
        }
        else
        {
            // If the timer reaches 0, we stop horizontal movement
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }


        // --- ANIMATION ---
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        if (currentSpeed < mediumMoveSpeed)
        {
            animator.SetInteger("RunAnimation", Mathf.Abs(1));

        }
        else if (currentSpeed >= mediumMoveSpeed && currentSpeed < maxMoveSpeed)
        {

            animator.SetInteger("RunAnimation", Mathf.Abs(2));

        }
        else if (currentSpeed >= maxMoveSpeed)
        {

            animator.SetInteger("RunAnimation", Mathf.Abs(3));

        }
    }

    /// <summary>
    /// If the player is hit, applies a knockback effect away from the hit source.
    /// </summary>
    /// <param name="hitSourcePosition"></param>
    public void ApplyKnockback(Vector2 hitSourcePosition)
    {
        if (isKnockedBack) return; // It avoids overlapping knockbacks
        isKnockedBack = true;

        // Determines the direction of knockback
        float direction = Mathf.Sign(transform.position.x - hitSourcePosition.x);

        rb.linearVelocity = Vector2.zero;

        // Applies knockback force
        rb.AddForce(new Vector2(direction * knockbackForce, knockbackUpForce), ForceMode2D.Impulse);

        playerHealth.DropFruits(direction);

        // Initiates recovery from knockback after a delay
        StartCoroutine(RecoverFromKnockback());
    }

    /// <summary>
    /// When the knockback duration ends, allows the player to move again.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RecoverFromKnockback()
    {
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
        animator.ResetTrigger("Hit");
    }

    /// <summary>
    /// Turns the player sprite based on movement direction
    /// </summary>
    private void FlipSprite()
    {
        if (movementInput.x > 0)
            playerSprite.flipX = false;
        else if (movementInput.x < 0)
            playerSprite.flipX = true;
    }

    /// <summary>
    /// Verifies if the player is grounded using a raycast
    /// </summary>
    private bool IsGrounded()
    {
        return Physics2D.Raycast(playerFeet.transform.position, Vector3.down, rayDistance, groundLayer);
    }

    /// <summary>
    /// Calculates jump logic based on input and grounded state.
    /// </summary>
    private void CalculateJump()
    {
        if (jumpInput && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Reinicia el impulso vertical
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator.SetBool("IsJumping", true);
        }

        // Detects landing
        bool grounded = IsGrounded();
        if (grounded && !wasGrounded)
            animator.SetBool("IsJumping", false);

        wasGrounded = grounded;
    }

    /// <summary>
    /// Draws gizmos for ground check raycast in the editor.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (playerFeet == null) return;

        Gizmos.color = Color.red;
        Vector3 start = playerFeet.transform.position;
        Vector3 end = start + Vector3.down * rayDistance;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.05f);
    }
}