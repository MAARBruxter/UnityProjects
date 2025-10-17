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

        powerUpCount = powerUps.transform.childCount;
    }

    private void Start()
    {
        healthText.text = playerHealth.Health.ToString("00");
        PowerUpsText.text = powerUpCount.ToString("00");
    }

    private void Update()
    {
        powerUpCount = powerUps.transform.childCount;

        if (powerUpCount == 0) {
            Debug.Log("WIN");
        }
            healthText.text = playerHealth.Health.ToString("00");
        PowerUpsText.text = powerUpCount.ToString("00");
    }

}
