using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class HumanPlayer : Player
{
    public HumanPlayer(List<GameObject> pieces) : base(pieces) { }

    public override void Initialize(Board board)
    {
        RarityWeights = new Dictionary<Rarity, int>()
        {
            { Rarity.Common, 80 },
            { Rarity.Uncommon, 15 },
            { Rarity.Rare, 5 }
        };

        for (int i = 0; i < board.Width; i++)
        {
            openPositions.Add(board.GetTileAt(i, 2));
        }
    }

    public override void MakeMove(ChessMatch match)
    {
        // Handle human input for moves.
    }
}