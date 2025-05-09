using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
public class BetrayerMovement : MovementProfile
{
    MovementProfile oldProfile;
    Game game;
    public BetrayerMovement(MovementProfile old){
        oldProfile=old;
    }
    public override List<BoardPosition> GetValidMoves(Chessman piece, bool allowFriendlyCapture) {
        return oldProfile.GetValidMoves(piece, true);
     }
    public override List<BoardPosition> GetValidSupportMoves(Chessman piece){
        return new List<BoardPosition>();
    }

    public override List<Vector2Int> GetDirections(Chessman piece){
        return oldProfile.GetDirections(piece);
    } 
}