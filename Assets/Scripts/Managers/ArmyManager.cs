using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;
using System.Linq;

public class ArmyManager : MonoBehaviour
{
    public List<GameObject> myPieces;
    public List<GameObject> pieces = new List<GameObject>();
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
        if(piece==null)
            return;
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
        else if (selectedPiece && board.Hero.playerCoins>=pricePerPiece*2 && !board.Hero.inventoryPieces.Contains(piece.gameObject) && !board.Hero.inventoryPieces.Contains(selectedPiece.gameObject)){
            Tile position1 = selectedPiece.startingPosition;
            Tile position2 = piece.startingPosition;
            selectedPiece.startingPosition=position2;
            piece.startingPosition=position1;
            selectedPiece.xBoard=position2.X;
            selectedPiece.yBoard=position2.Y;
            piece.xBoard=position1.X;
            piece.yBoard=position1.Y;
            board.PlacePiece(selectedPiece, board.GetTileAt(selectedPiece.xBoard, selectedPiece.yBoard));
            board.PlacePiece(piece, board.GetTileAt(piece.xBoard, piece.yBoard));
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
            board.PlacePiece(selectedPiece, position);
            selectedPiece.startingPosition=position;
            SpriteRenderer rend = selectedPiece.GetComponent<SpriteRenderer>();
            selectedPiece.flames.GetComponent<Renderer>().sortingOrder = 2;
            rend.sortingOrder = 1;
            board.EventHub.RaisePieceAdded(selectedPiece);
            DeselectPiece(selectedPiece);
        }else if (selectedPiece && board.Hero.playerCoins>=pricePerPiece){
            selectedPiece.owner.openPositions.Add(selectedPiece.startingPosition);
            selectedPiece.owner.openPositions.Remove(position);
            selectedPiece.startingPosition=position;
            board.ClearPosition(selectedPiece.xBoard, selectedPiece.yBoard);
            board.PlacePiece(selectedPiece, position);
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
        this.gameObject.SetActive(true);
        foreach (var piece in board.Hero.inventoryPieces)
        {
            piece.SetActive(true);
            board.PlacePiece(piece.GetComponent<Chessman>(), board.GetTileAt(index+2, 6));
            index++;
        }
        /* cards = CardFactory.Instance.CreateCards(board.Hero.orders.Where(x => x.canBeUsedFromManagement).ToList());
        index = 0;
        foreach (var card in cards)
        {
            Vector3 localPosition = new(index * 2 - ((1.96f * 2) / cards.Count), 2, -2);
            card.transform.position = localPosition;
            index++;
            StartCoroutine(card.GetComponent<Card>().CardHovered());
        } */
        
    }

    public bool CloseManagement()
    {
        DeselectPiece(selectedPiece);
        if (board.Hero.inventoryPieces.Count > 0)
        {
            foreach (var piece in board.Hero.inventoryPieces)
            {
                piece.GetComponent<MMSpringPosition>().BumpRandom();
            }
            return false;
        }
        else
        {
            gameObject.SetActive(false);
            return true;
        }    
    }

    


}
