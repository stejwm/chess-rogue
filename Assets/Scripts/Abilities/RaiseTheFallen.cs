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
    public Ability brokenDeath;
    public RaiseTheFallen() : base("Raise The Fallen", "Decimates piece on capture, raises a friendly pawn in it's position") {}


    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        RaiseTheFallen originalAbility = piece.abilities.OfType<RaiseTheFallen>().FirstOrDefault();
        if (originalAbility != null){
            originalAbility.stacks++;
           return;
        }
        GameManager._instance.OnAttackStart.AddListener(SetDecimating);
        GameManager._instance.OnPieceCaptured.AddListener(ListenForEnd);
        GameManager._instance.OnPieceBounced.AddListener(ReplaceOnBoard);
        piece.canStationarySlash=true;
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        GameManager._instance.OnPieceCaptured.RemoveListener(ListenForEnd);
        GameManager._instance.OnAttackStart.RemoveListener(SetDecimating);
        GameManager._instance.OnPieceBounced.RemoveListener(ReplaceOnBoard);
        piece.canStationarySlash=false; 

    }
    
    public void SetDecimating(Chessman attacker, Chessman defender){
        if (attacker==piece){
            GameManager._instance.isDecimating=true;
        }
    }
    public void ListenForEnd(Chessman attacker, Chessman defender){
        if (attacker==piece){
            GameManager._instance.currentMatch.MovePiece(piece, piece.xBoard, piece.yBoard);
            if(piece.owner.openPositions.Count==0){
                AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Raise the Fallen</gradient></color>",  "no open positions, cannot raise the dead");
                GameManager._instance.isDecimating=false;
                return;
            }
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Raise the Fallen</gradient></color>",  $"raised the dead at {BoardPosition.ConvertToChessNotation(defender.xBoard, defender.yBoard)}");

            var undead = PieceFactory._instance.CreateAbilityPiece(PieceType.Pawn, $"undead {defender.name}", defender.xBoard, defender.yBoard, PieceColor.White, Team.Hero, piece.owner, GameManager._instance.AllAbilities[25].Clone()); //Create with radiating death
            undead.GetComponent<Collider2D>().enabled = false;
            piece.owner.pieces.Add(undead);
            Chessman undeadChessman = undead.GetComponent<Chessman>();
            if (Regex.Matches(undeadChessman.name, "undead").Count >= 10){
                undeadChessman.AddAbility(brokenDeath.Clone());
            }
            undeadChessman.startingPosition = piece.owner.openPositions[0];
            piece.owner.openPositions.RemoveAt(0);
            for (int i = 0; i < stacks; i++){
                undeadChessman.attack++;
                undeadChessman.defense++;
                undeadChessman.support++;
            }
            GameManager._instance.currentMatch.MovePiece(undeadChessman, undeadChessman.xBoard, undeadChessman.yBoard);
            GameManager._instance.isDecimating=false;
        }
    }

    public void ReplaceOnBoard(Chessman attacker, Chessman defender, bool isReduced){
        if (attacker==piece){
            GameManager._instance.currentMatch.MovePiece(piece, piece.xBoard, piece.yBoard);
            GameManager._instance.isDecimating=false;
        }
    }

}