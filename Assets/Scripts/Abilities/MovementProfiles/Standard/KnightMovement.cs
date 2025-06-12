using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KnightMovement : MovementProfile
{
    public KnightMovement(Board board) : base(board) { }
    public override List<BoardPosition> GetValidMoves(Chessman piece, bool allowFriendlyCapture = false)
    {
        if (allowFriendlyCapture)
            return Movement.ValidKnightMoves(piece, piece.xBoard, piece.yBoard);
        else
            return Movement.RemoveFriendlyPieces(board, Movement.ValidKnightMoves(piece, piece.xBoard, piece.yBoard), piece);
    }
    public override List<BoardPosition> GetValidSupportMoves(Chessman piece){
        return Movement.ValidKnightMoves(piece,piece.xBoard,piece.yBoard);
    }
    public override List<Vector2Int> GetDirections(Chessman piece)
    {
        /* return new List<Vector2Int>
        {
            new Vector2Int(2, 1),   // Up-Right
            new Vector2Int(2, -1),  // Down-Right
            new Vector2Int(-2, 1),  // Up-Left
            new Vector2Int(-2, -1), // Down-Left
            new Vector2Int(1, 2),   // Right-Up
            new Vector2Int(1, -2),  // Right-Down
            new Vector2Int(-1, 2),  // Left-Up
            new Vector2Int(-1, -2)  // Left-Down
        }; */
        return null;
    }
}