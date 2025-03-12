using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Vampire", menuName = "Abilities/Vampire")]
public class Vampire : Ability
{
    private Chessman piece;
    private int bonus = 0;

    public Vampire() : base("Vampire", "+1 to all stats on dark squares, -1 to all stats on light squares. Ability is added to any piece that is attacked") {}

    public override void Apply(Chessman piece)
    {
        if(piece.abilities.Contains(this)){
            return;
        }
        this.piece = piece;
        
        Game._instance.OnMove.AddListener(AddBonus);
        Game._instance.OnAttackEnd.AddListener(SuckBlood);
        Game._instance.OnChessMatchStart.AddListener(MatchStartBonus);
        Game._instance.OnPieceBounced.AddListener(PossibleReset);
        piece.releaseCost += 15;
        if(Game._instance.state==ScreenState.ActiveMatch){
            Debug.Log("Applying vamp live");
            AddBonus(piece, new BoardPosition(piece.xBoard, piece.yBoard));
        }
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnMove.RemoveListener(AddBonus);
        Game._instance.OnAttackEnd.RemoveListener(SuckBlood);
        Game._instance.OnChessMatchStart.RemoveListener(MatchStartBonus);
        Game._instance.OnPieceBounced.RemoveListener(PossibleReset);
    }
    public void MatchStartBonus(){
        Debug.Log("Applying vamp at match start");
        AddBonus(piece, piece.startingPosition);
    }
    public void AddBonus(Chessman mover, BoardPosition targetPosition)
    {
        Debug.Log("add bonus vamp position: " + targetPosition.x + ", " + targetPosition.y);
        int currentBonus = bonus;
        if (mover == piece)
        {
            if (BoardManager._instance.GetTileAt(targetPosition.x, targetPosition.y).GetColor() == PieceColor.Black)
            {
                bonus = 1;
            }
            else
            {
                bonus = -1;
            }

            if (currentBonus != bonus && currentBonus != 0)
            {
                AdjustBonus(piece, bonus * 2);
            }
            else if (currentBonus == 0)
            {
                AdjustBonus(piece, bonus);
            }
        }
    }

    private void AdjustBonus(Chessman piece, int bonusChange)
    {
        Debug.Log("Bonus change: " + bonusChange);
        Debug.Log("attackBonus: " + piece.attackBonus);
        Debug.Log("new attackBonus: " + Mathf.Max(-piece.attack, piece.attackBonus+bonusChange));
        piece.attackBonus = Mathf.Max(-piece.attack, piece.attackBonus+bonusChange);
        piece.defenseBonus = Mathf.Max(-piece.defense, piece.defenseBonus + bonusChange);
        piece.supportBonus = Mathf.Max(-piece.support, piece.supportBonus + bonusChange);
    }

    public void SuckBlood(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport)
    {
        if (attacker == piece)
        {
            defender.AddAbility(Game._instance.AllAbilities[20].Clone());
        }
    }
    public void PossibleReset(Chessman attacker, Chessman defender, bool isBounceReduced)
    {
        if (attacker == piece)
        {
            AddBonus(piece, new BoardPosition(piece.xBoard, piece.yBoard));
        }
    }
}
