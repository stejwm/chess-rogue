using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BloodOffering", menuName = "Abilities/BloodOffering")]
public class BloodOffering : Ability
{
    private Chessman piece;
    
    public BloodOffering() : base("Blood Offering", "+1 to all stats when a friendly piece is captured") {}

    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnPieceCaptured.AddListener(AddBonus);
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnPieceCaptured.RemoveListener(AddBonus); 

    }
    public void AddBonus(Chessman attacker, Chessman defender){
        if(!piece){
            eventHub.OnPieceCaptured.RemoveListener(AddBonus); 
            return;
        }
        if (defender.color==piece.color){
            //piece.effectsFeedback.PlayFeedbacks();
            board.AbilityLogger.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Blood offering</gradient></color>"+ $"<color=green>+1</color> all stats on {BoardPosition.ConvertToChessNotation(piece.xBoard, piece.yBoard)}");
            piece.AddBonus(StatType.Attack, 1, abilityName);
            piece.AddBonus(StatType.Defense, 1, abilityName);
            piece.AddBonus(StatType.Support, 1, abilityName);
        }
    }

}
