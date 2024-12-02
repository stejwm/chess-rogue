using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ScoutPawn : MovementProfile
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
}