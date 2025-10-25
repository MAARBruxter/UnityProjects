using System.Collections;
using UnityEngine;

public enum Fruits
{
    apple,
    fallingApple
}

public class FruitController : MonoBehaviour
{
    [Header("Fruit Settings")]
    [Tooltip("Time before the fruit disappears")]
    [Range(0f, 10f)]
    [SerializeField] private float lifetime;

    [Tooltip("Type of fruit")]
    [SerializeField] private Fruits fruitType;

    [Tooltip("Strength of the bounce when hitting the ground")]
    [Range(0f, 10f)]
    [SerializeField] private float bounceStrenght;
    private PlayerHealth playerHealth;

    [Header("Blink Settings")]
    [Tooltip("Duration of each blink")]
    [Range(0.05f, 1f)]
    [SerializeField] private float blinkDuration = 0.2f;

    [Tooltip("Color used for blinking effect")]
    [SerializeField] private Color blinkColor;

    //Components
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (fruitType == Fruits.fallingApple)
        {
            StartCoroutine(DestroyAfterTime());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.Instance?.FruitSound();
            playerHealth.FruitCount++;
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initiates a sequence that destroys the game object after a specified lifetime.
    /// </summary>
    /// <remarks>The method waits for the specified lifetime duration, triggers a blinking effect,  and then
    /// waits for the same duration again before destroying the game object.</remarks>
    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        StartCoroutine(BlinkEffect(8));
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    /// <summary>
    /// Performs a blinking effect on the sprite by alternating its transparency.
    /// </summary>
    /// <remarks>This method changes the sprite's color to a transparent white and then back to its original
    /// color, creating a blinking effect. Each blink consists of two phases: transparent and opaque, each lasting for
    /// the duration specified by <c>blinkDuration</c>.</remarks>
    /// <param name="blinks">The number of times the sprite should blink. Must be a positive integer.</param>
    private IEnumerator BlinkEffect(int blinks)
    {
        blinkColor = new Color(1f, 1f, 1f, 0.2f); // Transparent white

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
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica si el objeto golpea el suelo
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {

            // Aplica una fuerza hacia arriba
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceStrenght);

        }
    }

}
