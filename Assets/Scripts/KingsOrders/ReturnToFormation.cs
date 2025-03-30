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

    public override IEnumerator Use(){
        Player hero = Game._instance.hero;
        foreach (var pieceObj in hero.pieces)
        {   
            Chessman piece = pieceObj.GetComponent<Chessman>();
            Game._instance.currentMatch.SetPositionEmpty(piece.xBoard, piece.yBoard);
            Game._instance.currentMatch.MovePiece(piece, piece.startingPosition.x, piece.startingPosition.y);
        }
        foreach (var pieceObj in Game._instance.currentMatch.black.pieces)
        {   
            Chessman piece = pieceObj.GetComponent<Chessman>();
            Game._instance.currentMatch.SetPositionEmpty(piece.xBoard, piece.yBoard);
            Game._instance.currentMatch.MovePiece(piece, piece.startingPosition.x, piece.startingPosition.y);
        }         
        yield return null;
    }


}
