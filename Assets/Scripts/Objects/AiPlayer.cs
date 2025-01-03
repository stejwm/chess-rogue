using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Integrations.Match3;
using UnityEngine;
public class AIPlayer : Player
{
    [SerializeField] private PlayerAgent agent;

    public AIPlayer(List<GameObject> pieces):base(pieces)
    {
        this.pieces=pieces;
    }
    public override void Initialize()
    {   
        agent.pieces=pieces;
        agent.StartUp();
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