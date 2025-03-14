using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
public class TravelersGraceMovement : MovementProfile
{
    MovementProfile oldProfile;
    Game game;
    public TravelersGraceMovement(MovementProfile old){
        oldProfile=old;
    }
    public override List<BoardPosition> GetValidMoves(Chessman piece, bool allowFriendlyCapture=false) {
        return Movement.AllOpenSquares();
     }
    public override List<BoardPosition> GetValidSupportMoves(Chessman piece){
        return oldProfile.GetValidSupportMoves(piece);
    }

    public override List<Vector2Int> GetDirections(Chessman piece){
        return oldProfile.GetDirections(piece);
    } 
}