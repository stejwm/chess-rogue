using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KingMovement : MovementProfile
{
    public override List<BoardPosition> GetValidMoves(Chessman piece) {
        return Movement.ValidKingMoves(piece,piece.xBoard,piece.yBoard);
     }
    public override List<BoardPosition> GetValidSupportMoves(Chessman piece){
        return Movement.ValidKingMoves(piece,piece.xBoard,piece.yBoard);
    }
}