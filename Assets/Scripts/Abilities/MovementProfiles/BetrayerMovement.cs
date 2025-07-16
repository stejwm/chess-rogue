using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
public class BetrayerMovement : MovementProfile
{
    MovementProfile oldProfile;
    
    public BetrayerMovement(Board board, MovementProfile old) : base(board) {oldProfile = old;}
    public override List<Tile> GetValidMoves(Chessman piece, bool allowFriendlyCapture) {
        return oldProfile.GetValidMoves( piece, true);
     }
    public override List<Tile> GetValidSupportMoves(Chessman piece){
        return new List<Tile>();
    }

    public override List<Vector2Int> GetDirections(Chessman piece){
        return oldProfile.GetDirections(piece);
    } 
}