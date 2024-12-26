using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Chessman
{
    public void Start()
    {
        Activate();
        moveProfile = new KingMovement();
        type = PieceType.King;
    }

    public override List<BoardPosition> GetValidMoves() => moveProfile.GetValidMoves(this);
    public override List<BoardPosition> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);

    // Called by MoveManager when the piece moves
    public void OnMove()
    {
        hasMoved = true;
    }

    public override void ResetBonuses()
    {
        base.ResetBonuses();
        hasMoved = false;  // Reset castling eligibility
    }
}