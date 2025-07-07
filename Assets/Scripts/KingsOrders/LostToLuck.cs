using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "LostToLuck", menuName = "KingsOrders/LostToLuck")]
public class LostToLuck : KingsOrder
{
    public LostToLuck() : base("Lost To Luck", "Abandon a piece in order to increase odds uncommon and rare abilities") {}

    public override IEnumerator Use(Board board)
    {
        Player hero = board.Hero;
        yield return new WaitUntil(() => board.selectedPosition !=null);
        Tile targetPosition = board.selectedPosition;
        board.selectedPosition= null;
        var Chessobj = board.GetPieceAtPosition(targetPosition.X, targetPosition.Y);
        if(Chessobj==null){
            Debug.Log("No piece at possition");
            yield break;
        }
        Chessman cm = Chessobj.GetComponent<Chessman>();
        board.Hero.pieces.Remove(Chessobj);
        board.Hero.openPositions.Add(cm.startingPosition);
        Chessobj.GetComponent<Chessman>().DestroyPiece();
        board.Hero.AbandonedPieces++;
        
        //TODO: Check weights do not drop below zero
        board.Hero.RarityWeights[Rarity.Uncommon] += 5;
        board.Hero.RarityWeights[Rarity.Rare] += 5;
        board.Hero.RarityWeights[Rarity.Common] -= 10;

        

        yield return null;
    }

}
