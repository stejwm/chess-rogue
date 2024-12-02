using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RookMovement : MovementProfile
{
    public override List<BoardPosition> GetValidMoves(Chessman piece) {
        return Movement.ValidRookMoves(piece,piece.xBoard,piece.yBoard);
     }
    public override List<BoardPosition> GetValidSupportMoves(Chessman piece){
        return Movement.ValidRookMoves(piece,piece.xBoard,piece.yBoard);
    }
}