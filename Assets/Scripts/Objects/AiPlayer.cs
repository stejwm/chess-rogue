using System.Collections;
using System.Collections.Generic;
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
        agent.RequestDecision();
    }

    public override void DestroyPieces(){
       
        base.DestroyPieces(); // Call the base class logic to destroy pieces.
        agent.ShutDown();         // Additional AI-specific logic.
    
    }
}