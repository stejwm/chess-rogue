using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvengingStrike", menuName = "Abilities/AvengingStrike")]
public class AvengingStrike : Ability
{
    private Chessman piece;
    MovementProfile startingProfile;
    private bool readyToAvenge = false;
    private BoardPosition targetPosition;

    
    public AvengingStrike() : base("Avenging Strike", "Automatically trigger attack if a supported piece was captured") {}


    public override void Apply(Board board, Chessman piece)
    {
        if(piece.abilities.Contains(this)){
            return;
        }
        this.piece = piece;
        piece.info += " " + abilityName;

        board.EventHub.OnSupportAdded.AddListener(Target);
        board.EventHub.OnPieceCaptured.AddListener(Avenge);
        board.EventHub.OnPieceBounced.AddListener(EndAvenge);
        board.EventHub.OnRawMoveEnd.AddListener(RawMoveCheck);
        
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {

        eventHub.OnSupportAdded.RemoveListener(Target);
        eventHub.OnPieceCaptured.RemoveListener(Avenge);
        eventHub.OnPieceBounced.RemoveListener(EndAvenge);
        eventHub.OnRawMoveEnd.RemoveListener(RawMoveCheck);

    }
    public void RawMoveCheck(Chessman piece, Tile destination)
    {
        if(readyToAvenge){
            board.CurrentMatch.MyTurn(piece.color);
            board.CurrentMatch.AvengingStrikeOverride =false;
            readyToAvenge=false;
            board.CurrentMatch.AvengerActive=false;
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">???</gradient></color>", "missed attack");
        }
    }

    public void EndAvenge(Chessman attacker, Chessman defender){
        board.CurrentMatch.AvengingStrikeOverride =false;
        readyToAvenge=false;
        board.CurrentMatch.AvengerActive=false;
    }

    public void Avenge(Chessman attacker, Chessman defender)
    {
        if(attacker==piece && readyToAvenge){
            board.CurrentMatch.AvengingStrikeOverride =false;
            readyToAvenge=false; 
            board.CurrentMatch.AvengerActive=false;    
        }
        else if (readyToAvenge)
        {
            Debug.Log("Avenging");
            piece.effectsFeedback.PlayFeedbacks();
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Avenging Strike</gradient></color>", "attacking");
            board.CurrentMatch.AvengingStrikeOverride =true;
            if(board.CurrentMatch.BloodThirstOverride){
                //Game._instance.currentMatch.MyTurn(piece.color);
                Debug.Log("Bloodthirst is active not setting turn tho");
            }
            board.CurrentMatch.ExecuteTurn(piece, targetPosition.x, targetPosition.y);
        }
    }
    public void Target(Chessman attacker, Chessman defender, Chessman supporter){
        if(supporter==piece && defender.color==piece.color && !board.CurrentMatch.AvengerActive){
            board.CurrentMatch.AvengerActive=true;
            Debug.Log("Avenger activated");
            readyToAvenge=true;
            targetPosition = new BoardPosition(defender.xBoard, defender.yBoard);
        }
    }
}
