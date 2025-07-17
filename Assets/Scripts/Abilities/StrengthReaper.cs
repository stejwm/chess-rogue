using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StrengthReaper", menuName = "Abilities/StrengthReaper")]
public class StrengthReaper : Ability
{
    private Chessman piece;
    private int bonus;
    
    public StrengthReaper() : base("Strength Reaper", "Reduces attacks by half when defending") {}


    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnAttackStart.AddListener(AddBonus);
        board.EventHub.OnAttackEnd.AddListener(RemoveBonus);
        base.Apply(board, piece);

        
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnAttackStart.RemoveListener(AddBonus); 
        eventHub.OnAttackEnd.RemoveListener(RemoveBonus); 

    }
    public void AddBonus(Chessman attacker, Chessman defender){
        if(piece==defender){
            bonus = attacker.CalculateAttack()/2;
            attacker.RemoveBonus(StatType.Attack, bonus, abilityName);
             piece.effectsFeedback.PlayFeedbacks();
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Strength Reaper</gradient></color>", $" attack reduced by <color=red>-{bonus}</color>");
        
        }
    }
    public void RemoveBonus(Chessman attacker, Chessman defender, int support, int defenseSupport){
        if (defender==piece)
            attacker.AddBonus(StatType.Attack, bonus, abilityName);
    }

}
