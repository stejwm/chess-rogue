using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class JesterMovement : MovementProfile
{
    public JesterMovement(Board board) : base(board) { }
    public override List<Tile> GetValidMoves(Chessman piece, bool allowFriendlyCapture = false)
    {
        if (allowFriendlyCapture)
            return Movement.ValidJesterMoves(board, piece, piece.xBoard, piece.yBoard);
        else
            return Movement.RemoveFriendlyPieces(board, Movement.ValidJesterMoves(board, piece, piece.xBoard, piece.yBoard), piece);
    }
    public override List<Tile> GetValidSupportMoves(Chessman piece){
        return Movement.ValidJesterMoves(board, piece,piece.xBoard,piece.yBoard);
    }
    public override List<Vector2Int> GetDirections(Chessman piece)
    {
        return null;
    }
}