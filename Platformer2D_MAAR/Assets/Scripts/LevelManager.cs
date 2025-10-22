using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance; //Capital I for this is a static reference to my object

    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winLevelPanel;

    private void Awake()
    {
        //Creates the singleton
        Instance = this;

        //Disable the GameOver panel
        gameOverPanel.SetActive(false);
        winLevelPanel.SetActive(false);

        Time.timeScale = 1f; //Resume the game in case it was paused
    }

    /// <summary>
    /// Activates the game over panel and pauses the game.
    /// </summary>
    public void GameOver()
    {
        //Activate the Game Over panel
        gameOverPanel.SetActive(true);
        //Pause the game
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Activates the win level panel and pauses the game.
    /// </summary>
    public void WinLevel()
    {
        //Activate the Win Level panel
        winLevelPanel.SetActive(true);
        //Pause the game
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Changes the scene to a nameScene
    /// </summary>
    /// <param name="nameScene"></param>
    public void MainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Loads the next scene in the build index. If there is 
    /// </summary>
    /// <remarks>This method increments the current scene's build index by one and loads the corresponding
    /// scene. Ensure that the next scene is included in the build settings to avoid runtime errors.</remarks>
    public void NextLevel()
    {
        //Total number of levels in build settings.
        int levelCount = SceneManager.sceneCountInBuildSettings - 1;

        //If there are more levels, load the next one
        if (SceneManager.GetActiveScene().buildIndex < levelCount)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {

        }

    }

}
