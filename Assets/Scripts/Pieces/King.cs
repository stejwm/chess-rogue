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
    public override List<Tile> GetValidMoves() => moveProfile.GetValidMoves(this);
    public override List<Tile> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);
    
}