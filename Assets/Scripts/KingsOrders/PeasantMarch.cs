using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PeasantMarch", menuName = "KingsOrders/PeasantMarch")]
public class PeasantMarch : KingsOrder
{

    public PeasantMarch() : base("Peasant March", "All pawns permanently gain +1 to all stats") {}

    public override IEnumerator Use(){
        Player hero = GameManager._instance.hero;
        foreach (var pieceObj in hero.pieces)
        {   
            Chessman piece = pieceObj.GetComponent<Chessman>();
            if(piece.type==PieceType.Pawn){
                piece.attack++;
                piece.defense++;
                piece.support++;
            }
        }      
        yield return null;
    }


}
