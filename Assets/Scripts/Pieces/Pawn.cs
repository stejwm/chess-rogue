using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Chessman
{
    private bool hasMovedBefore = false;

    public void Start()
    {
        Activate();
        moveProfile = new PawnMovement();
        type = PieceType.Pawn;
    }

    public override List<BoardPosition> GetValidMoves()
    {
        var validMoves = moveProfile.GetValidMoves(this);
        return validMoves;
    }

    public override List<BoardPosition> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);

    // Method to check if the pawn has moved before
    public bool HasMovedBefore() => hasMovedBefore;

    // Call this method when the pawn actually moves
    public void OnMove()
    {
        hasMovedBefore = true;
    }

    public override void ResetBonuses()
    {
        base.ResetBonuses();
        hasMovedBefore = false;
    }
}
