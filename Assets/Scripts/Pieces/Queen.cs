using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Chessman
{
    protected override void Awake()
    {
        moveProfile = new QueenMovement();
        type = PieceType.Queen;
        
        base.Awake();
    }

    public void Start()
    {
        Activate();
    }
    public override List<BoardPosition> GetValidMoves() => moveProfile.GetValidMoves(this);
    public override List<BoardPosition> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);
    
}