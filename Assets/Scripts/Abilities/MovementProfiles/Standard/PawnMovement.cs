using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PawnMovement : MovementProfile
{
    public PawnMovement(Board board) : base(board) { }
    public override List<BoardPosition> GetValidMoves(Chessman piece, bool allowFriendlyCapture=false) {
        if (allowFriendlyCapture)
            if(piece.color==PieceColor.White)
                return Movement.ValidPawnMoves(board, piece,piece.xBoard,piece.yBoard+1);
            else
                return Movement.ValidPawnMoves(board, piece,piece.xBoard,piece.yBoard-1);
        else
            if(piece.color==PieceColor.White)
                return Movement.RemoveFriendlyPieces(board,Movement.ValidPawnMoves(board, piece,piece.xBoard,piece.yBoard+1),piece);
            else
                return Movement.RemoveFriendlyPieces(board,Movement.ValidPawnMoves(board, piece,piece.xBoard,piece.yBoard-1),piece);
     }
    public override List<BoardPosition> GetValidSupportMoves(Chessman piece){
        if(piece.color==PieceColor.White)
            return Movement.ValidPawnSupportMoves(board, piece,piece.xBoard,piece.yBoard+1);
        else
            return Movement.ValidPawnSupportMoves(board, piece,piece.xBoard,piece.yBoard-1);
    }
    
    public override List<Vector2Int> GetDirections(Chessman piece)
    {
        /* bool isWhite = piece.color == PieceColor.White;
        return isWhite 
            ? new List<Vector2Int>
            {
                new Vector2Int(0, 1),   // Forward
                new Vector2Int(-1, 1),  // Attack Left
                new Vector2Int(1, 1)    // Attack Right
            }
            : new List<Vector2Int>
            {
                new Vector2Int(0, -1),  // Forward
                new Vector2Int(-1, -1), // Attack Left
                new Vector2Int(1, -1)   // Attack Right
            }; */
            return null;
    }
}