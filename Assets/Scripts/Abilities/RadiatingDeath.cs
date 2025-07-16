using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RadiatingDeath", menuName = "Abilities/RadiatingDeath")]
public class RadiatingDeath : Ability
{
    private Chessman piece;
    
    public RadiatingDeath() : base("Radiating Death", "Reduce all attackers stats by -1 when captured") {}

    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnPieceCaptured.AddListener(RadiateDeath);
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnPieceCaptured.RemoveListener(RadiateDeath); 

    }
    public void RadiateDeath(Chessman attacker, Chessman defender){
        if (defender == piece)
        {
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Radiating Death</gradient></color>", $"<color=red>-1</color> to all stats on {BoardPosition.ConvertToChessNotation(defender.xBoard, defender.yBoard)}");
            attacker.SetBonus(StatType.Attack, Mathf.Max(-attacker.attack, attacker.attackBonus - 1), $"{abilityName} ({piece.name})");
            attacker.SetBonus(StatType.Defense, Mathf.Max(-attacker.defense, attacker.defenseBonus - 1), $"{abilityName} ({piece.name})");
            attacker.SetBonus(StatType.Support, Mathf.Max(-attacker.support, attacker.supportBonus - 1), $"{abilityName} ({piece.name})");
        }
    }

}
