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
    public Board board;
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
        else if((board.Hero.inventoryPieces.Contains(piece.gameObject) && selectedPiece!=null) || board.Hero.inventoryPieces.Contains(selectedPiece.gameObject)){
            DeselectPiece(selectedPiece);
            return;
        }
        else if (selectedPiece==piece){
            DeselectPiece(piece);
        }
        else if (selectedPiece && board.Hero.playerCoins>=pricePerPiece*2){
            Tile position1 = selectedPiece.startingPosition;
            Tile position2 = piece.startingPosition;
            selectedPiece.startingPosition=position2;
            piece.startingPosition=position1;
            selectedPiece.xBoard=position2.X;
            selectedPiece.yBoard=position2.Y;
            piece.xBoard=position1.X;
            piece.yBoard=position1.Y;
            selectedPiece.UpdateUIPosition();
            piece.UpdateUIPosition();
            board.Hero.playerCoins-=pricePerPiece*2;
            UpdateCurrency();
            DeselectPiece(selectedPiece);
        }
        else{
            piece.GetComponent<MMSpringPosition>().BumpRandom();
        }
    }
    public void PositionSelect(Tile position){
        if(selectedPiece==null){
            return;
        }
        if(board.Hero.inventoryPieces.Contains(selectedPiece.gameObject)){
            board.Hero.inventoryPieces.Remove(selectedPiece.gameObject);
            board.Hero.pieces.Add(selectedPiece.gameObject);
            selectedPiece.owner.openPositions.Add(selectedPiece.startingPosition);
            selectedPiece.owner.openPositions.Remove(position);
            selectedPiece.startingPosition=position;
            selectedPiece.xBoard=position.X;
            selectedPiece.yBoard=position.Y;
            selectedPiece.UpdateUIPosition();
            UpdateCurrency();
            board.EventHub.RaisePieceAdded(selectedPiece);
            DeselectPiece(selectedPiece);
        }else if (selectedPiece && board.Hero.playerCoins>=pricePerPiece){
            selectedPiece.owner.openPositions.Add(selectedPiece.startingPosition);
            selectedPiece.owner.openPositions.Remove(position);
            selectedPiece.startingPosition=position;
            selectedPiece.xBoard=position.X;
            selectedPiece.yBoard=position.Y;
            selectedPiece.UpdateUIPosition();
            board.Hero.playerCoins-=pricePerPiece;
            UpdateCurrency();
            DeselectPiece(selectedPiece);
        }
        else{
            selectedPiece.GetComponent<MMSpringPosition>().BumpRandom();
        }
    }

    public void OpenShop(){
        //NEED to rewrite this
    }
    public void CheckInventory(){
        if (board.Hero.inventoryPieces.Count>0){
            //KingsOrderManager._instance.Hide();
            int i = 0;
            foreach (var obj in board.Hero.inventoryPieces)
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
        }
    }

    public void UpdateCurrency(){
        bloodText.text = ": "+board.Hero.playerBlood;
        coinText.text = ": "+board.Hero.playerCoins;
    }

    public void CloseShop(){
        //ManagementStatManager._instance.HideStats();
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
        foreach (GameObject piece in board.Hero.inventoryPieces)
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

       // KingsOrderManager._instance.flames.GetComponent<Renderer>().sortingOrder=5;

        //KingsOrderManager._instance.Hide();
        gameObject.SetActive(false);
    }

    


}
