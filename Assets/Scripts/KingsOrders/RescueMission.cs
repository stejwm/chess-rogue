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
        Player hero = board.Hero;
        bool gotSomething=false;
        foreach (var piece in board.CurrentMatch.black.capturedPieces)
        {
            if(board.GetPieceAtPosition(piece.GetComponent<Chessman>().xBoard, piece.GetComponent<Chessman>().yBoard)==null){
                piece.SetActive(true);
                piece.GetComponent<SpriteRenderer>().color=Color.red;
                gotSomething=true;
            }
        }
        if(!gotSomething){
            board.CurrentMatch.SetPiecesValidForAttack(hero);
            yield break;
        }
        yield return new WaitUntil(() => board.selectedPosition !=null);
        Tile targetPosition = board.selectedPosition;
        board.selectedPosition= null;
        GameObject rescuedPiece = null;
        foreach (var piece in board.CurrentMatch.black.capturedPieces)
        {
            if(targetPosition.X == piece.GetComponent<Chessman>().xBoard && targetPosition.Y == piece.GetComponent<Chessman>().yBoard){
                rescuedPiece = piece;
                piece.GetComponent<Chessman>().GetComponent<SpriteRenderer>().color=Color.white;

            }else{
                piece.GetComponent<Chessman>().GetComponent<SpriteRenderer>().color=Color.white;
                piece.SetActive(false);
            }
        }
        board.CurrentMatch.MovePiece(rescuedPiece.GetComponent<Chessman>(), targetPosition.X, targetPosition.Y);

        board.CurrentMatch.black.capturedPieces.Remove(rescuedPiece);
        board.Hero.pieces.Add(rescuedPiece);
        board.CurrentMatch.SetPiecesValidForAttack(hero);
        yield return null;
    }

}
