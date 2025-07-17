using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BishopMovement : MovementProfile
{
    public BishopMovement(Board board) : base(board) { }

    public override List<Tile> GetValidMoves(Chessman piece, bool allowFriendlyCapture = false)
    {
        if (allowFriendlyCapture)
            return Movement.ValidBishopMoves(board, piece, piece.xBoard, piece.yBoard);
        else
            return Movement.RemoveFriendlyPieces(board, Movement.ValidBishopMoves(board, piece, piece.xBoard, piece.yBoard), piece);
    }
    public override List<Tile> GetValidSupportMoves(Chessman piece){
        return Movement.ValidBishopMoves(board,piece,piece.xBoard,piece.yBoard);
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