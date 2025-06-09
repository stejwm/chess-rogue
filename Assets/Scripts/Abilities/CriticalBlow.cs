using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rand= System.Random;

[CreateAssetMenu(fileName = "CriticalBlow", menuName = "Abilities/CriticalBlow")]
public class CriticalBlow : Ability
{
    private static Rand rng = new Rand();
    private Chessman piece;
    int attackBonus=0;
    
    public CriticalBlow() : base("Critical Blow", "10% chance of x2 attack") {}


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
        if (attacker==piece && isAttacking){
            if (rng.Next(1,11)<=1){
                attackBonus+= piece.CalculateAttack();
                piece.effectsFeedback.PlayFeedbacks();
                AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Critical Blow</gradient></color>",  " x2");
            }
            piece.attackBonus+=attackBonus;
        }
    }
    public void RemoveBonus(Chessman attacker, Chessman defender, int support, int defenseSupport){
        if (attacker==piece){
            piece.attackBonus-=attackBonus;
            attackBonus=0;
        }
    }

}
