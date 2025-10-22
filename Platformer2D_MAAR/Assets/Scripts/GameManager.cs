using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Singelton: To use methods from this class I dont need to get a component
    public static GameManager Instance; //Capital I for this is a static reference to my object

    private void Awake()
    {
        //If the gameObject is already created, destroy old and mantain new
        if (Instance != null)
        {
            //If the GameManager already exists, destroy this duplicate
            Destroy(this.gameObject);
        }
        else
        {
            //Assing the static instance to the current object
            Instance = this;
            DontDestroyOnLoad(this.gameObject); //Make this object persistent between scenes
        }
    }

}
