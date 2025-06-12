using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.MLAgents.Sensors;
using UnityEngine;

[CreateAssetMenu(fileName = "RaiseTheFallen", menuName = "Abilities/RaiseTheFallen")]
public class RaiseTheFallen : Ability
{
    private Chessman piece;
    int stacks =0;
    public RaiseTheFallen() : base("Raise The Fallen", "Decimates piece on capture, raises a friendly pawn in it's position") {}


    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        RaiseTheFallen originalAbility = piece.abilities.OfType<RaiseTheFallen>().FirstOrDefault();
        if (originalAbility != null){
            originalAbility.stacks++;
           return;
        }
        eventHub.OnAttackStart.AddListener(SetDecimating);
        eventHub.OnPieceCaptured.AddListener(ListenForEnd);
        eventHub.OnPieceBounced.AddListener(ReplaceOnBoard);
        piece.canStationarySlash=true;
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnPieceCaptured.RemoveListener(ListenForEnd);
        eventHub.OnAttackStart.RemoveListener(SetDecimating);
        eventHub.OnPieceBounced.RemoveListener(ReplaceOnBoard);
        piece.canStationarySlash=false; 

    }
    
    public void SetDecimating(Chessman attacker, Chessman defender){
        if (attacker==piece){
            board.CurrentMatch.isDecimating=true;
        }
    }
    public void ListenForEnd(Chessman attacker, Chessman defender){
        if (attacker==piece){
            board.CurrentMatch.MovePiece(piece, piece.xBoard, piece.yBoard);
            if(piece.owner.openPositions.Count==0){
                AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Raise the Fallen</gradient></color>",  "no open positions, cannot raise the dead");
                board.CurrentMatch.isDecimating=false;
                return;
            }
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Raise the Fallen</gradient></color>",  $"raised the dead at {BoardPosition.ConvertToChessNotation(defender.xBoard, defender.yBoard)}");

            var undead = PieceFactory._instance.CreateAbilityPiece(board, PieceType.Pawn, $"undead {defender.name}", defender.xBoard, defender.yBoard, PieceColor.White, piece.owner, AbilityDatabase._instance.GetAbilityByName("RadiatingDeath")); //Create with radiating death
            undead.GetComponent<Collider2D>().enabled = false;
            piece.owner.pieces.Add(undead);
            Chessman undeadChessman = undead.GetComponent<Chessman>();
            if (Regex.Matches(undeadChessman.name, "undead").Count >= 10){
                undeadChessman.AddAbility(board, AbilityDatabase._instance.GetAbilityByName("BrokenDeath"));
            }
            undeadChessman.startingPosition = piece.owner.openPositions[0];
            piece.owner.openPositions.RemoveAt(0);
            for (int i = 0; i < stacks; i++){
                undeadChessman.attack++;
                undeadChessman.defense++;
                undeadChessman.support++;
            }
            board.CurrentMatch.MovePiece(undeadChessman, undeadChessman.xBoard, undeadChessman.yBoard);
            board.CurrentMatch.isDecimating=false;
        }
    }

    public void ReplaceOnBoard(Chessman attacker, Chessman defender){
        if (attacker==piece){
            board.CurrentMatch.MovePiece(piece, piece.xBoard, piece.yBoard);
            board.CurrentMatch.isDecimating=false;
        }
    }

}