using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rand= System.Random;

[CreateAssetMenu(fileName = "Merchant", menuName = "Abilities/Merchant")]
public class Merchant : Ability
{
    private static Rand rng = new Rand();
    private Chessman piece;
    int attackBonus=0;
    
    public Merchant() : base("Merchant", "10% chance of +1 attack per coin owned") {}


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
    public void AddBonus(Chessman attacker, int support, bool isAttacking, BoardPosition targetedPosition){
        if (attacker==piece && isAttacking){
            for (int i =0; i<piece.owner.playerCoins; i++){
                if (rng.Next(1,11)<=1){
                    attackBonus+=1;
                }
            }
            piece.attackBonus+=attackBonus;
        }
    }
    public void RemoveBonus(Chessman attacker, Chessman defender, int support, int defenseSupport){
        if (attacker==piece){
            piece.attackBonus-=attackBonus;
            attackBonus=0;
        }
    }

}
