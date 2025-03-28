using System;
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
        int startingRow;
        if (pieces[0].GetComponent<Chessman>().startingPosition.y <= 2)
            startingRow=2;
        else
            startingRow=7;
            
        for (int i =0; i<8; i++){
            openPositions.Add(new BoardPosition(i,startingRow));
        }
    }

    public override void CreateMoveCommandDictionary(){
        agent.CreateMoveCommandDictionary();
    }

    public override void SetSelectedPiece(Chessman piece){
        agent.selectedPiece=piece;
    }
    public override void SetSelectedDestination(BoardPosition position){
        agent.destinationPosition=position;
    }




    public override void RequestDecision(){
        try{
            agent.RequestDecision();
        }
        catch(Exception e){
            Debug.Log($"Exception occured requesting decision {e.Message} retrying");
            RetryDecisionRequest(0);
        }
        
    }
    public void RetryDecisionRequest(int retries =0){
        try{
            agent.RequestDecision();
        }
        catch(Exception e){
            Debug.Log($"Exception occured requesting decision {e.Message} retrying");
            RetryDecisionRequest(++retries);
        }
    }

    public override void MakeMove(ChessMatch match)
    {
        StartCoroutine(Move());
        
    }

    public IEnumerator Move(){
        yield return new WaitForSeconds(Game._instance.waitTime);
        Debug.Log("Requested move from "+color);
        agent.RequestDecision();
    }

    public override void DestroyPieces(){
       
        base.DestroyPieces(); // Call the base class logic to destroy pieces.
        agent.ShutDown();         // Additional AI-specific logic.
    
    }
}