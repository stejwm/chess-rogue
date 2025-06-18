using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IncreasingStrike", menuName = "Abilities/IncreasingStrike")]
public class IncreasingStrike : Ability
{
    private Chessman piece;
    
    public IncreasingStrike() : base("Increasing Strike", "Permanently gain +1 to attack for every capture") {}

    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnPieceCaptured.AddListener(AddBonus);
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnPieceCaptured.RemoveListener(AddBonus); 

    }
    public void AddBonus(Chessman attacker, Chessman defender){
        if (attacker==piece){
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Increasing Strike</gradient></color>", "I want more...");

            piece.attack+=1;
        }
    }

}
