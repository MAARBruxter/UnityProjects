using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovementNewInputSystem : MonoBehaviour
{
    [Header("Move Settings")]
    [Range(1, 10)]
    [SerializeField] private float moveSpeed;
    [Range(1, 3)]
    [SerializeField] private float jumpForce;

    [Space]
    public TMP_Text inputDeviceText;

    private float rayDistance = 0.5f;

    //Inputs
    public InputSystem_Actions playerControls; // Input System Actions
    private Vector2 movementInput; // Movement Input
    private bool jumpInput;
    private bool attackInput;

    private Vector3 jumpVector = Vector3.zero;

    //Components & GameObjects
    private SpriteRenderer playerSprite;
    private Rigidbody2D playerRigidbody;
    private GameObject playerFeet;
    private Animator animator;

    //Ground check layer
    private LayerMask groundLayer;

    //The player was in the ground on the prewvious frame
    private bool wasGrounded = true;

    private void Awake()
    {
        // Initializations
        playerSprite = GetComponentInChildren<SpriteRenderer>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        groundLayer = LayerMask.GetMask("Ground");
        playerFeet = GameObject.Find("PlayerFeet");

        animator = GetComponentInChildren<Animator>();

        //Initialize and enable the Input Actions
        playerControls = new InputSystem_Actions();
        playerControls.Player.Enable();

        UpdateDeviceText();
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        MovePlayer();
        FlipSprite();
        IsGrounded();
        CalculateJump();

        UpdateDeviceText();

        animator.SetFloat("yVelocity", playerRigidbody.linearVelocity.y);
    }

    private void FixedUpdate()
    {
        Jump();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (playerFeet != null)
        {
            Vector3 start = playerFeet.transform.position;

            Vector3 end = start + Vector3.down * rayDistance;

            Gizmos.DrawLine(start, end);

            Gizmos.DrawWireSphere(end, 0.05f);
        }
        
    }

    /// <summary>
    /// Generate the physics for the jump.
    /// </summary>
    private void Jump()
    {
        playerRigidbody.AddForce(jumpVector, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Read the vertical and horizontal Axis from the Input Manager
    /// </summary>
    private void GetInputs()
    {
        //Get movement input from Input System
        movementInput = playerControls.Player.Move.ReadValue<Vector2>();

        //Jump
        jumpInput = playerControls.Player.Jump.triggered;

        //Attack
        attackInput = playerControls.Player.Attack.triggered;

        //Trigger vibration on gamepad when attack input is detected
        if (attackInput && Gamepad.current != null)
        {
            StartCoroutine(VibrateGamePad());
        }

        //Debug.Log($"Vertical Axis: {vAxis} - Horizontal Axis: {hAxis}");
    }

    /// <summary>
    /// Change player position with axis and speed.
    /// </summary>
    private void MovePlayer()
    {
        transform.position += Vector3.right * movementInput.x * moveSpeed * Time.deltaTime;

        //Active or deactivate the running animation based on horizontal input
        animator.SetFloat("Speed", Mathf.Abs(movementInput.x));
    }

    /// <summary>
    /// Change player sprite direction based on horizontal axis.
    /// </summary>
    private void FlipSprite()
    {
        if (movementInput.x > 0)
            playerSprite.flipX = false; //Right direction
        else if (movementInput.x < 0)
            playerSprite.flipX = true; //Left direction
    }

    /// <summary>
    /// Check if player is touching the ground layer.
    /// </summary>
    /// <returns>
    /// Grounded state (true if touching ground, false otherwise).
    /// </returns>
    private bool IsGrounded()
    {
        //Check if player feet collider is touching the ground layer
        return Physics2D.Raycast(playerFeet.transform.position, Vector3.down, 0.5f, groundLayer);
    }

    /// <summary>
    /// Calculate the Vector3 for the jump.
    /// </summary>
    private void CalculateJump()
    {
        if (jumpInput && IsGrounded())
        {

            jumpVector = Vector3.up * jumpForce;
            animator.SetBool("IsJumping", true);

        }
        else if (!IsGrounded())
        {

            jumpVector = Vector3.zero;

        }

        //landing detection
        //Am I on the ground now (this frame)?
        bool grounded = IsGrounded();
        //If the player was in the air before, but now is touching the ground.
        if (grounded && !wasGrounded)
        {
            animator.SetBool("IsJumping", false);
        }
        //for the next frame we update
        wasGrounded = grounded;
    }

    private void OnDisable()
    {
        //Disable the Input Actions
        playerControls.Player.Disable();
    }

    /// <summary>
    /// 
    /// </summary>
    void UpdateDeviceText()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.isPressed)
        {
            inputDeviceText.text = "Using KeyBoard";
        }
        //TODO check Gamepad active with all controls correctly
        else if (Gamepad.current != null && Gamepad.current.leftStick.ReadValue().magnitude > 0)
        {
            inputDeviceText.text = "Using GamePad";
        }
    }

    private IEnumerator VibrateGamePad()
    {
        if (Gamepad.current != null)
        {
            // Vibrate the gamepad
            Gamepad.current.SetMotorSpeeds(0.5f, 0.5f);
            yield return new WaitForSeconds(0.25f); // Vibrate for 0.2 seconds
            Gamepad.current.SetMotorSpeeds(0f, 0f); // Stop vibration
        }
    }

}
