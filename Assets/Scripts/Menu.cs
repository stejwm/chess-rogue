using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static Menu _instance;
    public AudioSource audioSource;
    void Awake()
    {
        
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }

    // Update is called once per frame
    void Start()
    {
        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = 60;
        if (Display.displays.Length > 1) // Ensure multiple displays are available
        {
            Display.displays[1].Activate(); // Activate second display
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow); // Adjust as needed
        }
    }

    public void StartGame(){
        SceneManager.LoadScene(1);
    }

    public void QuitGame(){
        Debug.Log("Quit");
        Application.Quit();
    }
}
