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
        if (attacker==piece){
            for (int i =0; i<piece.owner.playerCoins; i++){
                if (rng.Next(1,11)<=1){
                    attackBonus+=1;
                }
            }
            if (attackBonus>0){
                AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Merchant</gradient></color>", $"<color=green>+{attackBonus} attack from wares</color>");
            }
            piece.AddBonus(StatType.Attack, attackBonus, abilityName);
        }
    }
    public void RemoveBonus(Chessman attacker, Chessman defender, int support, int defenseSupport){
        if (attacker==piece){
            piece.RemoveBonus(StatType.Attack, attackBonus, abilityName);
            attackBonus=0;
        }
    }

}
