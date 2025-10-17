using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move Settings")]
    [Range(1,10)]
    [SerializeField] private float moveSpeed;
    [Range(1,3)]
    [SerializeField] private float jumpForce;

    private float rayDistance = 0.5f;

    //Inputs
    private float vAxis; // Vertical Axis
    private float hAxis; // Horizontal Axis
    private bool jumpInput;

    private Vector3 jumpVector = Vector3.zero;

    //Components & GameObjects
    private SpriteRenderer playerSprite;
    private Rigidbody2D playerRigidbody;
    private GameObject playerFeet;

    //Ground check layer
    private LayerMask groundLayer;

    private void Awake()
    {
        // Initializations
        playerSprite = GetComponentInChildren<SpriteRenderer>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        groundLayer = LayerMask.GetMask("Ground");
        playerFeet = GameObject.Find("PlayerFeet");
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        MovePlayer();
        FlipSprite();
        IsGrounded();
        CalculateJump();
    }

    private void FixedUpdate()
    {
        Jump();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 start = playerFeet.transform.position;
        Vector3 end = start + Vector3.down * rayDistance;

        Gizmos.DrawLine(start, end);

        Gizmos.DrawWireSphere(end, 0.05f);
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
        //Read Axis
        vAxis = Input.GetAxisRaw("Vertical");
        hAxis = Input.GetAxisRaw("Horizontal");

        //Jump
        jumpInput = Input.GetKeyDown(KeyCode.Space);

        //Debug.Log($"Vertical Axis: {vAxis} - Horizontal Axis: {hAxis}");
    }

    /// <summary>
    /// Change player position with axis and speed.
    /// </summary>
    private void MovePlayer()
    {
        transform.position += Vector3.right * hAxis * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Change player sprite direction based on horizontal axis.
    /// </summary>
    private void FlipSprite()
    {
        if (hAxis > 0)
            playerSprite.flipX = false; //Right direction
        else if (hAxis < 0)
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

        }
        else if (!IsGrounded())
        {

            jumpVector = Vector3.zero;

        }
    }

}
