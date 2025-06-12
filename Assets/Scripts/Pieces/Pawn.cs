using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Chessman
{
        public override void Initialize(Board board){
        Activate();
        moveProfile = new PawnMovement(board);
        type = PieceType.Pawn;
    }

    public override List<BoardPosition> GetValidMoves() => moveProfile.GetValidMoves(this);
    public override List<BoardPosition> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);
    
}
