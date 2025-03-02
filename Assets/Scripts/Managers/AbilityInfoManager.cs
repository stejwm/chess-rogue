using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class AbilityInfoManager : MonoBehaviour
{
    
    public static AbilityInfoManager _instance;
    [SerializeField] private TMP_Text info;
    [SerializeField] private GameObject PopUpCanvas;
    private bool PopUpCanvasAlreadyActive = false;

    private Camera _mainCamera;
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
        gameObject.SetActive(false);
    }
    public void SetAndShowAbilityInfo(AbilityUI abilityUI){
        PopUpCanvasAlreadyActive = PopUpCanvas.activeSelf;
        PopUpCanvas.SetActive(true);
        gameObject.SetActive(true);
        info.text=abilityUI.ability.description;
    
        this.transform.position=abilityUI.gameObject.transform.position;
        float xVal = this.GetComponent<RectTransform>().localPosition.x;
        if(xVal<0)
            this.GetComponent<RectTransform>().localPosition+=new Vector3(200,0);
        else if(xVal>0)
            this.GetComponent<RectTransform>().localPosition-=new Vector3(200,0);


    }

    public void HideStats(){
        info.text=null;
        this.gameObject.SetActive(false);
        PopUpCanvas.SetActive(PopUpCanvasAlreadyActive);
    }
}
