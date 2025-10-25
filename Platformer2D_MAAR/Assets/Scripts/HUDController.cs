using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [Header("HUD Elements")]
    [Tooltip("Text element displaying the current fruit count.")]
    [SerializeField] private TextMeshProUGUI FruitCountText;

    [Tooltip("Text element displaying the level time.")]
    [SerializeField] private TextMeshProUGUI timeText;

    [Tooltip("Text element displaying the points at the end of the level.")]
    [SerializeField] private TextMeshProUGUI winPointsText;

    [Tooltip("Text element displaying the fruit points at the end of the level.")]
    [SerializeField] private TextMeshProUGUI fruitPointsText;

    [Tooltip("Text element displaying the time points at the end of the level.")]
    [SerializeField] private TextMeshProUGUI timePointsText;

    // Component references
    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        FruitCountText.text = playerHealth.FruitCount.ToString();

        int seconds = (int)LevelManager.Instance.InternalLevelTime % 60;

        int minutes = (int)LevelManager.Instance.InternalLevelTime / 60;

        timeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");

        if (minutes <= 0 && seconds <= 10)
        {
            timeText.color = Color.red;
        }
        else
        {
            timeText.color = Color.white;
        }

        //Only visible at the end of the level
        winPointsText.text = GameManager.Instance?.PlayerPoints.ToString("00000");
        fruitPointsText.text = "X " + playerHealth.FruitCount.ToString() + "= " + GameManager.Instance?.FruitPoints.ToString("00000");
        timePointsText.text = timeText.text + " = " + GameManager.Instance?.TimePoints.ToString("00000");

    }
}
