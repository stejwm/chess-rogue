using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InspiringPresence", menuName = "Abilities/InspiringPresence")]
public class InspiringPresence : Ability
{
    private Chessman piece;
    
    public InspiringPresence() : base("Inspiring Presence", "Permanently gain +1 to support for every successful support on a defending piece") {}

    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnSupportAdded.AddListener(CheckForResult);
        piece.releaseCost+=10;
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnSupportAdded.RemoveListener(CheckForResult); 

    }
    public void CheckForResult(Chessman supporter, Chessman attacker, Chessman defender){
        if(supporter==piece){
            Game._instance.OnPieceBounced.AddListener(IsBounce);
            Game._instance.OnPieceCaptured.AddListener(RemoveListener);
            Debug.Log("Inspiring Presence Listening for Bounce");
        }
    }

    public void RemoveListener(Chessman attacker, Chessman defender){
        Game._instance.OnPieceBounced.RemoveListener(IsBounce);
    }
    public void IsBounce(Chessman attacker, Chessman defender, bool isBounceReduced){
        if(defender.team==piece.team){
            piece.effectsFeedback.PlayFeedbacks();
            piece.support+=1;
            Debug.Log("Bounced, bonus added");
        }
        Debug.Log("Removing Bounce Listener");
        Game._instance.OnPieceBounced.RemoveListener(IsBounce);
        Game._instance.OnPieceCaptured.RemoveListener(RemoveListener);
    }

}