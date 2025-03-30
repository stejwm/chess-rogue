using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RedistributionOfPower", menuName = "KingsOrders/RedistributionOfPower")]
public class RedistributionOfPower : KingsOrder
{

    public RedistributionOfPower() : base("Redistribution Of Power", "Sets each stat of all your pieces to average of each stat across all your pieces") {}
    public override IEnumerator Use()
    {
    
        List<GameObject> allPieces = Game._instance.hero.pieces; // Assuming this gets all active pieces

        if (allPieces.Count == 0) yield break;

        // Calculate averages
        float totalAttack = 0, totalDefense = 0, totalSupport = 0;

        foreach (var pieceObj in allPieces)
        {
            Chessman piece = pieceObj.GetComponent<Chessman>();
            totalAttack += piece.attack;
            totalDefense += piece.defense;
            totalSupport += piece.support;
        }

        int avgAttack = Mathf.RoundToInt(totalAttack / allPieces.Count);
        int avgDefense = Mathf.RoundToInt(totalDefense / allPieces.Count);
        int avgSupport = Mathf.RoundToInt(totalSupport / allPieces.Count);

        // Apply the averages
        foreach (var pieceObj in allPieces)
        {
            Chessman piece = pieceObj.GetComponent<Chessman>();
            piece.attack = avgAttack;
            piece.defense = avgDefense;
            piece.support = avgSupport;
        }
        yield break;

    }
}