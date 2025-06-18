using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Chessman
{
    public override void Initialize(Board board){
        Activate();
        moveProfile = new BishopMovement(board);
        type = PieceType.Bishop;
    }
    public override List<Tile> GetValidMoves() => moveProfile.GetValidMoves(this);
    public override List<Tile> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);
    
}