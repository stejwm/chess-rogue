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
        Player hero = GameManager._instance.hero;
        GameManager._instance.tileSelect=true;
        yield return new WaitUntil(() => Board._instance.selectedPosition !=null);
        GameManager._instance.tileSelect=false;
        BoardPosition targetPosition = Board._instance.selectedPosition;
        Board._instance.selectedPosition=null;
        var Chessobj = GameManager._instance.currentMatch.GetPieceAtPosition(targetPosition.x, targetPosition.y);
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
