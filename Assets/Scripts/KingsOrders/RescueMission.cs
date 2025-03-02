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

    public override IEnumerator Use(){
        Player hero = Game._instance.hero;
        Game._instance.tileSelect=true;
        bool gotSomething=false;
        foreach (var piece in Game._instance.currentMatch.black.capturedPieces)
        {
            if(BoardManager._instance.GetTileAt(piece.GetComponent<Chessman>().xBoard, piece.GetComponent<Chessman>().yBoard).getPiece()==null){
                piece.SetActive(true);
                piece.GetComponent<SpriteRenderer>().color=Color.red;
                gotSomething=true;
            }
            Game._instance.togglePieceColliders(Game._instance.currentMatch.black.capturedPieces, false);
        }
        if(!gotSomething){
            Debug.Log("breaking");
            Game._instance.currentMatch.SetPiecesValidForAttack(hero);
            Game._instance.tileSelect=false;
            yield break;
        }
        yield return new WaitUntil(() => BoardManager._instance.selectedPosition !=null);
        Game._instance.tileSelect=false;
        BoardPosition targetPosition = BoardManager._instance.selectedPosition;
        BoardManager._instance.selectedPosition= null;
        GameObject rescuedPiece = null;
        foreach (var piece in Game._instance.currentMatch.black.capturedPieces)
        {
            if(targetPosition.x == piece.GetComponent<Chessman>().xBoard && targetPosition.y == piece.GetComponent<Chessman>().yBoard){
                rescuedPiece = piece;
                piece.GetComponent<Chessman>().GetComponent<SpriteRenderer>().color=Color.white;

            }else{
                piece.GetComponent<Chessman>().GetComponent<SpriteRenderer>().color=Color.white;
                piece.SetActive(false);
            }
        }
        Game._instance.currentMatch.MovePiece(rescuedPiece.GetComponent<Chessman>(), targetPosition.x, targetPosition.y);
        
        Game._instance.currentMatch.black.capturedPieces.Remove(rescuedPiece);
        Game._instance.hero.pieces.Add(rescuedPiece);
        Game._instance.togglePieceColliders(Game._instance.currentMatch.black.capturedPieces, true);

        
        //Game._instance.togglePieceColliders(civilians, false);
        Game._instance.currentMatch.SetPiecesValidForAttack(hero);
    }

}
