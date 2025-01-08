using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Integrations.Match3;
using UnityEngine;
using Rand= System.Random;

public class KnightsOfTheRoundTable : AIPlayer
{
    private static Rand rng = new Rand();
    public KnightsOfTheRoundTable(List<GameObject> pieces):base(pieces)
    {
        this.pieces=pieces;
    }
    public override void Initialize()
    {   
        pieces = PieceFactory._instance.CreateKnightsOfTheRoundTable(this);
        agent.pieces=pieces;
        agent.StartUp();
    }

    public override void LevelUp(int level){
        for (int i =0; i<level*2; i++)
            foreach (GameObject piece in pieces)
            {
                Chessman cm = piece.GetComponent<Chessman>();
                switch (rng.Next(3)){
                    case 0:
                        cm.defense+=1;
                        break;
                    case 1:
                        cm.attack+=1;
                        break;
                    case 2:
                        cm.support+=1;
                        break;
                }
            }
    }
}