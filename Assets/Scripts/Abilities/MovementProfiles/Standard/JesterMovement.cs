using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class JesterMovement : MovementProfile
{
    public override List<BoardPosition> GetValidMoves(Chessman piece, bool allowFriendlyCapture=false) {
        if (allowFriendlyCapture)
            return Movement.ValidJesterMoves(piece,piece.xBoard,piece.yBoard);
        else
            return Movement.RemoveFriendlyPieces(Movement.ValidJesterMoves(piece,piece.xBoard,piece.yBoard),piece);
     }
    public override List<BoardPosition> GetValidSupportMoves(Chessman piece){
        return Movement.ValidJesterMoves(piece,piece.xBoard,piece.yBoard);
    }
    public override List<Vector2Int> GetDirections(Chessman piece)
    {
        return null;
    }
}