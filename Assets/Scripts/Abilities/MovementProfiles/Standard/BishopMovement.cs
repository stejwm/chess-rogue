using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BishopMovement : MovementProfile
{
    public override List<BoardPosition> GetValidMoves(Chessman piece) {
        return Movement.ValidBishopMoves(piece,piece.xBoard,piece.yBoard);
     }
    public override List<BoardPosition> GetValidSupportMoves(Chessman piece){
        return Movement.ValidBishopMoves(piece,piece.xBoard,piece.yBoard);
    }
    public override List<Vector2Int> GetDirections(Chessman piece)
    {
        return new List<Vector2Int>
        {
            new Vector2Int(1, 1),   // Up-Right
            new Vector2Int(-1, -1), // Down-Left
            new Vector2Int(1, -1),  // Down-Right
            new Vector2Int(-1, 1)   // Up-Left
        };
    }
}