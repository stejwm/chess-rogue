using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "VictoryFormation", menuName = "Abilities/VictoryFormation")]
public class VictoryFormation : Ability
{
    private Chessman piece;
    List<GameObject> startingPieces = new List<GameObject>();

    public VictoryFormation() : base("Victory Formation", "permanent +1 to all stats if no pieces captured during match") { }

    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        board.EventHub.OnChessMatchStart.AddListener(RollCall);
        board.EventHub.OnGameEnd.AddListener(Loot);
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnGameEnd.RemoveListener(Loot);

    }
    public void Loot(PieceColor color)
    {
        if (color == piece.color)
        {
            if (startingPieces.All(item => piece.owner.pieces.Contains(item)))
            {
                piece.attack++;
                piece.defense++;
                piece.support++;
            }
        }
    }
    
    public void RollCall(){
        
        foreach (GameObject mate in piece.owner.pieces)
        {
            startingPieces.Add(mate);
        }
    }

}