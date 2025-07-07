using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "RoyalFavor", menuName = "KingsOrders/RoyalFavor")]
public class RoyalFavor : KingsOrder
{
    public RoyalFavor() : base("Royal Favor", "One piece permanently gains +1 to all stats") {}

    public override IEnumerator Use(Board board)
    {
        Player hero = board.Hero;
        yield return new WaitUntil(() => board.selectedPosition !=null);
        Tile targetPosition = board.selectedPosition;
        board.selectedPosition=null;
        var Chessobj = board.GetPieceAtPosition(targetPosition.X, targetPosition.Y);
        if(Chessobj==null){
            Debug.Log("No piece at possition");
            yield break;
        }
        Chessman piece = Chessobj.GetComponent<Chessman>();
        
        piece.attack++;
        piece.defense++;
        piece.support++;  
        yield return null; 
    }


}
