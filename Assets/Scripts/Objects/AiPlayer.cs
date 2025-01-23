using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Integrations.Match3;
using UnityEngine;
using Rand= System.Random;

public class AIPlayer : Player
{
    [SerializeField] protected PlayerAgent agent;
    private static Rand rng = new Rand();
    public PieceColor color;
    
    public AIPlayer(List<GameObject> pieces):base(pieces)
    {
        this.pieces=pieces;
    }
    public override void Initialize()
    {
        agent.pieces=pieces;
        agent.StartUp();
    }

    public void CreateMoveCommandDictionary(){
        agent.CreateMoveCommandDictionary();
    }

    public void SetSelectedPiece(Chessman piece){
        agent.selectedPiece=piece;
    }
    public void SetSelectedDestination(BoardPosition position){
        agent.destinationPosition=position;
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

    public virtual void RandomAbilities(){
        
        foreach (GameObject piece in pieces)
        {
            Chessman cm = piece.GetComponent<Chessman>();
            int index = rng.Next(30);
            if (Game._instance.AllAbilities.Count>index){
                cm.AddAbility(Game._instance.AllAbilities[index].Clone());
            }
        }
    }

    public void RequestDecision(){
        agent.RequestDecision();
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