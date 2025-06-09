using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LoneGuard", menuName = "Abilities/LoneGuard")]
public class LoneGuard : Ability
{
    private Chessman piece;
    
    public LoneGuard() : base("Lone Guard", "+5 defense if defending with no support") {}


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
    public void AddBonus(Chessman cm, int support, bool isAttacking, BoardPosition targetedPosition){
        if (cm==piece && support==0 && !isAttacking){
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Lone Guard</gradient></color>", "<color=green>+5 defense</color>");

            piece.defenseBonus+=5;
        }
    }
    public void RemoveBonus(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport){
        if (defender==piece && defenseSupport==0)
            piece.defenseBonus-=5;
    }

}
