using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Longshot", menuName = "Abilities/Longshot")]
public class Longshot : Ability
{
    private Chessman piece;
    private bool bonusAdded;
    
    public Longshot() : base("Longshot", "+5 attack if piece attacks from 4 or more squares away") {}


    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnAttack.AddListener(AddBonus);
        Game._instance.OnAttackEnd.AddListener(RemoveBonus);
        piece.releaseCost+=20;

    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnAttack.RemoveListener(AddBonus); 

    }
    public void AddBonus(Chessman cm, int support, bool isAttacking, BoardPosition targetedPosition){
        //Debug.Log("starting position: "+cm.xBoard+","+cm.yBoard + " attacking position "+targetedPosition.x+","+targetedPosition.y);
        if (cm==piece && isAttacking && (!Enumerable.Range(cm.xBoard-4,8).Contains(targetedPosition.x) ||!Enumerable.Range(cm.yBoard-4,8).Contains(targetedPosition.y))){
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
