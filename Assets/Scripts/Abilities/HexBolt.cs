using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HexBolt", menuName = "Abilities/HexBolt")]
public class HexBolt : Ability
{
    private Chessman piece;
    
    public HexBolt() : base("Hex Bolt", "When bounced, Removes all abilities from defender until end of their next turn") {}


    public override void Apply(Board board, Chessman piece)
    {
        if(piece.abilities.Contains(this))
            return;
            
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnPieceBounced.AddListener(Hex);
        base.Apply(board, piece);

        
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnPieceBounced.RemoveListener(Hex); 

    }
    public void Hex(Chessman attacker, Chessman defender){
        if (attacker==piece){
            defender.hexed=true;
            foreach (var ability in defender.abilities)
            {
                ability.Remove(defender);
            }
            piece.effectsFeedback.PlayFeedbacks();
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Hex Bolt</gradient></color>", " Hexed");
        }
    }

}
