using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

public class ArmyManager : MonoBehaviour
{
    public List<GameObject> myPieces;
    public TMP_Text bloodText;
    public TMP_Text coinText;

    public List<GameObject> pieces = new List<GameObject>();
    public Chessman selectedPiece;
    public int pricePerPiece = 2;

    //current turn
    public static ArmyManager _instance;


    void Awake()
    {
        
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }

    //Unity calls this right when the game starts, there are a few built in functions
    //that Unity can call for you
    public void Start()
    {
        gameObject.SetActive(false);
        
    }

    public void SelectPiece(Chessman piece)
    {
        selectedPiece=piece;
        piece.highlightedParticles.Play(); 
    }
    public void DeselectPiece(Chessman piece)
    {
        selectedPiece=null;
        piece.highlightedParticles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear); 
    }


    public void PieceSelect(Chessman piece){
        if(selectedPiece==null){
            SelectPiece(piece);
        }
        else if (selectedPiece==piece){
            DeselectPiece(piece);
        }
        else if (selectedPiece && Game._instance.hero.playerCoins>=10){
            BoardPosition position1 = selectedPiece.startingPosition;
            BoardPosition position2 = piece.startingPosition;
            selectedPiece.startingPosition=position2;
            piece.startingPosition=position1;
            selectedPiece.xBoard=position2.x;
            selectedPiece.yBoard=position2.y;
            piece.xBoard=position1.x;
            piece.yBoard=position1.y;
            selectedPiece.UpdateUIPosition();
            piece.UpdateUIPosition();
            Game._instance.hero.playerCoins-=pricePerPiece*2;
            UpdateCurrency();
            DeselectPiece(selectedPiece);
        }
        else{
            piece.GetComponent<MMSpringPosition>().BumpRandom();
        }
    }
    public void PositionSelect(BoardPosition position){
        if(selectedPiece==null){
            return;
        }
        else if (selectedPiece && Game._instance.hero.playerCoins>=pricePerPiece){
            selectedPiece.owner.openPositions.Add(new BoardPosition(selectedPiece.xBoard, selectedPiece.yBoard));
            selectedPiece.owner.openPositions.Remove(position);
            selectedPiece.startingPosition=position;
            selectedPiece.xBoard=position.x;
            selectedPiece.yBoard=position.y;
            selectedPiece.UpdateUIPosition();
            Game._instance.hero.playerCoins-=pricePerPiece;
            UpdateCurrency();
            DeselectPiece(selectedPiece);
        }
        else{
            selectedPiece.GetComponent<MMSpringPosition>().BumpRandom();
        }
    }

    public void OpenShop(){
        //Game._instance.isInMenu=false;
        gameObject.SetActive(true);
        
        UpdateCurrency();
        myPieces=Game._instance.hero.pieces;
        Game._instance.toggleAllPieceColliders(false);
        foreach (GameObject piece in myPieces)
        {
            if (piece !=null && piece.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                rend.sortingOrder = 7;
                piece.GetComponent<Chessman>().highlightedParticles.GetComponent<Renderer>().sortingOrder=5;
            }
            piece.GetComponent<Chessman>().UpdateUIPosition();
        }
        Game._instance.togglePieceColliders(myPieces, true);
        BoardManager._instance.CreateManagementBoard();
        //CreatePieces();
    }

    public void UpdateCurrency(){
        bloodText.text = ": "+Game._instance.hero.playerBlood;
        coinText.text = ": "+Game._instance.hero.playerCoins;
    }

    public void CloseShop(){
        ManagementStatManager._instance.HideStats();
        foreach (GameObject piece in myPieces)
        {
            if (piece.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                rend.sortingOrder = 1;
                piece.GetComponent<Chessman>().highlightedParticles.GetComponent<Renderer>().sortingOrder=0;
                piece.GetComponent<Chessman>().highlightedParticles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
        foreach (GameObject piece in Game._instance.hero.inventoryPieces)
        {
            if (piece.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                rend.sortingOrder = 1;
                piece.GetComponent<Chessman>().highlightedParticles.GetComponent<Renderer>().sortingOrder=0;
                piece.GetComponent<Chessman>().highlightedParticles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
        foreach (GameObject piece in pieces)
        {
            Destroy(piece);
        }
        BoardManager._instance.DestroyBoard();
        Game._instance.CloseShop();
        gameObject.SetActive(false);
    }

    


}
