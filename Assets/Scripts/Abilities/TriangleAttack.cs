using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rand= System.Random;

[CreateAssetMenu(fileName = "TriangleAttack", menuName = "Abilities/TriangleAttack")]
public class TriangleAttack : Ability
{
    private static Rand rng = new Rand();
    private Chessman piece;
    int attackBonus=0;
    int matchingSupporters = 0;
    
    public TriangleAttack() : base("Triangle Attack", "x3 attack if supported by 2 of the same piece types") { }


    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        board.EventHub.OnAttack.AddListener(AddBonus);
        board.EventHub.OnSupportAdded.AddListener(MarkSupporters);
        board.EventHub.OnAttackEnd.AddListener(RemoveBonus);
        base.Apply(board, piece);

    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnAttack.RemoveListener(AddBonus);
        eventHub.OnAttackEnd.RemoveListener(RemoveBonus);

    }

    public void MarkSupporters(Chessman attacker, Chessman defender, Chessman supporter)
    {
        if (attacker == piece)
        {
            if (supporter.color == piece.color && supporter.type == piece.type)
            {
                matchingSupporters++;
            }
        }
    }
    public void AddBonus(Chessman attacker, int support, Tile targetedPosition)
    {
        if (attacker == piece && matchingSupporters>=2)
        {
            attackBonus += piece.CalculateAttack()*2;
            //piece.effectsFeedback.PlayFeedbacks();
            board.AbilityLogger.AddAbilityLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Triangle Attack</gradient></color>", " x3 Attack");
            piece.AddBonus(StatType.Attack, attackBonus, abilityName);
        }
    }
    public void RemoveBonus(Chessman attacker, Chessman defender, int support, int defenseSupport){
        if (attacker == piece)
        {
            piece.RemoveBonus(StatType.Attack, attackBonus, abilityName);
            attackBonus = 0;
            matchingSupporters = 0;
        }
    }

}
