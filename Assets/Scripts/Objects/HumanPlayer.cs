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
       RarityWeights = new Dictionary<Rarity, int>()
        {
            { Rarity.Common, 80 },
            { Rarity.Uncommon, 15 },
            { Rarity.Rare, 5 }
        };
    }

    public override void MakeMove(ChessMatch match)
    {
        // Handle human input for moves.
    }
}