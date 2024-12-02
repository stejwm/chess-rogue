using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PawnMovement : MovementProfile
{
    public override List<BoardPosition> GetValidMoves(Chessman piece) {
        if(piece.color==PieceColor.White)
            return Movement.ValidPawnMoves(piece,piece.xBoard,piece.yBoard+1);
        else
            return Movement.ValidPawnMoves(piece,piece.xBoard,piece.yBoard-1);
     }
    public override List<BoardPosition> GetValidSupportMoves(Chessman piece){
        if(piece.color==PieceColor.White)
            return Movement.ValidPawnSupportMoves(piece,piece.xBoard,piece.yBoard+1);
        else
            return Movement.ValidPawnSupportMoves(piece,piece.xBoard,piece.yBoard-1);
    }
}