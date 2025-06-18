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
    public List<GameObject> pieces = new List<GameObject>();
    public List<GameObject> cards = new List<GameObject>();
    public Chessman selectedPiece;
    public int pricePerPiece = 2;
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
            DeselectPiece(selectedPiece);
        }
        else{
            selectedPiece.GetComponent<MMSpringPosition>().BumpRandom();
        }
    }

    public void OpenManagement(Board board)
    {
        this.board = board;
        int index = 0;
        foreach (var piece in board.Hero.inventoryPieces)
        {
            piece.SetActive(true);
            board.PlacePiece(piece.GetComponent<Chessman>(), board.GetTileAt(index, 4));
            index++;
        }
        cards = CardFactory.Instance.CreateCards(board.Hero.orders);
        foreach (var card in cards)
        {
            Vector3 localPosition = new(index * 2 - ((1.96f * 2) / cards.Count), 2, -2);
            card.transform.position = localPosition;
            index++;
            card.GetComponent<Card>().CardHovered();
        }
        this.gameObject.SetActive(true);
    }

    public void CloseManagement()
    {
        foreach (var card in cards)
            Destroy(card);
        foreach (var piece in board.Hero.inventoryPieces)
            piece.SetActive(false);

        gameObject.SetActive(false);   
    }

    


}
