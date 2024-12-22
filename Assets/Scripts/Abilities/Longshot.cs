using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Longshot", menuName = "Abilities/Longshot")]
public class Longshot : Ability
{
    private Chessman piece;
    
    public Longshot() : base("Longshot", "+5 attack if piece attacks from 5 or more squares away") {}


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
        if (cm==piece && isAttacking && (!Enumerable.Range(cm.xBoard-5,cm.xBoard+5).Contains(targetedPosition.x) ||!Enumerable.Range(cm.yBoard-5,cm.yBoard+5).Contains(targetedPosition.y)))
            piece.attackBonus+=5;
    }
    public void RemoveBonus(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport){
        if (defender==piece && defenseSupport==0)
            piece.attackBonus-=5;
    }

}
