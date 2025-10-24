using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    
    public static LevelManager Instance; //Capital I for this is a static reference to my object

    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winLevelPanel;

    [Space]
    [Tooltip("Level time in seconds")]
    [Range(0f,300f)]
    [SerializeField]private float levelTime;

    private float internalLevelTime;

    //Variables to manage power-ups in the level
    private int totalLevelPowerUps;

    private int remainingPowerUps;
    public int RemainingPowerUps { get => remainingPowerUps; set => remainingPowerUps = value; }

    private int currentPlayerPowerUps;
    public int CurrentPlayerPowerUps { get => currentPlayerPowerUps; set => currentPlayerPowerUps = value; }
    public float InternalLevelTime { get => internalLevelTime; set => internalLevelTime = value; }

    PlayerHealth playerHealth;

    bool endLevel = false;

    private void Awake()
    {
        //Creates the singleton
        Instance = this;

        //Disable the panels
        gameOverPanel.SetActive(false);
        winLevelPanel.SetActive(false);

        Time.timeScale = 1f; //Resume the game in case it was paused

        //Initialize level time
        internalLevelTime = levelTime;
    }

    private void Start()
    {
        //Initialite
        totalLevelPowerUps = GameObject.FindGameObjectsWithTag("PowerUp").Length;
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        remainingPowerUps = totalLevelPowerUps;
    }

    private void Update()
    {
        if (!endLevel && currentPlayerPowerUps >= totalLevelPowerUps)
        {
            endLevel = true;
            WinLevel();
        }

        //Manage level time
        internalLevelTime -= Time.deltaTime;

        if (internalLevelTime <= 0.9f)
        {
            GameOver();
        }

        //Update of powerups
        remainingPowerUps = totalLevelPowerUps - currentPlayerPowerUps;

    }



    public void WinLevel()
    {
        if (GameManager.Instance)
        {
            //Update Player points with time bonus and life bonus
            GameManager.Instance.PlayerPoints += (int)(internalLevelTime * 100); //Time bonus
            GameManager.Instance.PlayerPoints += (playerHealth.Health * 500); //Life bonus
        }
        
        //Activate the Win Level panel
        winLevelPanel.SetActive(true);
        //Pause the game
        Time.timeScale = 0f;
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
    /// Changes the scene to a nameScene
    /// </summary>
    /// <param name="nameScene"></param>
    public void MainMenuScene()
    {
        //Initialize high score check
        CheckHighScore();

        GameManager.Instance.PlayerPoints = 0; //Reset player points for next game

        SceneManager.LoadScene(GameConstants.MAINMENU_LEVEL_NAME);

        

    }

    public void ReloadScene()
    {
        //Initialize high score check
        CheckMaxLevel();

        CheckHighScore();

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

        CheckMaxLevel();

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

            GameManager.Instance.PlayerPoints = 0; //Reset player points for next game
        }
        
    }

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

    public void CheckMaxLevel()
    {
        //Ask for the current level
        int currentLevelIndex = int.Parse(SceneManager.GetActiveScene().name.Substring(5)); //Assuming level names are in the format "LevelX"

        //Save the next level for the first time if not saved yet
        if (!PlayerPrefs.HasKey(GameConstants.MAXLEVEL_KEY))
        {
            PlayerPrefs.SetInt(GameConstants.MAXLEVEL_KEY, currentLevelIndex + 1);
        }
        else
        {
            if (PlayerPrefs.GetInt(GameConstants.MAXLEVEL_KEY) < currentLevelIndex + 1)
            {
                PlayerPrefs.SetInt(GameConstants.MAXLEVEL_KEY, currentLevelIndex + 1);
            }
        }
    }

}
