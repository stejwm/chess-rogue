using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Swift", menuName = "Abilities/Swift")]
public class Swift : Ability
{
    private Chessman piece;
    private bool swifting = false;
    
    public Swift() : base("Swift", "Move again if first move is not an attack") {}


    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;

        eventHub.OnRawMoveEnd.AddListener(Swifting);
        eventHub.OnAttackEnd.AddListener(EndSwift);
        base.Apply(board, piece);

    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnRawMoveEnd.RemoveListener(Swifting);
        eventHub.OnAttackEnd.RemoveListener(EndSwift);
    }

    public void Swifting(Chessman mover, Tile destination)
    {
        List<GameObject> pieces;

        if(mover==piece && !swifting && piece.moveProfile.GetValidMoves(piece).Count>=0){
            swifting=true;
            board.CurrentMatch.SwiftOverride =true;

            pieces = piece.owner.pieces;
            foreach (GameObject pieceObject in pieces)
            {
                pieceObject.GetComponent<Chessman>().isValidForAttack=false;
            }
            piece.isValidForAttack=true;
            
            board.CurrentMatch.MyTurn(piece.color);

            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Swift</gradient></color>",  " move again");
            piece.owner.MakeMove(board.CurrentMatch);
        }
        else if(mover==piece && swifting){
            swifting=false;
            board.CurrentMatch.SwiftOverride =false;
        }
    }

    private void EndSwift(Chessman attackingPiece, Chessman defendingPiece, int attackSupport, int defenseSupport){
        if(attackingPiece==piece){
            swifting=false;
            board.CurrentMatch.SwiftOverride =false;
        }
    }
}
