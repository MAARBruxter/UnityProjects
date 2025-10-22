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
    [Range(0f, 10f)]
    [SerializeField] private float lifetime;
    [SerializeField] private Fruits fruitType;
    [Range(0f, 10f)]
    [SerializeField] private float bounceStrenght;
    private PlayerHealth playerHealth;

    [SerializeField] private float blinkDuration = 0.2f;
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

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        StartCoroutine(BlinkEffect(8));
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

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
