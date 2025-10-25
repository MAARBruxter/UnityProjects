using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    private void Start()
    {
        //First time we save max level as 1
        if (!PlayerPrefs.HasKey(GameConstants.MAXACT_KEY))
        {
            PlayerPrefs.SetInt(GameConstants.MAXACT_KEY, 1);
        }
    }

    /// <summary>
    /// Advances the application to the next scene in the build order.
    /// </summary>
    /// <remarks>This method loads the scene that follows the current active scene in the build index. Ensure
    /// that the next scene is included in the build settings to avoid runtime errors.</remarks>
    public void SelectAct()
    {
        //Go to the next Scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        //Quit the application
        Application.Quit();
    }

}
