using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vampire", menuName = "Abilities/Vampire")]
public class Vampire : Ability
{
    private Chessman piece;
    private int bonus=0;
    
    public Vampire() : base("Vampire", "+1 to all stats on dark squares, -1 to all stats on light squares. Ability is added to any piece that is attacked") {}


    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnMove.AddListener(AddBonus);
        Game._instance.OnAttackEnd.AddListener(SuckBlood);
        piece.releaseCost+=15;
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnMove.RemoveListener(AddBonus); 

    }
    public void AddBonus(Chessman mover, BoardPosition targetPosition){
        int currentBonus=bonus;
        if (mover==piece){
            Debug.Log("Vamp position:" +piece.xBoard + piece.yBoard);
            if(BoardManager._instance.GetTileAt(targetPosition.x, targetPosition.y).GetColor()==PieceColor.Black){
                bonus=1;
            }else{
                bonus=-1;
            }
            if(currentBonus!=bonus && currentBonus!=0){
                piece.attackBonus+=bonus*2;
                piece.defenseBonus+=bonus*2;
                piece.supportBonus+=bonus*2;
            }
            else if(currentBonus==0){
                piece.attackBonus+=bonus;
                piece.defenseBonus+=bonus;
                piece.supportBonus+=bonus;
            }
        }
    }

    public void SuckBlood(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport){
        if(attacker==piece){
            defender.AddAbility(Game._instance.AllAbilities[20].Clone());
        }
    }

}
