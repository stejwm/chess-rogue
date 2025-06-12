using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Chessman
{

    public override void Initialize(Board board){
        Activate();
        moveProfile = new KingMovement(board);
        type = PieceType.King;
    }
    public override List<BoardPosition> GetValidMoves() => moveProfile.GetValidMoves(this);
    public override List<BoardPosition> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);
    
}