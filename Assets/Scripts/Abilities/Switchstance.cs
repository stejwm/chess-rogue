using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName = "Switchstance", menuName = "Abilities/Switchstance")]
public class Switchstance : Ability
{
    private Chessman piece;
    
    public Switchstance() : base("Switchstance", "Swap attack and defense values on every combat (Attacking or Defending)") {}


    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnAttackEnd.AddListener(Swap);
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnAttackEnd.RemoveListener(Swap); 

    }
    public void Swap(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport){
        string SwitchTag = $" ({this.name})";
        if (defender == piece || attacker == piece)
        {
            int attack = piece.attack;
            int defense = piece.defense;
            int bonusAttack = piece.attackBonus;
            int bonusDefense = piece.defenseBonus;
            piece.attack = defense;
            piece.defense = attack;
            piece.attackBonus = bonusDefense;
            piece.defenseBonus = bonusAttack;

            var SwappedAttackDict = new Dictionary<string, int>();
            foreach (var entry in piece.AttackBonuses)
            {
                if (entry.Key.EndsWith(SwitchTag))
                    SwappedAttackDict[entry.Key.EndsWith(SwitchTag) ? entry.Key[..^SwitchTag.Length] : entry.Key] = entry.Value;
                else
                    SwappedAttackDict[entry.Key.EndsWith(SwitchTag) ? entry.Key : entry.Key + SwitchTag] = entry.Value;
            }

            var SwappedDefenseDict = new Dictionary<string, int>();
            foreach (var entry in piece.AttackBonuses)
            {
                if (entry.Key.EndsWith(SwitchTag))
                    SwappedDefenseDict[entry.Key.EndsWith(SwitchTag) ? entry.Key[..^SwitchTag.Length] : entry.Key] = entry.Value;
                else
                    SwappedDefenseDict[entry.Key.EndsWith(SwitchTag) ? entry.Key : entry.Key + SwitchTag] = entry.Value;
            }
            piece.AttackBonuses = SwappedAttackDict;
            piece.DefenseBonuses = SwappedDefenseDict;


            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Switchstance</gradient></color>", $"Defense set to {piece.CalculateDefense()} Attack set to {piece.CalculateAttack()}");
        }
    }

}
