using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Chessman
{
    public override void Initialize(Board board){
        Activate();
        moveProfile = new RookMovement(board);
        type = PieceType.Rook;
    }
    public override List<Tile> GetValidMoves() => moveProfile.GetValidMoves(this);
    public override List<Tile> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);
    
}