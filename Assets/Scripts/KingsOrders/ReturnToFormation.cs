using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ReturnToFormation", menuName = "KingsOrders/ReturnToFormation")]
public class ReturnToFormation : KingsOrder
{

    public ReturnToFormation() : base("Return To Formation", "Returns all pieces to their starting positions") {}

    public override IEnumerator Use(Board board){
        Player hero = board.Hero;
        foreach (var pieceObj in hero.pieces)
        {   
            Chessman piece = pieceObj.GetComponent<Chessman>();
            board.CurrentMatch.SetPositionEmpty(piece.xBoard, piece.yBoard);
            board.CurrentMatch.MovePiece(piece, piece.startingPosition.X, piece.startingPosition.Y);
        }
        foreach (var pieceObj in board.CurrentMatch.black.pieces)
        {   
            Chessman piece = pieceObj.GetComponent<Chessman>();
            board.CurrentMatch.SetPositionEmpty(piece.xBoard, piece.yBoard);
            board.CurrentMatch.MovePiece(piece, piece.startingPosition.X, piece.startingPosition.Y);
        }   
        yield return null;
    }


}
