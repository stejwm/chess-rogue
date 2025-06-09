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


    public override void Apply(Chessman piece)
    {
        //startingProfile=piece.moveProfile;
        this.piece = piece;
        piece.info += " " + abilityName;

        GameManager._instance.OnSupportAdded.AddListener(Target);
        GameManager._instance.OnPieceCaptured.AddListener(Avenge);
        GameManager._instance.OnPieceBounced.AddListener(EndAvenge);
        GameManager._instance.OnRawMoveEnd.AddListener(RawMoveCheck);
        
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {

        GameManager._instance.OnSupportAdded.RemoveListener(Target);
        GameManager._instance.OnPieceCaptured.RemoveListener(Avenge);
        GameManager._instance.OnPieceBounced.RemoveListener(EndAvenge);
        GameManager._instance.OnRawMoveEnd.RemoveListener(RawMoveCheck);

    }
    public void RawMoveCheck(Chessman piece, BoardPosition destination)
    {
        if(readyToAvenge){
            GameManager._instance.currentMatch.MyTurn(piece.color);
            GameManager._instance.currentMatch.AvengingStrikeOverride =false;
            readyToAvenge=false;
            GameManager._instance.currentMatch.AvengerActive=false;
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">???</gradient></color>", "missed attack");
        }
    }

    public void EndAvenge(Chessman attacker, Chessman defender, bool isBounceReduced){
        GameManager._instance.currentMatch.AvengingStrikeOverride =false;
        readyToAvenge=false;
        GameManager._instance.currentMatch.AvengerActive=false;
    }

    public void Avenge(Chessman attacker, Chessman defender)
    {
        if(attacker==piece && readyToAvenge){
            GameManager._instance.currentMatch.AvengingStrikeOverride =false;
            readyToAvenge=false; 
            GameManager._instance.currentMatch.AvengerActive=false;    
        }
        else if (readyToAvenge)
        {
            Debug.Log("Avenging");
            piece.effectsFeedback.PlayFeedbacks();
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Avenging Strike</gradient></color>", "attacking");
            GameManager._instance.currentMatch.AvengingStrikeOverride =true;
            if(GameManager._instance.currentMatch.BloodThirstOverride){
                //Game._instance.currentMatch.MyTurn(piece.color);
                Debug.Log("Bloodthirst is active not setting turn tho");
            }
            GameManager._instance.currentMatch.ExecuteTurn(piece, targetPosition.x, targetPosition.y);
        }
    }
    public void Target(Chessman supporter, Chessman attacker, Chessman defender){
        if(supporter==piece && defender.color==piece.color && !GameManager._instance.currentMatch.AvengerActive){
            GameManager._instance.currentMatch.AvengerActive=true;
            Debug.Log("Avenger activated");
            readyToAvenge=true;
            targetPosition = new BoardPosition(defender.xBoard, defender.yBoard);
        }
    }
}
