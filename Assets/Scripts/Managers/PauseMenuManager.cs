using System.Collections;
using System.Collections.Generic;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager _instance;
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
        gameObject.SetActive(false);
    }

    public void OpenMenu(){
        gameObject.SetActive(true);
        Time.timeScale=0;
    }

    public void CloseMenu(){
        Time.timeScale=1;
        gameObject.SetActive(false);
    }

    public void StartGame(){
        SceneManager.LoadScene(1);
    }

    public void SaveGame(){
        var writer = QuickSaveWriter.Create("Game");
            writer.Write("Hero", Game._instance.hero);
            writer.Write("State", Game._instance.state);
            writer.Write("Match", Game._instance.currentMatch);
            writer.Commit();
    }

    public void QuitGame(){
        Time.timeScale=1;
        SceneManager.LoadScene(0);
    }
}
