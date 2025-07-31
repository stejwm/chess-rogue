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
        board.EventHub.RaisePieceRemoved(cm);
        cm.DestroyPiece();
        board.Hero.AbandonedPieces++;

        //TODO: Check weights do not drop below zero
        if (board.Hero.RarityWeights[Rarity.Common] <= 12)
        {
            board.Hero.RarityWeights[Rarity.Uncommon] -= 6;
            board.Hero.RarityWeights[Rarity.Rare] += 6;
        }
        else if (board.Hero.RarityWeights[Rarity.Uncommon] <= 20)
        {

        }
        else
        {
            board.Hero.RarityWeights[Rarity.Uncommon] += 6;
            board.Hero.RarityWeights[Rarity.Rare] += 6;
            board.Hero.RarityWeights[Rarity.Common] -= 12;
        }
        

        yield return null;
    }

}
