using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InspiringPresence", menuName = "Abilities/InspiringPresence")]
public class InspiringPresence : Ability
{
    private Chessman piece;
    
    public InspiringPresence() : base("Inspiring Presence", "Permanently gain +1 to support for every successful support on a defending piece") {}

    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnSupportAdded.AddListener(CheckForResult);
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnSupportAdded.RemoveListener(CheckForResult); 

    }
    public void CheckForResult(Chessman attacker, Chessman defender, Chessman supporter){
        if(supporter==piece){
            eventHub.OnPieceBounced.AddListener(IsBounce);
            eventHub.OnPieceCaptured.AddListener(RemoveListener);
            Debug.Log("Inspiring Presence Listening for Bounce");
        }
    }

    public void RemoveListener(Chessman attacker, Chessman defender){
        eventHub.OnPieceBounced.RemoveListener(IsBounce);
    }
    public void IsBounce(Chessman attacker, Chessman defender){
        if(defender.team==piece.team){
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Inspiring Presence</gradient></color>", "happy to help!");
            piece.support+=1;
            Debug.Log("Bounced, bonus added");
        }
        Debug.Log("Removing Bounce Listener");
        eventHub.OnPieceBounced.RemoveListener(IsBounce);
        eventHub.OnPieceCaptured.RemoveListener(RemoveListener);
    }

}