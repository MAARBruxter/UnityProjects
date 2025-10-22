using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovementNewInputSystem : MonoBehaviour
{
    [Header("Move Settings")]
    [Range(1, 10)]
    [SerializeField] private float baseMoveSpeed = 5f;
    [Range(1, 10)]
    [SerializeField] private float mediumMoveSpeed = 8f;
    [Range(10, 20)]
    [SerializeField] private float maxMoveSpeed = 15f;
    [SerializeField] private float accelerationTime = 2f; // tiempo en segundos para llegar a velocidad máxima
    [SerializeField] private float decelerationRate = 3f; // qué tan rápido desacelera cuando suelta el botón
    private float accelerationTimer = 0f;
    private float lastDirection = 0f;
    private bool isSkidding = false; // Estado de derrape
    private float skidVelocity = 0f;  // Velocidad durante el derrape
    [Range(1, 20)]
    [SerializeField] private float skidFriction = 5f;  // Qué tan rápido se detiene el derrape

    [Header("Jump Settings")]
    [Range(1, 10)]
    [SerializeField] private float jumpForce = 8f;

    // Inputs
    private Vector2 movementInput;
    private bool jumpInput;
    private bool attackInput;

    // Input Actions
    private InputSystem_Actions playerControls;

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
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackUpForce = 4f;
    [SerializeField] private float knockbackDuration = 0.4f;

    private bool isKnockedBack = false;

    public float AccelerationTimer { get => accelerationTimer; set => accelerationTimer = value; }

    private void Awake()
    {
        // Inicialización de componentes
        playerSprite = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        groundLayer = LayerMask.GetMask("Ground");

        // Inicializa Input Actions
        playerControls = new InputSystem_Actions();
        playerControls.Enable();
    }

    private void OnEnable() => playerControls.Enable();
    private void OnDisable() => playerControls.Disable();

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
    /// Lee los valores de input del jugador
    /// </summary>
    private void GetInputs()
    {
        movementInput = playerControls.Player.Move.ReadValue<Vector2>();
        jumpInput = playerControls.Player.Jump.triggered;
        attackInput = playerControls.Player.Attack.triggered;

        // Vibración del gamepad al atacar
        if (attackInput && Gamepad.current != null)
            StartCoroutine(VibrateGamepad());
    }

    /// <summary>
    /// Mueve al jugador horizontalmente con aceleración progresiva
    /// </summary>
    private void MovePlayer()
    {

        if (isKnockedBack) return;

        float inputX = movementInput.x;

        // --- ACELERACIÓN ---
        if (Mathf.Abs(inputX) > 0.1f)
        {
            // Guarda la última dirección usada
            lastDirection = Mathf.Sign(inputX);

            // Reinicia aceleración si cambia de dirección
            if (Mathf.Sign(inputX) != Mathf.Sign(rb.linearVelocity.x) && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
            {
                isSkidding = true;
                animator.SetBool("IsSkidding", true);
                if (!animator.GetBool("IsJumping"))
                {
                    DustParticles.SetActive(true);
                }
                skidVelocity = rb.linearVelocity.x; // Guarda la velocidad actual para el derrape
            }


            // Aumenta la aceleración
            AccelerationTimer += Time.fixedDeltaTime;
        }
        else
        {
            // Sin input → desacelera gradualmente
            AccelerationTimer -= Time.fixedDeltaTime * decelerationRate;
        }

        // Mantén el timer entre 0 y accelerationTime
        AccelerationTimer = Mathf.Clamp(AccelerationTimer, 0f, accelerationTime);

        // Calcula velocidad actual
        float t = AccelerationTimer / accelerationTime;
        float currentSpeed = Mathf.Lerp(baseMoveSpeed, maxMoveSpeed, t);


        // --- DERRAPE ---
        if (isSkidding)
        {
            //Si salta, las partículas se desactivan
            if (animator.GetBool("IsJumping"))
            {
                DustParticles.SetActive(false);
            }

            // Reduce la velocidad de derrape poco a poco

            skidVelocity = Mathf.MoveTowards(skidVelocity, 0f, skidFriction * Time.fixedDeltaTime); //Nos lleva la velocidad acumulada a 0, en el tiempo definido por skidFriction
            rb.linearVelocity = new Vector2(skidVelocity, rb.linearVelocity.y);

            // Cuando la velocidad llega a 0, termina el derrape
            if (Mathf.Abs(skidVelocity) < 0.01f)
            {
                isSkidding = false;
                animator.SetBool("IsSkidding", false);
                DustParticles.SetActive(false);
                AccelerationTimer = 0f; // Reinicia aceleración para permitir nuevo movimiento
            }

            // Salimos del método aquí para evitar que el jugador pueda mover mientras derrapa
            return;
        }


        // --- MOVIMIENTO CON INERCIA ---
        // Si el jugador no presiona, sigue avanzando en la última dirección
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
            // Cuando el timer llega a 0, se detiene del todo
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }


        // --- ANIMACIÓN ---
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

    public void ApplyKnockback(Vector2 hitSourcePosition)
    {
        if (isKnockedBack) return; // Evita aplicar varias veces
        isKnockedBack = true;

        // Determina dirección opuesta al golpe
        float direction = Mathf.Sign(transform.position.x - hitSourcePosition.x);

        // Limpia velocidad anterior
        rb.linearVelocity = Vector2.zero;

        // Aplica impulso hacia atrás y un poco hacia arriba
        rb.AddForce(new Vector2(direction * knockbackForce, knockbackUpForce), ForceMode2D.Impulse);

        // Inicia recuperación
        StartCoroutine(RecoverFromKnockback());
    }

    private IEnumerator RecoverFromKnockback()
    {
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
        animator.ResetTrigger("Hit");
    }

    /// <summary>
    /// Voltea el sprite según la dirección del movimiento
    /// </summary>
    private void FlipSprite()
    {
        if (movementInput.x > 0)
            playerSprite.flipX = false;
        else if (movementInput.x < 0)
            playerSprite.flipX = true;
    }

    /// <summary>
    /// Verifica si el jugador está tocando el suelo
    /// </summary>
    private bool IsGrounded()
    {
        return Physics2D.Raycast(playerFeet.transform.position, Vector3.down, rayDistance, groundLayer);
    }

    /// <summary>
    /// Calcula el vector de salto y controla las animaciones
    /// </summary>
    private void CalculateJump()
    {
        if (jumpInput && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Reinicia el impulso vertical
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator.SetBool("IsJumping", true);
        }

        // Detecta aterrizaje
        bool grounded = IsGrounded();
        if (grounded && !wasGrounded)
            animator.SetBool("IsJumping", false);

        wasGrounded = grounded;
    }

    /// <summary>
    /// Dibuja el rayo de detección de suelo
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

    /// <summary>
    /// Vibración breve del gamepad al atacar
    /// </summary>
    private IEnumerator VibrateGamepad()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0.5f, 0.5f);
            yield return new WaitForSeconds(0.25f);
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
    }
}