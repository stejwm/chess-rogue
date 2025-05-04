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
    public GameObject kingsOrderObject;
    public GameObject KOCanvas;
    public GameObject KOParent;

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
        else if((Game._instance.hero.inventoryPieces.Contains(piece.gameObject) && selectedPiece!=null) || Game._instance.hero.inventoryPieces.Contains(selectedPiece.gameObject)){
            DeselectPiece(selectedPiece);
            return;
        }
        else if (selectedPiece==piece){
            DeselectPiece(piece);
        }
        else if (selectedPiece && Game._instance.hero.playerCoins>=pricePerPiece*2){
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
        if(Game._instance.hero.inventoryPieces.Contains(selectedPiece.gameObject)){
            Game._instance.hero.inventoryPieces.Remove(selectedPiece.gameObject);
            Game._instance.hero.pieces.Add(selectedPiece.gameObject);
            selectedPiece.owner.openPositions.Add(new BoardPosition(selectedPiece.xBoard, selectedPiece.yBoard));
            selectedPiece.owner.openPositions.Remove(position);
            selectedPiece.startingPosition=position;
            selectedPiece.xBoard=position.x;
            selectedPiece.yBoard=position.y;
            selectedPiece.UpdateUIPosition();
            UpdateCurrency();
            Game._instance.OnPieceAdded.Invoke(selectedPiece);
            DeselectPiece(selectedPiece);
        }else if (selectedPiece && Game._instance.hero.playerCoins>=pricePerPiece){
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
        ChessMatch fakeMatch = new ChessMatch(Game._instance.hero);
        Game._instance.currentMatch=fakeMatch;
        gameObject.SetActive(true);
        ShopManager._instance.HideShop();
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


        SpriteRenderer KORend = kingsOrderObject.GetComponent<SpriteRenderer>();
        KORend.sortingOrder = 7;
        

        Canvas KOCanvasRend = KOCanvas.GetComponent<Canvas>();
        KOCanvasRend.sortingOrder = 8;

        KingsOrderManager._instance.flames.GetComponent<Renderer>().sortingOrder=9;


        KingsOrderManager._instance.Setup();
        

        Game._instance.togglePieceColliders(myPieces, true);
        BoardManager._instance.CreateManagementBoard();
        CheckInventory();
    }
    public void CheckInventory(){
        if (Game._instance.hero.inventoryPieces.Count>0){
            //KingsOrderManager._instance.Hide();
            int i = 0;
            foreach (var obj in Game._instance.hero.inventoryPieces)
            {
                Chessman piece = obj.GetComponent<Chessman>();
                obj.SetActive(true);
                piece.xBoard=3+i;
                piece.yBoard=4; 
                i++;
                piece.UpdateUIPosition();
                if (piece !=null && obj.GetComponent<SpriteRenderer>())
                {
                    SpriteRenderer rend = obj.GetComponent<SpriteRenderer>();
                    rend.sortingOrder = 7;
                    piece.highlightedParticles.GetComponent<Renderer>().sortingOrder=5;
                }
                piece.UpdateUIPosition();
            }
            
            Game._instance.togglePieceColliders(Game._instance.hero.inventoryPieces, true);
            Game._instance.togglePieceColliders(Game._instance.hero.pieces, true);
        }
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
                rend.sortingOrder = 5;
                piece.GetComponent<Chessman>().highlightedParticles.GetComponent<Renderer>().sortingOrder=0;
                piece.GetComponent<Chessman>().highlightedParticles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
        foreach (GameObject piece in Game._instance.hero.inventoryPieces)
        {
            if (piece.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                rend.sortingOrder = 5;
                piece.GetComponent<Chessman>().highlightedParticles.GetComponent<Renderer>().sortingOrder=0;
                piece.GetComponent<Chessman>().highlightedParticles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
        foreach (GameObject piece in pieces)
        {
            Destroy(piece);
        }
        SpriteRenderer KORend = kingsOrderObject.GetComponent<SpriteRenderer>();
        KORend.sortingOrder = 1;

        Canvas KOCanvasRend = KOCanvas.GetComponent<Canvas>();
        KOCanvasRend.sortingOrder = 2;

        KingsOrderManager._instance.flames.GetComponent<Renderer>().sortingOrder=5;

        KingsOrderManager._instance.Hide();
        BoardManager._instance.DestroyBoard();
        Game._instance.CloseArmyManagement();
        gameObject.SetActive(false);
    }

    


}
