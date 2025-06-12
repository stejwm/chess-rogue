using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jester : Chessman
{
        public override void Initialize(Board board){
        Activate();
        moveProfile = new JesterMovement(board);
        type = PieceType.Jester;
    }
    public override List<BoardPosition> GetValidMoves() => moveProfile.GetValidMoves(this);
    public override List<BoardPosition> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);
    

}