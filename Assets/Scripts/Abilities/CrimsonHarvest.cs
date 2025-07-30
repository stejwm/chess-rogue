using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrimsonHarvest", menuName = "Abilities/CrimsonHarvest")]
public class CrimsonHarvest : Ability
{
    private Chessman piece;
    
    public CrimsonHarvest() : base("Crimson Harvest", "+1 blood to opponents value when bounced") {}

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
            board.AbilityLogger.AddAbilityLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Crimson Harvest</gradient></color>", $"<color=red>let the streets run red</color>");
            piece.owner.playerCoins+=2;
        }
    }

}