using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Singelton: To use methods from this class I dont need to get a component
    public static AudioManager Instance; //Capital I for this is a static reference to my object

    [SerializeField] private AudioSource powerUpAudio;
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

    public void PlayPowerUpSound()
    {
        powerUpAudio.Play();
    }

    public void PlayDamageSound()
    {
        damageAudio.Play();
    }
}
