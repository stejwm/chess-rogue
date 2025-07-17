using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assassin", menuName = "Abilities/Assassin")]
public class Assassin : Ability
{
    private Chessman piece;
    
    public Assassin() : base("Assassin", "+5 attack if attacking with no support") {}


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
    public void AddBonus(Chessman attacker, int support, Tile targetedPosition){
        if (attacker==piece && support==0){
            piece.effectsFeedback.PlayFeedbacks();
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Assassin</gradient></color>", "<color=green>+5</color> attack");
            piece.AddBonus(StatType.Attack, 5, abilityName);
        }
    }
    public void RemoveBonus(Chessman attacker, Chessman defender, int support, int defenseSupport){
        if (attacker==piece && support==0)
            piece.RemoveBonus(StatType.Attack, 5, abilityName);
    }

}
