using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LoneGuard", menuName = "Abilities/LoneGuard")]
public class LoneGuard : Ability
{
    private Chessman piece;
    
    public LoneGuard() : base("Lone Guard", "+5 defense if defending with no support") {}


    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnAttackStart.AddListener(CheckDefender);
        board.EventHub.OnAttackEnd.AddListener(RemoveBonus);
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        board.EventHub.OnAttackStart.AddListener(CheckDefender);
        eventHub.OnAttack.RemoveListener(AddBonus); 
        eventHub.OnAttackEnd.RemoveListener(RemoveBonus);

    }
    public void CheckDefender(Chessman attacker, Chessman defender)
    {
        if (defender == piece)
        {
            board.EventHub.OnAttack.AddListener(AddBonus);
        }
    }
    public void AddBonus(Chessman cm, int support, int defenseSupport, Tile targetedPosition)
    {
        if (defenseSupport == 0)
        {
            board.AbilityLogger.AddAbilityLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Lone Guard</gradient></color>", "<color=green>+5 defense</color>");
            piece.AddBonus(StatType.Defense, 5, abilityName);
        }
        eventHub.OnAttack.RemoveListener(AddBonus);
    }
    public void RemoveBonus(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport){
        if (defender==piece && defenseSupport==0)
            piece.RemoveBonus(StatType.Defense, 5, abilityName);
    }

}
