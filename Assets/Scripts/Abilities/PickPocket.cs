using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PickPocket", menuName = "Abilities/PickPocket")]
public class PickPocket : Ability
{
    private Chessman piece;
    
    public PickPocket() : base("Pickpocket", "Get +2 coins when this piece is bounced") {}

    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnPieceBounced.AddListener(Steal);
        piece.releaseCost+=10;
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnPieceBounced.RemoveListener(Steal); 

    }

    /* public void IsCapture(Chessman attacker){
        if(attacker.team==piece.team)
            AddBonus();

        Game._instance.OnPieceCaptured.RemoveListener(IsCapture);
    } */
    public void Steal(Chessman attacker, Chessman defender, bool isBounceReduced){
        if(attacker==piece){
            piece.effectsFeedback.PlayFeedbacks();
            piece.owner.playerCoins+=2;
        }
    }

}