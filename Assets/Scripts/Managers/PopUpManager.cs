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
        //gameObject.SetActive(false);
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

}
