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


    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;

        GameManager._instance.OnRawMoveEnd.AddListener(Swifting);
        GameManager._instance.OnAttackEnd.AddListener(EndSwift);
        base.Apply(piece);

    }

    public override void Remove(Chessman piece)
    {
        GameManager._instance.OnRawMoveEnd.RemoveListener(Swifting);
        GameManager._instance.OnAttackEnd.RemoveListener(EndSwift);
    }

    public void Decimate(Chessman attacker, int support, bool isAttacking, BoardPosition targetedPosition){
        if(attacker==piece && isAttacking)
            GameManager._instance.isDecimating=true;
    }
    public void Swifting(Chessman mover, BoardPosition destination)
    {
        List<GameObject> pieces;

        if(mover==piece && !swifting && piece.moveProfile.GetValidMoves(piece).Count>=0){
            swifting=true;
            GameManager._instance.currentMatch.SwiftOverride =true;

            pieces = piece.owner.pieces;
            foreach (GameObject pieceObject in pieces)
            {
                pieceObject.GetComponent<Chessman>().isValidForAttack=false;
            }
            piece.isValidForAttack=true;
            
            GameManager._instance.currentMatch.MyTurn(piece.color);

            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Swift</gradient></color>",  " move again");
            piece.owner.MakeMove(GameManager._instance.currentMatch);
        }
        else if(mover==piece && swifting){
            swifting=false;
            GameManager._instance.currentMatch.SwiftOverride =false;
        }
    }

    private void EndSwift(Chessman attackingPiece, Chessman defendingPiece, int attackSupport, int defenseSupport){
        if(attackingPiece==piece){
            swifting=false;
            GameManager._instance.currentMatch.SwiftOverride =false;
        }
    }
}
