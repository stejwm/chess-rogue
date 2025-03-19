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

    public override IEnumerator Use(){
        Player hero = Game._instance.hero;
        Game._instance.tileSelect=true;
        yield return new WaitUntil(() => BoardManager._instance.selectedPosition !=null);
        Game._instance.tileSelect=false;
        BoardPosition targetPosition = BoardManager._instance.selectedPosition;
        BoardManager._instance.selectedPosition=null;
        var Chessobj = Game._instance.currentMatch.GetPieceAtPosition(targetPosition.x, targetPosition.y);
        if(Chessobj==null){
            Debug.Log("No piece at possition");
            yield break;
        }
        Chessman piece = Chessobj.GetComponent<Chessman>();
        
        piece.attack++;
        piece.defense++;
        piece.support++;     

    }


}
