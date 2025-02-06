using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CountermarchMovement : MovementProfile
{
        public override List<BoardPosition> GetValidMoves(Chessman piece, bool allowFriendlyCapture=false) {
        List<BoardPosition> validMoves = new List<BoardPosition>();
        validMoves.AddRange(Movement.ValidPawnMoves(piece,piece.xBoard,piece.yBoard-1));
        validMoves.AddRange(Movement.ValidPawnMoves(piece,piece.xBoard,piece.yBoard+1));
        if (allowFriendlyCapture)
            return validMoves;
        else
            return Movement.RemoveFriendlyPieces(validMoves,piece);
     }
    public override List<BoardPosition> GetValidSupportMoves(Chessman piece){
        List<BoardPosition> validMoves = new List<BoardPosition>();
        validMoves.AddRange(Movement.ValidPawnSupportMoves(piece,piece.xBoard,piece.yBoard-1));
        validMoves.AddRange(Movement.ValidPawnSupportMoves(piece,piece.xBoard,piece.yBoard+1));
        return validMoves;
    }
    
    public override List<Vector2Int> GetDirections(Chessman piece)
    {
        return null;
    }
}