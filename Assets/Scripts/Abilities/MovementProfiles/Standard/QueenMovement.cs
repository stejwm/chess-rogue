using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class QueenMovement : MovementProfile
{
    public override List<BoardPosition> GetValidMoves(Chessman piece) {
        return Movement.ValidQueenMoves(piece,piece.xBoard,piece.yBoard);
     }
    public override List<BoardPosition> GetValidSupportMoves(Chessman piece){
        return Movement.ValidQueenMoves(piece,piece.xBoard,piece.yBoard);
    }
}