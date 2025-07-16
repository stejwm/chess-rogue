using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
public class TravelersGraceMovement : MovementProfile
{
    MovementProfile oldProfile;
    public TravelersGraceMovement(Board board, MovementProfile old) : base(board) {oldProfile = old;}
    public override List<Tile> GetValidMoves(Chessman piece, bool allowFriendlyCapture = false)
    {
        return Movement.AllOpenSquares(board);
    }
    public override List<Tile> GetValidSupportMoves( Chessman piece){
        return oldProfile.GetValidSupportMoves(piece);
    }

    public override List<Vector2Int> GetDirections(Chessman piece){
        return oldProfile.GetDirections(piece);
    } 
}