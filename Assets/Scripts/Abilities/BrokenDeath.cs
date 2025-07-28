using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BrokenDeath", menuName = "Abilities/BrokenDeath")]
public class BrokenDeath : Ability
{
    private Chessman piece;
    
    public BrokenDeath() : base("Broken Death", "Decimates attacker when captured") {}

    public override void Apply(Board board, Chessman piece)
    {
        if(piece.abilities.Contains(this)){
            return;
        }
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnPieceCaptured.AddListener(BreakDeath);
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnPieceCaptured.RemoveListener(BreakDeath); 

    }
    public void BreakDeath(Chessman attacker, Chessman defender){
        if (defender==piece){
            eventHub.OnPieceCaptured.Invoke(defender, attacker);
            attacker.DestroyPiece();
            board.AbilityLogger.AddAbilityLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Broken Death</gradient></color>", $" dragged {attacker.name} to hell with them");
        }
    }

}
