using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Components")]
    private Transform groundDetection;
    private SpriteRenderer spriteRenderer;

    //Ground Check
    [Tooltip("Layer mask for ground detection.")]
    public LayerMask layer;

    //Movement
    [Header("Enemy movement")]
    private Vector2 direction;

    [Tooltip("Determines the speed of the enemy.")]
    [Range(0.5f, 2f)]
    [SerializeField] private float movespeed = 1f;

    [Header("Player Components")]
    private PlayerMovementNewInputSystem playerMovement;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        groundDetection = transform.GetChild(1);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        layer = LayerMask.GetMask("Ground");
        direction = Vector2.left;
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementNewInputSystem>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        AngryPigMove();
    }

    /// <summary>
    /// Controls the way the AngryPig moves.
    /// </summary>
    void AngryPigMove()
    {

        //if ground below is not deteced or the wall is detected, change direction.
        if (!GeneralDetection(1.5f, Vector3.down, Color.yellow) || GeneralDetection(0.5f, direction, Color.green))
        {
            direction = -direction; //same as direction *= -1;
            spriteRenderer.flipX = !spriteRenderer.flipX;
            //Invert the GroundDetection X point
            groundDetection.localPosition = new Vector3(-groundDetection.localPosition.x, groundDetection.localPosition.y, groundDetection.localPosition.z);
        }

        //Move the crab
        transform.position += (Vector3)direction * movespeed * Time.deltaTime;

    }

    /// <summary>
    /// This detects if thew enemy is touching any ground collision.
    /// </summary>
    /// <param name="rayLenght"></param>
    /// <param name="direction"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public bool GeneralDetection(float rayLenght, Vector3 direction, Color color)
    {
        Debug.DrawRay(groundDetection.position, direction * rayLenght, color);
        return Physics2D.Raycast(groundDetection.position, direction, rayLenght, layer);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && playerHealth.CanTakeDamage)
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage();
            playerMovement.ApplyKnockback(transform.position);
            StartCoroutine(VibrateGamePad());
        }
    }

    /// <summary>
    /// Initiates a vibration on the current gamepad for a short duration.
    /// </summary>
    /// <remarks>This coroutine checks if a gamepad is connected and, if so, vibrates it for 0.25 seconds. The
    /// vibration is applied equally to both motors of the gamepad.</remarks>
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
