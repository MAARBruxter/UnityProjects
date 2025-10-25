using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Singelton: To use methods from this class I dont need to get a component
    public static AudioManager Instance; //Capital I for this is a static reference to my object

    [Header("Audio Sources")]
    [Tooltip("Audio source for fruit collection sound")]
    [SerializeField] private AudioSource fruitAudio;

    [Tooltip("Audio source for damage sound")]
    [SerializeField] private AudioSource damageAudio;

    private void Awake()
    {
        //If the gameObject is already created, destroy old and mantain new
        if (Instance != null)
        {
            //If the AudioManager already exists, destroy this duplicate
            Destroy(this.gameObject);
        }
        else
        {
            //Assing the static instance to the current object
            Instance = this;
            DontDestroyOnLoad(this.gameObject); //Make this object persistent between scenes
        }
    }

    /// <summary>
    /// Plays the fruit collection sound.
    /// </summary>
    public void FruitSound()
    {
        fruitAudio.Play();
    }

    /// <summary>
    /// Plays the damage sound.
    /// </summary>
    public void PlayDamageSound()
    {
        damageAudio.Play();
    }
}
