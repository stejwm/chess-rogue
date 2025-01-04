using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    public static LogManager _instance;
    [SerializeField] private RectTransform content;
    public GameObject LogPrefab;
    public TMP_Text LogText;
    private float currentHeight = 0f;
    [SerializeField] private float messageSpacing = 10f;

    void Awake()
    {
        
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }
    public void WriteLog(string message)
    {
        LogText.text += message + "\n";
    }
    public void ClearLogs()
    {
        LogText.text ="";
    }

}
