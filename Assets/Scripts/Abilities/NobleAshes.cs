using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NobleAshes", menuName = "Abilities/NobleAshes")]
public class NobleAshes : Ability
{
    private Chessman piece;
    int bonus = 0;

    public NobleAshes() : base("Noble Ashes", "+1 to all stats for each owned ranked (non-pawn) piece below 8") { }

    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnPieceAdded.AddListener(RemoveBonus);
        board.EventHub.OnPieceRemoved.AddListener(AddBonus);
        board.EventHub.OnChessMatchStart.AddListener(ApplyBonus);
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnPieceAdded.RemoveListener(RemoveBonus);
        eventHub.OnPieceRemoved.RemoveListener(AddBonus);
        eventHub.OnChessMatchStart.RemoveListener(ApplyBonus);
    }
    public void ApplyBonus()
    {
        piece.AddBonus(StatType.Attack, bonus, abilityName);
        piece.AddBonus(StatType.Defense, bonus, abilityName);
        piece.AddBonus(StatType.Support, bonus, abilityName);
        board.AbilityLogger.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Noble Ashes</gradient></color>" + $"<color=green>+{bonus}</color> all stats on {BoardPosition.ConvertToChessNotation(piece.xBoard, piece.yBoard)}");
    }
    public void AddBonus(Chessman deadPiece)
    {
        if (deadPiece.color==piece.color && deadPiece.type > PieceType.Pawn)
        {
            bonus++;
            if (board.CurrentMatch != null)
            {
                ApplyBonus();
            }
        }
    }
    public void RemoveBonus(Chessman newPiece){
        if (newPiece.color==piece.color && newPiece.type > PieceType.Pawn){
            bonus--;
            board.AbilityLogger.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Noble Ashes</gradient></color>"+ $"<color=red>-1</color> bonus, thanks {newPiece.name}");
        }
    }

}
