using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BloodOffering", menuName = "Abilities/BloodOffering")]
public class BloodOffering : Ability
{
    private Chessman piece;
    
    public BloodOffering() : base("Blood Offering", "+1 to all stats when a friendly piece is captured") {}

    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnPieceCaptured.AddListener(AddBonus);
        piece.releaseCost+=15;
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnPieceCaptured.RemoveListener(AddBonus); 

    }
    public void AddBonus(Chessman attacker, Chessman defender){
        if(!piece){
            Game._instance.OnPieceCaptured.RemoveListener(AddBonus); 
            return;
        }
        if (defender.color==piece.color){
            piece.effectsFeedback.PlayFeedbacks();
            piece.attackBonus+=1;
            piece.defenseBonus+=1;
            piece.supportBonus+=1;
        }
    }

}
