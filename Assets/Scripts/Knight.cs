using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Chessman
{
        public void Start(){
        Activate();
        moveProfile = new KnightMovement();
    }
    public override List<BoardPosition> GetValidMoves() => moveProfile.GetValidMoves(this);
    public override List<BoardPosition> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);
    

}