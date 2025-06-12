using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Chessman
{
        public override void Initialize(Board board){
        Activate();
        moveProfile = new KnightMovement(board);
        type = PieceType.Knight;
    }
    public override List<BoardPosition> GetValidMoves() => moveProfile.GetValidMoves(this);
    public override List<BoardPosition> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);
    

}