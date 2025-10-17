using System.Collections;
using UnityEngine;

public enum PlayerState
{
    color,
    transparent
}

public class PlayerHealth : MonoBehaviour
{

    [Header("Player Health")]
    [Range(0, 10)]
    [SerializeField] private int health;

    [Header("Blink Damage")]
    [SerializeField] private float blinkDuration = 0.2f;
    [SerializeField] private Color blinkColor = Color.red;

    public PlayerState blinkState;

    private bool canTakeDamage = true;

    //Components
    private SpriteRenderer spriteRenderer;

    public int Health { get => health; set => health = value; }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage()
    {
        if (canTakeDamage)
        {
            if (Health <= 0)
            {
                Debug.Log("Player Dead");
                // Handle player death (e.g., reload scene, show game over screen, etc.)
            } else { 
                // Reduce health
                Health--;
                Debug.Log("Player Health: " + Health);
            

                StartCoroutine(BlinkEffect(4));
            }
        }
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
