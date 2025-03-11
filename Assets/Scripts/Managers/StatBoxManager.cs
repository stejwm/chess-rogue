using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class StatBoxManager : MonoBehaviour
{
    
    public static StatBoxManager _instance;
    public List<StatBox> statBoxes;

    public StatBox EnemyStatBox;
    public bool lockView;
    // Start is called before the first frame update
    void Awake()
    {
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }

    void Start(){
        Cursor.visible=true;
        foreach (var statBox in statBoxes)
        {
            statBox.gameObject.SetActive(false);
        }
    }

    public void LockView(){
        lockView=true;
    }
    public void UnlockView(){
        lockView=false;
    }
    public void SetAndShowStats(Chessman piece){
        if(!lockView)
        {
            foreach (StatBox statBox in statBoxes)
            {
                statBox.SetStats(piece);
            }
        }

    }

    public void SetAndShowEnemyStats(Chessman piece){
        EnemyStatBox.SetStats(piece);
    }

    public void HideStats(){
        foreach (var statBox in statBoxes)
        {
            statBox.HideStats();
        }
    }
}
