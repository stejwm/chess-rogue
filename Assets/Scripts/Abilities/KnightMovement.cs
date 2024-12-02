using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KnightMovement : MovementProfile
{
    public override List<BoardPosition> GetValidMoves(Chessman piece) {
        return Movement.ValidKnightMoves(piece,piece.xBoard,piece.yBoard);
     }
    public override List<BoardPosition> GetValidSupportMoves(Chessman piece){
        return Movement.ValidKnightMoves(piece,piece.xBoard,piece.yBoard);
    }
}