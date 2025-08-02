using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static Menu _instance;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private TMP_Dropdown displayDropdown;
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
        optionsMenu.SetActive(false);
        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = 60;
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }

    public void StartGame(){
        SceneLoadManager.LoadPreviousSave=false;
        SceneManager.LoadScene(1);
    }

    public void LoadGame(){
        SceneLoadManager.LoadPreviousSave=true;
        SceneManager.LoadScene(1);
    }

    public void Options()
    {
        List<string> displayNames = new List<string>();

        for (int i = 0; i < Display.displays.Length; i++)
        {
            var display = Display.displays[i];
            string label = $"Display {i + 1} ({display.systemWidth}x{display.systemHeight})";
            displayNames.Add(label);
        }

        displayDropdown.ClearOptions();
        displayDropdown.AddOptions(displayNames);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
    public void Back()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void SetDisplay(int displayIndex)
    {
        Display.displays[displayIndex].Activate();
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
