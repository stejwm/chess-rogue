using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StrengthReaper", menuName = "Abilities/StrengthReaper")]
public class StrengthReaper : Ability
{
    private Chessman piece;
    private int bonus;
    
    public StrengthReaper() : base("Strength Reaper", "Reduces attacks by half when defending") {}


    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnAttackStart.AddListener(AddBonus);
        Game._instance.OnAttackEnd.AddListener(RemoveBonus);
        base.Apply(piece);

        
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnAttackStart.RemoveListener(AddBonus); 
        Game._instance.OnAttackEnd.RemoveListener(RemoveBonus); 

    }
    public void AddBonus(Chessman attacker, Chessman defender){
        if(piece==defender){
            bonus = attacker.CalculateAttack()/2;
            attacker.attackBonus-= bonus;
             piece.effectsFeedback.PlayFeedbacks();
            AbilityLogger._instance.LogAbilityUsage($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Strength Reaper</gradient></color>", $" attack reduced by <color=red>-{bonus}</red>");
        
        }
    }
    public void RemoveBonus(Chessman attacker, Chessman defender, int support, int defenseSupport){
        if (defender==piece)
            attacker.attackBonus+=bonus;
    }

}
