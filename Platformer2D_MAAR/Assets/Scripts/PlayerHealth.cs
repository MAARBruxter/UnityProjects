using System.Collections;
using UnityEngine;

public enum PlayerState
{
    color,
    transparent
}

public class PlayerHealth : MonoBehaviour
{
    [Header("Fruits Settings")]
    [Tooltip("Number of fruits collected by the player.")]
    [SerializeField] private int fruitCount;

    private bool canTakeDamage = true;

    public int FruitCount { get => fruitCount; set => fruitCount = value; }
    public bool CanTakeDamage { get => canTakeDamage; set => canTakeDamage = value; }

    [Header("Blink Damage")]
    [Tooltip("Duration of each blink when the player takes damage.")]
    [SerializeField] private float blinkDuration = 0.2f;

    [Tooltip("Color used for the blink effect when the player takes damage.")]
    [SerializeField] private Color blinkColor = Color.red;

    //Components
    private Animator animator;
    public PlayerState blinkState;

    //Components references
    private SpriteRenderer spriteRenderer;

    private PlayerMovementNewInputSystem playerMovement;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementNewInputSystem>();
        animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage()
    {
        if (canTakeDamage)
        {
            AudioManager.Instance?.PlayDamageSound();
            if (fruitCount == 0)
            {
                LevelManager.Instance.GameOver();
            }
            else
            {
                animator.SetTrigger("Hit");
                playerMovement.AccelerationTimer = 0f;
                StartCoroutine(BlinkEffect(6));

            }
        }
    }

    /// <summary>
    /// When the player takes damage, he drops all the fruits that were collected,
    /// scattering them in the direction the player was pushed.
    /// </summary>
    public void DropFruits(float direction)
    {
        for (int i = 0; i < fruitCount; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 1f, 0f);
            GameObject droppedFruit = Instantiate(LevelManager.Instance.fallingApplePrefab, spawnPos, Quaternion.identity);

            Rigidbody2D rb = droppedFruit.GetComponent<Rigidbody2D>();

            // Lanza las frutas principalmente hacia la dirección del golpe
            float forceX = UnityEngine.Random.Range(8f, 10f) * direction;
            float forceY = UnityEngine.Random.Range(4f, 8f);

            rb.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
        }

        // Vacía el contador de frutas
        fruitCount = 0;
    }


    /// <summary>
    /// Repeats blinksTimes a blink every blinckDuration seconds
    /// </summary>
    /// <param name="blinks"></param>
    /// <returns></returns>
    private IEnumerator BlinkEffect(int blinks)
    {
        canTakeDamage = false;
        if (blinkState == PlayerState.color)
        {
            blinkColor = Color.red;
        }
        else if (blinkState == PlayerState.transparent)
        {
            blinkColor = new Color(1f, 1f, 1f, 0.2f); // Transparent white
        }

        do
        {
            // Make the sprite transparent
            spriteRenderer.color = blinkColor;
            yield return new WaitForSeconds(blinkDuration);

            // Restore the original color
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(blinkDuration);

            blinks--;
        } while (blinks > 0);

        canTakeDamage = true;
    }
}
