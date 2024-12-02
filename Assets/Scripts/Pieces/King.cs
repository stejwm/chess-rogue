using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Chessman
{

    public void Start(){
        Activate();
        moveProfile = new KingMovement();
    }
    public override List<BoardPosition> GetValidMoves() => moveProfile.GetValidMoves(this);
    public override List<BoardPosition> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);
    
}