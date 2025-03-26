using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PickPocket", menuName = "Abilities/PickPocket")]
public class PickPocket : Ability
{
    private Chessman piece;
    
    public PickPocket() : base("Pickpocket", "+3 coins when bounced") {}

    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnPieceBounced.AddListener(Steal);
        piece.releaseCost+=Cost;
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnPieceBounced.RemoveListener(Steal); 

    }
    public void Steal(Chessman attacker, Chessman defender, bool isBounceReduced){
        if(attacker==piece){
            piece.effectsFeedback.PlayFeedbacks();
            piece.owner.playerCoins+=2;
        }
    }

}