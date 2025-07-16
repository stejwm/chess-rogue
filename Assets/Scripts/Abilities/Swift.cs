using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Swift", menuName = "Abilities/Swift")]
public class Swift : Ability
{
    private Chessman piece;
    int stacks = 1;
    int uses = 0;
    
    public Swift() : base("Swift", "Move again if first move is not an attack") { }

    public override void Apply(Board board, Chessman piece)
    {
        Swift originalAbility = piece.abilities.OfType<Swift>().FirstOrDefault();
        if (originalAbility != null){
            originalAbility.stacks++;
            return;
        }
            
        this.piece = piece;
        piece.info += " " + abilityName;

        board.EventHub.OnRawMoveEnd.AddListener(Swifting);
        board.EventHub.OnAttackEnd.AddListener(EndSwift);
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

        // Only allow as many uses as stacks
        if (mover == piece && uses < stacks && piece.moveProfile.GetValidMoves(piece).Count > 0)
        {
            uses++;
            board.CurrentMatch.SwiftOverride = true;

            pieces = piece.owner.pieces;
            foreach (GameObject pieceObject in pieces)
            {
                pieceObject.GetComponent<Chessman>().isValidForAttack = false;
            }
            piece.isValidForAttack = true;

            board.CurrentMatch.MyTurn(piece.color);

            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Swift</gradient></color>", " move again");
            piece.owner.MakeMove(board.CurrentMatch);

            // If all uses are spent, reset SwiftOverride
            if (uses > stacks)
            {
                board.CurrentMatch.SwiftOverride = false;
            }
        }
        else if (mover == piece && uses >= stacks)
        {
            board.CurrentMatch.SwiftOverride = false;
            uses = 0;
        }
    }

    private void EndSwift(Chessman attackingPiece, Chessman defendingPiece, int attackSupport, int defenseSupport){
        if(attackingPiece==piece){
            uses = 0; // End all remaining uses if an attack occurs
            board.CurrentMatch.SwiftOverride = false;
        }
    }
}
