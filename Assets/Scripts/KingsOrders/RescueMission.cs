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
        Player hero = GameManager._instance.hero;
        GameManager._instance.tileSelect=true;
        bool gotSomething=false;
        foreach (var piece in GameManager._instance.currentMatch.black.capturedPieces)
        {
            if(Board._instance.GetTileAt(piece.GetComponent<Chessman>().xBoard, piece.GetComponent<Chessman>().yBoard).CurrentPiece==null){
                piece.SetActive(true);
                piece.GetComponent<SpriteRenderer>().color=Color.red;
                gotSomething=true;
            }
            GameManager._instance.togglePieceColliders(GameManager._instance.currentMatch.black.capturedPieces, false);
        }
        if(!gotSomething){
            GameManager._instance.currentMatch.SetPiecesValidForAttack(hero);
            GameManager._instance.tileSelect=false;
            yield break;
        }
        yield return new WaitUntil(() => Board._instance.selectedPosition !=null);
        GameManager._instance.tileSelect=false;
        BoardPosition targetPosition = Board._instance.selectedPosition;
        Board._instance.selectedPosition= null;
        GameObject rescuedPiece = null;
        foreach (var piece in GameManager._instance.currentMatch.black.capturedPieces)
        {
            if(targetPosition.x == piece.GetComponent<Chessman>().xBoard && targetPosition.y == piece.GetComponent<Chessman>().yBoard){
                rescuedPiece = piece;
                piece.GetComponent<Chessman>().GetComponent<SpriteRenderer>().color=Color.white;

            }else{
                piece.GetComponent<Chessman>().GetComponent<SpriteRenderer>().color=Color.white;
                piece.SetActive(false);
            }
        }
        GameManager._instance.currentMatch.MovePiece(rescuedPiece.GetComponent<Chessman>(), targetPosition.x, targetPosition.y);
        
        GameManager._instance.currentMatch.black.capturedPieces.Remove(rescuedPiece);
        GameManager._instance.hero.pieces.Add(rescuedPiece);
        GameManager._instance.togglePieceColliders(GameManager._instance.currentMatch.black.capturedPieces, true);

        
        //Game._instance.togglePieceColliders(civilians, false);
        GameManager._instance.currentMatch.SetPiecesValidForAttack(hero);
    }

}
