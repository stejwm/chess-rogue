using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    public TMP_Text abilityInfoTitle;
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
        

        abilityInfoTitle.text=abilityUI.ability.abilityName;
        abilityInfoText.text=abilityUI.ability.description;
        abilityInfo.SetActive(true);
    
        abilityInfo.transform.position=abilityUI.gameObject.transform.position;
        float xVal = abilityInfo.GetComponent<RectTransform>().localPosition.x;
        if(xVal<0){
            abilityInfo.GetComponent<RectTransform>().localPosition+=new Vector3(200,0);
        }
        else if(xVal>0)
            abilityInfo.GetComponent<RectTransform>().localPosition-=new Vector3(200,0);

        LayoutRebuilder.ForceRebuildLayoutImmediate(abilityInfo.GetComponent<RectTransform>());
    }
    public void SetAndShowText(string text, GameObject parent){
        alreadyActive = gameObject.activeSelf;
        gameObject.SetActive(true);
        
        abilityInfoText.text=text;
        abilityInfo.SetActive(true);
    
        abilityInfo.transform.position=parent.transform.position;
        float xVal = abilityInfo.GetComponent<RectTransform>().localPosition.x;
        if(xVal<0){
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
    public void DiplomacyValues(Transform transform){
        SetAndShowUpgrades(25,0, transform);
    }
    public void StatValues(Transform transform){
        SetAndShowUpgrades(0,1, transform);
    }
    public void SetAndShowUpgrades(int coins, int blood, Transform transform){
        alreadyActive = gameObject.activeSelf;
        gameObject.SetActive(true);
        if(coins!=0)
            coinValue.text=":"+coins.ToString();
        else
            coinValue.text=":X";
        if(blood!=0)
            bloodValue.text=":"+blood.ToString();
        else
            bloodValue.text=":X";
        values.gameObject.SetActive(true);
        values.transform.position=transform.position;
        values.GetComponent<RectTransform>().localPosition+=new Vector3(-85,0);
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
