using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class HumanPlayer : Player
{
    public HumanPlayer(List<GameObject> pieces) : base(pieces) { }

    public override void Initialize()
    {
        //pieces = PieceFactory._instance.CreateWhitePieces(this);
        openPositions = new List<BoardPosition>();
        int startingRow;
        if (pieces[0].GetComponent<Chessman>().startingPosition.y <= 2)
            startingRow=2;
        else
            startingRow=7;
            
        for (int i =0; i<8; i++){
            openPositions.Add(new BoardPosition(i,startingRow));
        }
    }

    public override void MakeMove(ChessMatch match)
    {
        // Handle human input for moves.
    }
}