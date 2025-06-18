using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Chessman
{
    public override void Initialize(Board board){
        Activate();
        moveProfile = new QueenMovement(board);
        type = PieceType.Queen;
    }
    public override List<Tile> GetValidMoves() => moveProfile.GetValidMoves(this);
    public override List<Tile> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);
    
}