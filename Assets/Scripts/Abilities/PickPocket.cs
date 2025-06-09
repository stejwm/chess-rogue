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
        GameManager._instance.OnPieceBounced.AddListener(Steal);
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        GameManager._instance.OnPieceBounced.RemoveListener(Steal); 

    }
    public void Steal(Chessman attacker, Chessman defender, bool isBounceReduced){
        if(attacker==piece){
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">PickPocket</gradient></color>", $"<color=yellow>+3</color> coins");
            piece.owner.playerCoins+=2;
        }
    }

}