using UnityEngine;

public enum EnemyType
{
    Crab,
    Octopus,
    Jumper
}

public class EnemyController : MonoBehaviour
{
    //Enemy type
    public EnemyType enemyType;

    // Components
    private Transform groundDetection;
    private SpriteRenderer spriteRenderer;

    //Ground Check
    public LayerMask layer;

    //Movement
    [Header("Enemy movement")]
    [SerializeField] private Vector2 direction;
    [Range(0.5f, 2f)]
    [SerializeField] private float movespeed = 1f;

    [Header("Jumper move Settings")]
    [Range(0.5f, 2.5f)]
    [SerializeField] private float sinAmplitude;
    [Range(0.5f, 2.5f)]
    [SerializeField] private float sinFrecuency;
    private float sinCenterY;
    [SerializeField] private Vector3 initPos;
    [SerializeField] private float maxPos;

    private void Awake()
    {
        groundDetection = transform.GetChild(1);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        layer = LayerMask.GetMask("Ground");
        initPos = transform.position;

        if (enemyType == EnemyType.Crab)
        {
            direction = Vector2.right;
        }

        if (enemyType == EnemyType.Octopus)
        {
            direction = Vector2.up;
        }
    }

    private void Start()
    {
        sinCenterY = transform.position.y;
    }

    private void EnemyMove()
    {
        switch (enemyType)
        {
            case EnemyType.Crab:
                CrabMove();
                break;
            case EnemyType.Octopus:
                OctopusMove();
                break;
            case EnemyType.Jumper:
                JumperMove();
                break;
        }
    }

    private void Update()
    {
        EnemyMove();
    }

    /// <summary>
    /// Controls the way the crab moves.
    /// </summary>
    void CrabMove()
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
    /// Controls the way the octopus moves.
    /// </summary>
    void OctopusMove()
    {
        //if the wall is detected, change direction.
        if (GeneralDetection(0.5f, direction, Color.green))
        {
            direction = -direction; //same as direction *= -1;
            groundDetection.localPosition = new Vector3(groundDetection.localPosition.x, -groundDetection.localPosition.y, groundDetection.localPosition.z);
        }

        //Move the octopus
        transform.position += (Vector3)direction * movespeed * Time.deltaTime;

    }

    /// <summary>
    /// Controls the way the jumper moves with SinFrecuency and Sin Amplitude.
    /// </summary>
    void JumperMove()
    {
        limitLateral();

        if (GeneralDetection(1f, direction, Color.magenta))
        {
            direction = -direction; //same as direction *= -1;
        }

        Vector3 enemyPosition = transform.position;

        //Returns an asocillating value between -sinAmplitude and +sinAmplitude
        //sinFrecuency controls how many oscillations happen per unit of distance
        //sinAmplitude controls the jump height
        float sin = Mathf.Sin(enemyPosition.x * sinFrecuency) * sinAmplitude;

        // sinCenterY is the base Y position, sin will make it go up and down
        //y oscillate smoothly in the form of a sinusidal wave
        enemyPosition.y = sinCenterY + sin;
        enemyPosition.x += direction.x * movespeed * Time.deltaTime;

        transform.position = enemyPosition;
    }

    void limitLateral()
    {
        if (transform.position.x <= initPos.x - maxPos)
        {
            direction = -direction;
        }
        else if (transform.position.x >= initPos.x + maxPos)
        {
            direction = -direction;
        }
    }

    /// <summary>
    /// This detects if thew enemy is touching any ground collision.
    /// </summary>
    /// <param name="rayLenght"></param>
    /// <param name="direction"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    private bool GeneralDetection(float rayLenght, Vector3 direction, Color color)
    {
        Debug.DrawRay(groundDetection.position, direction * rayLenght, color);
        return Physics2D.Raycast(groundDetection.position, direction, rayLenght, layer);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage();
        }
    }
}
