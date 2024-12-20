using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public static MapManager _instance;


    void Awake()
    {
        
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }

    public void Start()
    {
        gameObject.SetActive(false);
        
    }

    public void OpenMap(){
        gameObject.SetActive(true);
    }

    public void CloseMap(){
        Game._instance.CloseMap();
        gameObject.SetActive(false);
    }


}
