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
            { Rarity.Common, 55 },
            { Rarity.Uncommon, 35 },
            { Rarity.Rare, 10 }
        };
    }

    public override void MakeMove(ChessMatch match)
    {
        // Handle human input for moves.
    }
}