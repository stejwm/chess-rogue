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

        Game._instance.OnSupportAdded.AddListener(Target);
        Game._instance.OnPieceCaptured.AddListener(Avenge);
        Game._instance.OnPieceBounced.AddListener(EndAvenge);
        Game._instance.OnRawMoveEnd.AddListener(RawMoveCheck);
        
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {

        Game._instance.OnSupportAdded.RemoveListener(Target);
        Game._instance.OnPieceCaptured.RemoveListener(Avenge);
        Game._instance.OnPieceBounced.RemoveListener(EndAvenge);
        Game._instance.OnRawMoveEnd.RemoveListener(RawMoveCheck);

    }
    public void RawMoveCheck(Chessman piece, BoardPosition destination)
    {
        if(readyToAvenge){
            Game._instance.currentMatch.MyTurn(piece.color);
            Game._instance.currentMatch.AvengingStrikeOverride =false;
            readyToAvenge=false;
            Game._instance.currentMatch.AvengerActive=false;
            AbilityLogger._instance.LogAbilityUsage($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">???</gradient></color>", "missed attack");
        }
    }

    public void EndAvenge(Chessman attacker, Chessman defender, bool isBounceReduced){
        Game._instance.currentMatch.AvengingStrikeOverride =false;
        readyToAvenge=false;
        Game._instance.currentMatch.AvengerActive=false;
    }

    public void Avenge(Chessman attacker, Chessman defender)
    {
        if(attacker==piece && readyToAvenge){
            Game._instance.currentMatch.AvengingStrikeOverride =false;
            readyToAvenge=false; 
            Game._instance.currentMatch.AvengerActive=false;    
        }
        else if (readyToAvenge)
        {
            Debug.Log("Avenging");
            piece.effectsFeedback.PlayFeedbacks();
            AbilityLogger._instance.LogAbilityUsage($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Avenging Strike</gradient></color>", "attacking");
            Game._instance.currentMatch.AvengingStrikeOverride =true;
            if(Game._instance.currentMatch.BloodThirstOverride){
                //Game._instance.currentMatch.MyTurn(piece.color);
                Debug.Log("Bloodthirst is active not setting turn tho");
            }
            Game._instance.currentMatch.ExecuteTurn(piece, targetPosition.x, targetPosition.y);
        }
    }
    public void Target(Chessman supporter, Chessman attacker, Chessman defender){
        if(supporter==piece && defender.color==piece.color && !Game._instance.currentMatch.AvengerActive){
            Game._instance.currentMatch.AvengerActive=true;
            Debug.Log("Avenger activated");
            readyToAvenge=true;
            targetPosition = new BoardPosition(defender.xBoard, defender.yBoard);
        }
    }
}
