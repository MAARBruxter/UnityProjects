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
        //TODO Play power-up sound effect

        //TODO Manage points GameManager, LevelManager.

    }
}
