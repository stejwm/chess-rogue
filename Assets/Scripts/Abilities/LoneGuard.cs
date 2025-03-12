using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LoneGuard", menuName = "Abilities/LoneGuard")]
public class LoneGuard : Ability
{
    private Chessman piece;
    
    public LoneGuard() : base("Lone Guard", "+5 defense if defends with no support") {}


    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnAttack.AddListener(AddBonus);
        Game._instance.OnAttackEnd.AddListener(RemoveBonus);
        piece.releaseCost+=20;
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnAttack.RemoveListener(AddBonus); 

    }
    public void AddBonus(Chessman cm, int support, bool isAttacking, BoardPosition targetedPosition){
        if (cm==piece && support==0 && !isAttacking){
            piece.effectsFeedback.PlayFeedbacks();
            piece.defenseBonus+=5;
        }
    }
    public void RemoveBonus(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport){
        if (defender==piece && defenseSupport==0)
            piece.defenseBonus-=5;
    }

}
