using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreTextText;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(GameConstants.HIGHSCORE_KEY))
        {
            highScoreTextText.text = "High Score " + PlayerPrefs.GetInt(GameConstants.HIGHSCORE_KEY).ToString("00000");
        }
    }

    void Start()
    {
        //Get children lenght of Button Container --> Number of Levels
        GameObject buttonContainer = GameObject.Find(GameConstants.BUTTONCONTAINER_KEY);

        //dinamically put on every button the listener loadLevelSelection
        for (int i = 0; i < buttonContainer.transform.childCount; i++)
        {
            int levelIndex = i; //Levels start from 1

            //if the level button is lower or equal than the max level unlocked, add listener
            if (levelIndex + 1 <= PlayerPrefs.GetInt(GameConstants.MAXLEVEL_KEY))
            {
                buttonContainer.transform.GetChild(levelIndex).GetComponent<Button>().interactable = true;
                buttonContainer.transform.GetChild(levelIndex).GetComponent<Button>().onClick.AddListener(() => LoadLevelSelection(levelIndex + 1));
            }
            else
            {
                //Disable button if level is locked
                buttonContainer.transform.GetChild(levelIndex).GetComponent<Button>().interactable = false;
            }
        }
    }

    /// <summary>
    /// Load the scene with LevelIndex + 1 to get the actual level name.
    /// </summary>
    /// <param name="levelIndex"></param>
    public void LoadLevelSelection(int levelIndex)
    {
        //Load the selected level
        UnityEngine.SceneManagement.SceneManager.LoadScene(GameConstants.LEVEL_KEY + levelIndex);
    }
}
