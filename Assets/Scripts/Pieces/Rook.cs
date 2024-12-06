using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Chessman
{
    public void Start(){
        Activate();
        moveProfile = new RookMovement();
        type = PieceType.Rook;
    }
    public override List<BoardPosition> GetValidMoves() => moveProfile.GetValidMoves(this);
    public override List<BoardPosition> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);
    
}