using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assassin", menuName = "Abilities/Assassin")]
public class Assassin : Ability
{
    private Chessman piece;
    
    public Assassin() : base("Assassin", "+5 attack if attacking with no support") {}


    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        GameManager._instance.OnAttack.AddListener(AddBonus);
        GameManager._instance.OnAttackEnd.AddListener(RemoveBonus);
        base.Apply(piece);

        
    }

    public override void Remove(Chessman piece)
    {
        GameManager._instance.OnAttack.RemoveListener(AddBonus); 
        GameManager._instance.OnAttackEnd.RemoveListener(RemoveBonus); 

    }
    public void AddBonus(Chessman attacker, int support, bool isAttacking, BoardPosition targetedPosition){
        if (attacker==piece && support==0 && isAttacking){
            piece.effectsFeedback.PlayFeedbacks();
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Assassin</gradient></color>", "<color=green>+5</color> attack");
            piece.attackBonus+=5;
        }
    }
    public void RemoveBonus(Chessman attacker, Chessman defender, int support, int defenseSupport){
        if (attacker==piece && support==0)
            piece.attackBonus-=5;
    }

}
