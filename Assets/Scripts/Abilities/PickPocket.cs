using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PickPocket", menuName = "Abilities/PickPocket")]
public class PickPocket : Ability
{
    private Chessman piece;
    
    public PickPocket() : base("Pickpocket", "+3 coins when bounced") {}

    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnPieceBounced.AddListener(Steal);
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnPieceBounced.RemoveListener(Steal); 

    }
    public void Steal(Chessman attacker, Chessman defender){
        if(attacker==piece){
            board.AbilityLogger.AddAbilityLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">PickPocket</gradient></color>", $"<color=yellow>+2</color> coins");
            piece.owner.playerCoins+=2;
        }
    }

}