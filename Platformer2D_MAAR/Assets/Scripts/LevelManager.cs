using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance; //Capital I for this is a static reference to my object

    [Header("Panels")]
    [Tooltip("UI Panels for Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    [Tooltip("UI Panels for Win Level")]
    [SerializeField] private GameObject winLevelPanel;

    [Tooltip("UI for the current Act")]
    [SerializeField] private GameObject ActUI;
    [Space]
    [Tooltip("Falling apple prefab")]
    public GameObject fallingApplePrefab;

    [Space]
    [Tooltip("Level time in seconds")]
    [Range(0f, 300f)]
    [SerializeField] private float levelTime;

    private float internalLevelTime;

    public float InternalLevelTime { get => internalLevelTime; set => internalLevelTime = value; }

    private PlayerHealth playerHealth;

    private void Awake()
    {
        //Creates the singleton
        Instance = this;

        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();

        //Disable the GameOver panel
        gameOverPanel.SetActive(false);
        winLevelPanel.SetActive(false);

        Time.timeScale = 1f; //Resume the game in case it was paused

        //Initialize level time
        internalLevelTime = levelTime;
    }

    private void Update()
    {
        

        //Manage level time
        internalLevelTime -= Time.deltaTime;

        if (internalLevelTime <= 0.9f)
        {
            GameOver();
        }
    }

    /// <summary>
    /// Activates the game over panel and pauses the game.
    /// </summary>
    public void GameOver()
    {
        //Disable the Act UI
        ActUI.SetActive(false);
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
        if (GameManager.Instance)
        {
            //Update Player points with time bonus and life bonus
            GameManager.Instance.TimePoints += (int)(internalLevelTime * 100); //Time bonus
            GameManager.Instance.FruitPoints += playerHealth.FruitCount * 500; //Fruit bonus
            GameManager.Instance.PlayerPoints = GameManager.Instance.TimePoints + GameManager.Instance.FruitPoints;
        }

        //Disable the Act UI
        ActUI.SetActive(false);
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
        //Initialize high score check
        CheckHighScore();

        GameManager.Instance.TimePoints = 0; //Reset time points for next game
        GameManager.Instance.FruitPoints = 0; //Reset fruit points for next game
        GameManager.Instance.PlayerPoints = 0; //Reset player points for next game

        SceneManager.LoadScene(GameConstants.MAINMENU_LEVEL_NAME);
    }

    public void ReloadScene()
    {
        //Initialize high score check
        CheckMaxAct();

        CheckHighScore();

        GameManager.Instance.TimePoints = 0; //Reset time points for next game
        GameManager.Instance.FruitPoints = 0; //Reset fruit points for next game
        GameManager.Instance.PlayerPoints = 0; //Reset player points for next game

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Loads the next scene in the build index. If there is 
    /// </summary>
    /// <remarks>This method increments the current scene's build index by one and loads the corresponding
    /// scene. Ensure that the next scene is included in the build settings to avoid runtime errors.</remarks>
    public void NextLevel()
    {
        CheckMaxAct();

        //Total number of levels in build settings.
        int levelCount = SceneManager.sceneCountInBuildSettings - 1;

        //If there are more levels, load the next one
        if (SceneManager.GetActiveScene().buildIndex < levelCount)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            // Last level
            CheckHighScore();

            GameManager.Instance.TimePoints = 0; //Reset time points for next game
            GameManager.Instance.FruitPoints = 0; //Reset fruit points for next game
            GameManager.Instance.PlayerPoints = 0; //Reset player points for next game
        }

    }

    /// <summary>
    /// Checks and updates the high score if the current player's score exceeds the previously saved high score.
    /// </summary>
    /// <remarks>This method retrieves the existing high score from persistent storage and compares it with
    /// the current player's score. If the current score is higher, it updates the stored high score. If no high score
    /// is saved, it initializes the high score with the current player's score.</remarks>
    private void CheckHighScore()
    {
        //Check if previous high score saved
        if (PlayerPrefs.HasKey(GameConstants.HIGHSCORE_KEY))
        {
            int highScore = PlayerPrefs.GetInt(GameConstants.HIGHSCORE_KEY);
            //If current score is higher than saved high score, update it
            if (GameManager.Instance.PlayerPoints > highScore)
            {
                PlayerPrefs.SetInt(GameConstants.HIGHSCORE_KEY, GameManager.Instance.PlayerPoints);
            }
        }
        else
        {
            //No high score saved, save current score as high score
            PlayerPrefs.SetInt(GameConstants.HIGHSCORE_KEY, GameManager.Instance.PlayerPoints);
        }
    }

    /// <summary>
    /// Updates the maximum act reached by the player if the current act exceeds the previously recorded maximum.
    /// </summary>
    /// <remarks>This method checks the current act index and updates the stored maximum act in player
    /// preferences if the current act is higher. The act names are assumed to be in the format "ActX", where X is
    /// the act number.</remarks>
    public void CheckMaxAct()
    {
        //Ask for the current act
        int currentLevelIndex = int.Parse(SceneManager.GetActiveScene().name.Substring(3)); //Assuming act names are in the format "ActX"

        //Save the next level for the first time if not saved yet
        if (!PlayerPrefs.HasKey(GameConstants.MAXACT_KEY))
        {
            PlayerPrefs.SetInt(GameConstants.MAXACT_KEY, currentLevelIndex + 1);
        }
        else
        {
            if (PlayerPrefs.GetInt(GameConstants.MAXACT_KEY) < currentLevelIndex + 1)
            {
                PlayerPrefs.SetInt(GameConstants.MAXACT_KEY, currentLevelIndex + 1);
            }
        }
    }

}
