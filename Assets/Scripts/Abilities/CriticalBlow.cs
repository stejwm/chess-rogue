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


    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnAttack.AddListener(AddBonus);
        board.EventHub.OnAttackEnd.AddListener(RemoveBonus);
        base.Apply(board, piece);

    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnAttack.RemoveListener(AddBonus);
        eventHub.OnAttackEnd.RemoveListener(RemoveBonus);

    }
    public void AddBonus(Chessman attacker, int support, int defendingSupport, Tile targetedPosition){
        if (attacker == piece)
        {
            if (rng.Next(1, 11) <= 1)
            {
                attackBonus += piece.CalculateAttack();
                piece.effectsFeedback.PlayFeedbacks();
                board.AbilityLogger.AddAbilityLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Critical Blow</gradient></color>", " x2");
            }
            piece.AddBonus(StatType.Attack, attackBonus, abilityName);
        }
    }
    public void RemoveBonus(Chessman attacker, Chessman defender, int support, int defenseSupport){
        if (attacker==piece){
            piece.RemoveBonus(StatType.Attack, attackBonus, abilityName);
            attackBonus=0;
        }
    }

}
