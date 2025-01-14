using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Integrations.Match3;
using UnityEngine;
using Rand= System.Random;

public class AIPlayer : Player
{
    [SerializeField] protected PlayerAgent agent;
    private static Rand rng = new Rand();
    public AIPlayer(List<GameObject> pieces):base(pieces)
    {
        this.pieces=pieces;
    }
    public override void Initialize()
    {   
        pieces = PieceFactory._instance.CreateBlackPieces(this);
        agent.pieces=pieces;
        agent.StartUp();
    }

    public virtual void LevelUp(int level){
        for (int i =0; i<level; i++)
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

    public override void MakeMove(ChessMatch match)
    {
        StartCoroutine(Move());
        
    }

    public IEnumerator Move(){
        yield return new WaitForSeconds(Game._instance.waitTime);
        agent.RequestDecision();
    }

    public override void DestroyPieces(){
       
        base.DestroyPieces(); // Call the base class logic to destroy pieces.
        agent.ShutDown();         // Additional AI-specific logic.
    
    }
}