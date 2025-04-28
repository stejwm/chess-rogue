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

    public Vampire() : base("Vampire", "+1 to all stats on dark squares, -1 to all stats on light squares. transfer ability when attacking") {}

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
        if(Game._instance.state==ScreenState.ActiveMatch){
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
        if(bonus!=0){
            Debug.Log("Removing vamp bonus");
            RemoveBonus();
        }
        AddBonus(piece, piece.startingPosition);
    }

    public void AddBonus(Chessman mover, BoardPosition targetPosition)
    {
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
        if(piece == null) return;
        Debug.Log($"piece {piece.name} current stats are {piece.attack},{piece.defense},{piece.support} bonus is {bonus} apply bonus change of {bonusChange}");
        piece.attackBonus = Mathf.Max(-piece.attack, piece.attackBonus+bonusChange);
        piece.defenseBonus = Mathf.Max(-piece.defense, piece.defenseBonus + bonusChange);
        piece.supportBonus = Mathf.Max(-piece.support, piece.supportBonus + bonusChange);
        if (bonusChange > 0)  
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Vampire</gradient></color>",  $"<color=green>+1</color> to all stats on {BoardPosition.ConvertToChessNotation(piece.xBoard, piece.yBoard)}");
        else
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Vampire</gradient></color>",  $"<color=red>-1</color> to all stats on {BoardPosition.ConvertToChessNotation(piece.xBoard, piece.yBoard)}");


    }

    public void SuckBlood(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport)
    {
        if (attacker == piece)
        {
            defender.AddAbility(Game._instance.AllAbilities[20].Clone());
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Vampire</gradient></color>",  $"fledgling created on {BoardPosition.ConvertToChessNotation(defender.xBoard, defender.yBoard)}");

        }
    }
    public void RemoveBonus(){
        piece.attackBonus = Mathf.Max(-piece.attack, piece.attackBonus-bonus);
        piece.defenseBonus = Mathf.Max(-piece.defense, piece.defenseBonus - bonus);
        piece.supportBonus = Mathf.Max(-piece.support, piece.supportBonus - bonus);
        bonus=0;
    }
    public void PossibleReset(Chessman attacker, Chessman defender, bool isBounceReduced)
    {
        if (attacker == piece)
        {
            AddBonus(piece, new BoardPosition(piece.xBoard, piece.yBoard));
        }
    }
}
