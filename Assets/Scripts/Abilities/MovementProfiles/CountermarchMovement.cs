using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CountermarchMovement : MovementProfile
{
    public CountermarchMovement(Board board) : base(board) {}
    public override List<BoardPosition> GetValidMoves(Chessman piece, bool allowFriendlyCapture = false)
    {
        List<BoardPosition> validMoves = new List<BoardPosition>();
        validMoves.AddRange(Movement.ValidPawnMoves(board, piece, piece.xBoard, piece.yBoard - 1));
        validMoves.AddRange(Movement.ValidPawnMoves(board, piece, piece.xBoard, piece.yBoard + 1));
        if (allowFriendlyCapture)
            return validMoves;
        else
            return Movement.RemoveFriendlyPieces(board, validMoves, piece);
    }
    public override List<BoardPosition> GetValidSupportMoves(Chessman piece){
        List<BoardPosition> validMoves = new List<BoardPosition>();
        validMoves.AddRange(Movement.ValidPawnSupportMoves(board, piece,piece.xBoard,piece.yBoard-1));
        validMoves.AddRange(Movement.ValidPawnSupportMoves(board, piece,piece.xBoard,piece.yBoard+1));
        return validMoves;
    }
    
    public override List<Vector2Int> GetDirections(Chessman piece)
    {
        return null;
    }
}