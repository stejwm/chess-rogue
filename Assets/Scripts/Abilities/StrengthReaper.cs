using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StrengthReaper", menuName = "Abilities/StrengthReaper")]
public class StrengthReaper : Ability
{
    private Chessman piece;
    private int bonus;
    
    public StrengthReaper() : base("Strength Reaper", "Adds half of attackers attack to defense when defending") {}


    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnAttack.AddListener(AddBonus);
        Game._instance.OnAttackEnd.AddListener(RemoveBonus);
        piece.releaseCost+=20;
        base.Apply(piece);

        
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnAttack.RemoveListener(AddBonus); 
        Game._instance.OnAttackEnd.RemoveListener(RemoveBonus); 

    }
    public void AddBonus(Chessman attacker, int support, bool isAttacking, BoardPosition targetedPosition){
        if(piece!=attacker && isAttacking){
            bonus=attacker.CalculateAttack()/2;
        }
        if (attacker==piece && !isAttacking){
            piece.effectsFeedback.PlayFeedbacks();
            AbilityLogger._instance.LogAbilityUsage($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Strength Reaper</gradient></color>", $"<color=green>+{bonus}</color> defense");
            piece.defenseBonus+=bonus;
        }
    }
    public void RemoveBonus(Chessman attacker, Chessman defender, int support, int defenseSupport){
        if (defender==piece)
            piece.defenseBonus-=bonus;
    }

}
