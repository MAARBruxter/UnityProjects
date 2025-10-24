using UnityEngine;

public class PlayerPoints : MonoBehaviour
{
    [Range(100, 500)]
    [SerializeField] private int pointsPerPowerUp = 250;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("PowerUp"))
        {
            RegisterPowerUpPoints();
            Destroy(collision.gameObject);
        }
    }

    /// <summary>
    /// Gives Player points for collecting a power-up.
    /// </summary>
    private void RegisterPowerUpPoints()
    {
        //The ? operator checks if the Instance is not null before calling the method
        AudioManager.Instance?.PlayPowerUpSound();

        LevelManager.Instance.CurrentPlayerPowerUps++;
        LevelManager.Instance.RemainingPowerUps--;

        //Manage points GameManager
        GameManager.Instance.PlayerPoints += pointsPerPowerUp;

    }
}
