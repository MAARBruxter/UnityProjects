using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public enum PlayerState
{
    color,
    transparent
}

public class PlayerHealth : MonoBehaviour
{
    [Header("Fruits Settings")]
    [SerializeField] private int fruitCount;

    public int FruitCount { get => fruitCount; set => fruitCount = value; }

    [Header("Blink Damage")]
    [SerializeField] private float blinkDuration = 0.2f;
    [SerializeField] private Color blinkColor = Color.red;
    private Animator animator;

    public PlayerState blinkState;

    private bool canTakeDamage = true;

    //Components
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
