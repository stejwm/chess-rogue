using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ScoutCounterMovement : MovementProfile
{
    public ScoutCounterMovement(Board board) : base(board) {}
    public override List<Tile> GetValidMoves(Chessman piece, bool allowFriendlyCapture = false)
    {
        HashSet<Tile> moveSet = new HashSet<Tile>();
        moveSet.UnionWith(Movement.ValidPawnMoves(board, piece, piece.xBoard, piece.yBoard - 1));
        moveSet.UnionWith(Movement.ValidPawnMoves(board, piece, piece.xBoard, piece.yBoard + 1));
        moveSet.UnionWith(Movement.ValidScoutMoves(board, piece, piece.xBoard, piece.yBoard));

        List<Tile> validMoves = moveSet.ToList();
        if (allowFriendlyCapture)
            return validMoves;
        else
            return Movement.RemoveFriendlyPieces(board, validMoves, piece);
    }
    public override List<Tile> GetValidSupportMoves(Chessman piece){
        List<Tile> validMoves = new List<Tile>();
        validMoves.AddRange(Movement.ValidPawnSupportMoves(board, piece,piece.xBoard,piece.yBoard-1));
        validMoves.AddRange(Movement.ValidPawnSupportMoves(board, piece,piece.xBoard,piece.yBoard+1));
        return validMoves;
    }
    
    public override List<Vector2Int> GetDirections(Chessman piece)
    {
        return null;
    }
}