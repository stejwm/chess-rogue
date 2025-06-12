using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "RescueMission", menuName = "KingsOrders/RescueMission")]
public class RescueMission : KingsOrder
{
    public RescueMission() : base("Rescue Mission", "Place a captured piece on the board") {}

    public override IEnumerator Use(Board board)
    {
        /*  Player hero = board.Hero;
         GameManager._instance.tileSelect=true;
         bool gotSomething=false;
         foreach (var piece in board.CurrentMatch.black.capturedPieces)
         {
             if(board.GetTileAt(piece.GetComponent<Chessman>().xBoard, piece.GetComponent<Chessman>().yBoard).CurrentPiece==null){
                 piece.SetActive(true);
                 piece.GetComponent<SpriteRenderer>().color=Color.red;
                 gotSomething=true;
             }
             GameManager._instance.togglePieceColliders(board.CurrentMatch.black.capturedPieces, false);
         }
         if(!gotSomething){
             board.CurrentMatch.SetPiecesValidForAttack(hero);
             GameManager._instance.tileSelect=false;
             yield break;
         }
         yield return new WaitUntil(() => board.selectedPosition !=null);
         GameManager._instance.tileSelect=false;
         BoardPosition targetPosition = board.selectedPosition;
         board.selectedPosition= null;
         GameObject rescuedPiece = null;
         foreach (var piece in board.CurrentMatch.black.capturedPieces)
         {
             if(targetPosition.x == piece.GetComponent<Chessman>().xBoard && targetPosition.y == piece.GetComponent<Chessman>().yBoard){
                 rescuedPiece = piece;
                 piece.GetComponent<Chessman>().GetComponent<SpriteRenderer>().color=Color.white;

             }else{
                 piece.GetComponent<Chessman>().GetComponent<SpriteRenderer>().color=Color.white;
                 piece.SetActive(false);
             }
         }
         board.CurrentMatch.MovePiece(rescuedPiece.GetComponent<Chessman>(), targetPosition.x, targetPosition.y);

         board.CurrentMatch.black.capturedPieces.Remove(rescuedPiece);
         board.Hero.pieces.Add(rescuedPiece);
         //GameManager._instance.togglePieceColliders(board.CurrentMatch.black.capturedPieces, true);


         //Game._instance.togglePieceColliders(civilians, false);
         board.CurrentMatch.SetPiecesValidForAttack(hero); */
        yield return null;
    }

}
