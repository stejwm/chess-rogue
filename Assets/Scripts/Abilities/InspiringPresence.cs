using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InspiringPresence", menuName = "Abilities/InspiringPresence")]
public class InspiringPresence : Ability
{
    private Chessman piece;
    
    public InspiringPresence() : base("Inspiring Presence", "Permanently gain +1 to support every time you successfully support a defending piece") {}

    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnSupportAdded.AddListener(CheckForResult);
        piece.releaseCost+=10;
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnSupportAdded.RemoveListener(CheckForResult); 

    }
    public void CheckForResult(Chessman supporter, Chessman attacker, Chessman defender){
        if(supporter==piece){
            Game._instance.OnPieceBounced.AddListener(IsBounce);
        }
    }

    /* public void IsCapture(Chessman attacker){
        if(attacker.team==piece.team)
            AddBonus();

        Game._instance.OnPieceCaptured.RemoveListener(IsCapture);
    } */
    public void IsBounce(Chessman attacker, Chessman defender, bool isBounceReduced){
        if(defender.team==piece.team){
            piece.effectsFeedback.PlayFeedbacks();
            AddBonus();
        }

        Game._instance.OnPieceBounced.RemoveListener(IsBounce);
    }
    public void AddBonus(){
            piece.support+=1;
    }

}