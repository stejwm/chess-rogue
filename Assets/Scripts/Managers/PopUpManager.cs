using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class PopUpManager : MonoBehaviour
{
    
    public static PopUpManager _instance;
    [SerializeField] GameObject piecesPanel; 
    public PieceType selectedPieceType = PieceType.None;
    public GameObject abilityInfo;
    public TMP_Text abilityInfoText;
    public GameObject values;
    public TMP_Text coinValue;
    public TMP_Text bloodValue;

    public bool alreadyActive = false;
    
    void Awake()
    {
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }

    }

    public void ShowValues(){
        gameObject.SetActive(true);

    }
    void Start(){
        Cursor.visible=true;
        gameObject.SetActive(false);
        abilityInfo.SetActive(false);
    }
    public void ShowPieceTypes(){
        gameObject.SetActive(true);
        piecesPanel.SetActive(true);
    }

    public void HidePieceTypes(){
        gameObject.SetActive(false);
        piecesPanel.SetActive(false);
        selectedPieceType=PieceType.None;
    }

    public void KnightSelected(){
        selectedPieceType=PieceType.Knight;
    }
    public void BishopSelected(){
        selectedPieceType=PieceType.Bishop;
    }
    public void QueenSelected(){
        selectedPieceType=PieceType.Queen;
    }
    public void RookSelected(){
        selectedPieceType=PieceType.Rook;
    }

    public void SetAndShowAbilityInfo(AbilityUI abilityUI){
        alreadyActive = gameObject.activeSelf;
        gameObject.SetActive(true);
        
        abilityInfoText.text=abilityUI.ability.description;
        abilityInfo.SetActive(true);
    
        abilityInfo.transform.position=abilityUI.gameObject.transform.position;
        float xVal = abilityInfo.GetComponent<RectTransform>().localPosition.x;
        if(xVal<0){
            Debug.Log("Display Right");
            abilityInfo.GetComponent<RectTransform>().localPosition+=new Vector3(200,0);
        }
        else if(xVal>0)
            abilityInfo.GetComponent<RectTransform>().localPosition-=new Vector3(200,0);


    }

    public void HideAbilityInfo(){
        abilityInfoText.text=null;
        abilityInfo.gameObject.SetActive(false);
        gameObject.SetActive(alreadyActive);
    }

    public void SetAndShowValues(Chessman piece){
        alreadyActive = gameObject.activeSelf;
        gameObject.SetActive(true);
        coinValue.text=piece.releaseCost.ToString();
        bloodValue.text=piece.blood.ToString();
        values.gameObject.SetActive(true);
        values.transform.position=piece.gameObject.transform.position;
        values.GetComponent<RectTransform>().localPosition+=new Vector3(96,0);
        /* float xVal = values.GetComponent<RectTransform>().localPosition.x;
        if(xVal<0)
            
        else if(xVal>0)
            values.GetComponent<RectTransform>().localPosition-=new Vector3(48,0); */
        
    }

    public void HideValues(){
        values.gameObject.SetActive(false);
        gameObject.SetActive(alreadyActive);
    }

}
