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
        // Call OnMove when a move is executed
        if (validMoves.Count > 0)
        {
            OnMove();
        }
        return validMoves;
    }

    public override List<BoardPosition> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);

    // Method to check if the pawn has moved before
    public bool HasMovedBefore() => hasMovedBefore;

    // Call this method when the pawn moves
    public void OnMove()
    {
        hasMovedBefore = true;
    }
}
