using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI PowerUpsText;

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
    }

}
