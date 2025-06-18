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
    public override List<Tile> GetValidMoves() => moveProfile.GetValidMoves(this);
    public override List<Tile> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);
    

}