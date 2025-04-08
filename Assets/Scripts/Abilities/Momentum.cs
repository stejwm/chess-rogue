using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Momentum", menuName = "Abilities/Momentum")]
public class Momentum : Ability
{
    private Chessman piece;
    private int bonus;
    public Momentum() : base("Momentum", "Gain +1 to attack per move without an attack, resets after attacking") {}
    private int attackIncrease=0;

    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnRawMoveEnd.AddListener(RawMoveEnd);
        Game._instance.OnAttack.AddListener(Check);
        piece.releaseCost+=Cost;
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnAttack.RemoveListener(Check); 
        Game._instance.OnRawMoveEnd.RemoveListener(RawMoveEnd);

    }
    public void RawMoveEnd(Chessman movedPiece, BoardPosition targetPosition){
        if(movedPiece==piece){
            AddBonus(piece, null);
        }
        else if(movedPiece.color==piece.color && movedPiece!=piece){
            RemoveBonus();
        }
    }

    public void RemoveBonus(){
        piece.attackBonus-=bonus;
        bonus=0;
    }

    public void AddBonus(){
        bonus+=1;
        piece.attackBonus+=bonus;
    }

}
