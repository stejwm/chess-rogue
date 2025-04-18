using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ScoutCounterMovement : MovementProfile
{
    public override List<BoardPosition> GetValidMoves(Chessman piece, bool allowFriendlyCapture=false) {
        HashSet<BoardPosition> moveSet = new HashSet<BoardPosition>();
        moveSet.UnionWith(Movement.ValidPawnMoves(piece,piece.xBoard,piece.yBoard-1));
        moveSet.UnionWith(Movement.ValidPawnMoves(piece,piece.xBoard,piece.yBoard+1));
        moveSet.UnionWith(Movement.ValidScoutMoves(piece,piece.xBoard,piece.yBoard));

        List<BoardPosition> validMoves = moveSet.ToList();
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