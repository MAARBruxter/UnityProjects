using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI PowerUpsText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI winPointsText;
    [SerializeField] private TextMeshProUGUI gameOverPointsText;

    private PlayerHealth playerHealth;

    [Header("PowerUp Count")]
    public GameObject powerUps;
    private int powerUpCount;

    private void Awake()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        healthText.text = playerHealth.Health.ToString("00");
        PowerUpsText.text = LevelManager.Instance.RemainingPowerUps.ToString("00");

    }

    private void Update()
    {
        powerUpCount = powerUps.transform.childCount;

        healthText.text = playerHealth.Health.ToString("00");
        PowerUpsText.text = LevelManager.Instance.RemainingPowerUps.ToString("00");

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
        gameOverPointsText.text = GameManager.Instance?.PlayerPoints.ToString("00000");
    }

}
