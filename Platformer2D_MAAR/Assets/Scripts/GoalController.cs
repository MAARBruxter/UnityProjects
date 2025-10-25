using UnityEngine;

public class GoalController : MonoBehaviour
{
    [Header("Goal Settings")]

    //Set to true when the level is completed to prevent multiple triggers.
    private bool endLevel = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!endLevel)
            {
                endLevel = true;
                LevelManager.Instance.WinLevel();
            }
            
        }
    }

}
