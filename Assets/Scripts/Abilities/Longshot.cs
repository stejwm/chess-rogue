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


    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnAttack.AddListener(AddBonus);
        Game._instance.OnAttackEnd.AddListener(RemoveBonus);
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnAttack.RemoveListener(AddBonus); 
        Game._instance.OnAttackEnd.RemoveListener(RemoveBonus);

    }
    public void AddBonus(Chessman cm, int support, bool isAttacking, BoardPosition targetedPosition){
        //Debug.Log("starting position: "+cm.xBoard+","+cm.yBoard + " attacking position "+targetedPosition.x+","+targetedPosition.y);
        if (cm==piece && isAttacking && (!Enumerable.Range(cm.xBoard-4,8).Contains(targetedPosition.x) ||!Enumerable.Range(cm.yBoard-4,8).Contains(targetedPosition.y))){
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Longshot</gradient></color>", "<color=green>+5</color> attack");
            piece.effectsFeedback.PlayFeedbacks();

            piece.attackBonus+=5;
            bonusAdded=true;
        }
    }
    public void RemoveBonus(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport){
        if (bonusAdded){
            piece.attackBonus-=5;
            bonusAdded=false;
        }
    }

}
