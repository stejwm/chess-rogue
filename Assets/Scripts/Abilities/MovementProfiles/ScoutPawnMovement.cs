using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ScoutPawnMovement : MovementProfile
{
    public override List<BoardPosition> GetValidMoves(Chessman piece) {
        List<BoardPosition> validMoves = new List<BoardPosition>();
        validMoves.AddRange(Movement.ValidScoutMoves(piece,piece.xBoard,piece.yBoard));

        if(piece.color==PieceColor.White)
            validMoves.AddRange(Movement.ValidPawnSupportMoves(piece,piece.xBoard,piece.yBoard+1));
        else
            validMoves.AddRange(Movement.ValidPawnSupportMoves(piece,piece.xBoard,piece.yBoard-1));
        
        return validMoves;
     }
    public override List<BoardPosition> GetValidSupportMoves(Chessman piece){
        if(piece.color==PieceColor.White)
            return Movement.ValidPawnSupportMoves(piece,piece.xBoard,piece.yBoard+1);
        else
            return Movement.ValidPawnSupportMoves(piece,piece.xBoard,piece.yBoard-1);
    }
    public override List<Vector2Int> GetDirections(Chessman piece){
        return new List<Vector2Int>
        {
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1),  // Down
            new Vector2Int(1, 0),   // Right
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(1, 1),   // Up-Right
            new Vector2Int(-1, -1), // Down-Left
            new Vector2Int(1, -1),  // Down-Right
            new Vector2Int(-1, 1)   // Up-Left
        };
    }
}