using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Longshot", menuName = "Abilities/Longshot")]
public class Longshot : Ability
{
    private Chessman piece;
    private bool bonusAdded;
    
    public Longshot() : base("Longshot", "+5 attack if attacking from 4 or more squares away") {}


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
    public void AddBonus(Chessman cm, int support, Tile targetedPosition){
        //Debug.Log("starting position: "+cm.xBoard+","+cm.yBoard + " attacking position "+targetedPosition.x+","+targetedPosition.y);
        if (cm==piece && (!Enumerable.Range(cm.xBoard-4,8).Contains(targetedPosition.X) ||!Enumerable.Range(cm.yBoard-4,8).Contains(targetedPosition.Y))){
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Longshot</gradient></color>", "<color=green>+5</color> attack");
            piece.effectsFeedback.PlayFeedbacks();

            piece.AddBonus(StatType.Attack, 5, abilityName);
            bonusAdded =true;
        }
    }
    public void RemoveBonus(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport){
        if (bonusAdded){
            piece.RemoveBonus(StatType.Attack, 5, abilityName);
            bonusAdded=false;
        }
    }

}
