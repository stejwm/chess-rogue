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
        Game._instance.isInMenu=true;
    }

    public void CloseMap(){
        Game._instance.isInMenu=false;
        Game._instance.CloseMap();
        gameObject.SetActive(false);
    }

    public void NextMatch(){
        CloseMap();
        Game._instance.NextMatch();
    }
    public void OpenShop(){
        CloseMap();
        Game._instance.OpenShop();
    }


}
