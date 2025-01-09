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

    
    public AvengingStrike() : base("Avenging Strike", "Automatically trigger attack if supported piece was captured") {}


    public override void Apply(Chessman piece)
    {
        //startingProfile=piece.moveProfile;
        this.piece = piece;
        piece.info += " " + abilityName;
        piece.releaseCost+=15;
        //game.OnPieceCaptured += Thirst;
        //Debug.Log(game==null);

        Game._instance.OnSupportAdded.AddListener(Target);
        Game._instance.OnPieceCaptured.AddListener(Avenge);
        Game._instance.OnPieceBounced.AddListener(EndAvenge);
    }

    public override void Remove(Chessman piece)
    {

        //game.OnPieceCaptured -= Thirst;
        Game._instance.OnPieceCaptured.RemoveListener(Avenge);  // Unsubscribe from the event

    }

    public void EndAvenge(Chessman attacker, Chessman defender, bool isBounceReduced){
        Game._instance.currentMatch.AvengingStrikeOverride =false;
        readyToAvenge=false;
    }

    public void Avenge(Chessman attacker)
    {
        if(attacker==piece && readyToAvenge){
            Game._instance.currentMatch.AvengingStrikeOverride =false;
            readyToAvenge=false;     
        }
        else if (readyToAvenge)
        {
            Debug.Log("Overriding turn for Avenging Strike");
            AbilityLogger._instance.LogAbilityUsage($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Avenging Strike</gradient></color>", "attacking");
            Game._instance.currentMatch.AvengingStrikeOverride =true;
            if(Game._instance.currentMatch.BloodThirstOverride)
                Game._instance.currentMatch.MyTurn(piece.color);
            Game._instance.currentMatch.ExecuteTurn(piece, targetPosition.x, targetPosition.y);
        }
    }
    public void Target(Chessman supporter, Chessman attacker, Chessman defender){
        if(supporter==piece && defender.color==piece.color){
            readyToAvenge=true;
            targetPosition = new BoardPosition(defender.xBoard, defender.yBoard);
        }
    }
}
