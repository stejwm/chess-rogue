using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Integrations.Match3;
using UnityEngine;


public class AIPlayer : Player
{
    [SerializeField] protected PlayerAgent agent;

    
    public AIPlayer(List<GameObject> pieces):base(pieces)
    {
        this.pieces=pieces;
    }
    public override void Initialize()
    {
        agent.pieces=pieces;
        agent.StartUp();
        openPositions = new List<BoardPosition>();
    }

    public override void CreateMoveCommandDictionary(){
        agent.CreateMoveCommandDictionary();
    }

    public void SetSelectedPiece(Chessman piece){
        agent.selectedPiece=piece;
    }
    public void SetSelectedDestination(BoardPosition position){
        agent.destinationPosition=position;
    }




    public void RequestDecision(){
        agent.RequestDecision();
    }

    public override void MakeMove(ChessMatch match)
    {
        StartCoroutine(Move());
        
    }

    public IEnumerator Move(){
        yield return new WaitForSeconds(GameManager._instance.waitTime);
        Debug.Log("Requested move from "+color);
        agent.RequestDecision();
    }

    public override void DestroyPieces(){
       
        base.DestroyPieces(); // Call the base class logic to destroy pieces.
        agent.ShutDown();         // Additional AI-specific logic.
    
    }
}