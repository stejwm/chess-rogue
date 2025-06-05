using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Chessman
{
    protected override void Awake()
    {
        moveProfile = new KnightMovement();
        type = PieceType.Knight;
        base.Awake();
    }

    public void Start()
    {
        Activate();
    }
    public override List<BoardPosition> GetValidMoves() => moveProfile.GetValidMoves(this);
    public override List<BoardPosition> GetValidSupportMoves() => moveProfile.GetValidSupportMoves(this);
    

}