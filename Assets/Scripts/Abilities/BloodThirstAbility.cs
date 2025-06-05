using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "NewBloodThirstyAbility", menuName = "Abilities/BloodThirstyAbility")]
public class BloodThirstAbility : Ability
{
    private Chessman piece;
    private GameObject controller;
    MovementProfile startingProfile;
    private bool thirsting = false;
    
    public BloodThirstAbility() : base("Blood Thirst", "Attack again after capture, Decimates all captured pieces") {}


    public override void Apply(Chessman piece)
    {
        startingProfile=piece.moveProfile;
        Debug.Log($"Setting bloodthirst original profile, profile =null? {piece.moveProfile==null}");
        this.piece = piece;
        piece.info += " " + abilityName;

        Game._instance.OnPieceCaptured.AddListener(Thirst);
        Game._instance.OnAttackStart.AddListener(Decimate);
        Game._instance.OnPieceBounced.AddListener(EndThirst);
        Game._instance.OnGameEnd.AddListener(ResetMoveProfile);
        base.Apply(piece);

    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnPieceCaptured.RemoveListener(Thirst);
        Game._instance.OnAttackStart.RemoveListener(Decimate);
        Game._instance.OnPieceBounced.RemoveListener(EndThirst);
        Game._instance.OnGameEnd.RemoveListener(ResetMoveProfile);
    }

    public void Decimate(Chessman attacker, Chessman defender){
        if(attacker==piece){
            Game._instance.isDecimating=true;
            if(!thirsting){
                startingProfile=piece.moveProfile;
            }
        }
    }
    public void Thirst(Chessman attacker, Chessman defender)
    {
        /* if(attacker==piece && !thirsting){
            Debug.Log("Setting thirst to most recent profile");
            //startingProfile=piece.moveProfile; //update to currentMoveProfile in case it has changed
        } */
        if (attacker == piece)
        {
            thirsting=true;
            Game._instance.isDecimating=false;
            CoroutineRunner.instance.StartCoroutine(EnableSecondAttackCoroutine());
        }
        if(defender == piece && thirsting){
            Game._instance.isDecimating=false;
            List<GameObject> pieces;
            pieces = piece.owner.pieces;
            foreach (GameObject pieceObject in pieces)
            {
                pieceObject.GetComponent<Chessman>().isValidForAttack=true;
            }
            Debug.Log("Piece captured thirst over");
            thirsting=false;
            piece.moveProfile=startingProfile;
            Game._instance.currentMatch.BloodThirstOverride =false;
        }
    }

    public IEnumerator EnableSecondAttackCoroutine()
    {
        if (Game._instance.currentMatch.AvengerActive)
        {
            Debug.Log("Waiting for Avenging Strike to resolve...");
            yield return new WaitUntil(() => !Game._instance.currentMatch.AvengerActive); // Wait for AvengingStrike to finish
        }
        else{
           Debug.Log("No avenging strike "); 
        }
        EnableSecondAttack();
    }
    private void EnableSecondAttack()
    {   
        if(piece==null || !piece.gameObject.activeSelf)
            return;
        Game._instance.currentMatch.BloodThirstOverride =true;
        Debug.Log("Blood thirst activated");
        piece.effectsFeedback.PlayFeedbacks();
        List<GameObject> pieces;
        if (piece.abilities.OfType<Betrayer>().FirstOrDefault()!=null){
            piece.moveProfile = new BetrayerMovement(new AttackOnlyMovement(startingProfile));
        }
        else
        {
            if(startingProfile==null){
                Debug.Log("Starting profile is null, setting to default");
                startingProfile = piece.moveProfile;
            }
            piece.moveProfile = new AttackOnlyMovement(startingProfile);
        }
        pieces = piece.owner.pieces;
        foreach (GameObject pieceObject in pieces)
        {
            pieceObject.GetComponent<Chessman>().isValidForAttack=false;
        }
        piece.isValidForAttack=true;
        
        Game._instance.currentMatch.MyTurn(piece.color);
        
        if (piece.moveProfile.GetValidMoves(piece).Count<=0){
            thirsting=false;
            piece.moveProfile=startingProfile;
            Game._instance.currentMatch.BloodThirstOverride =false;
            Game._instance.isDecimating=false;
            return;
        }
        AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Blood Thirst</gradient></color>",  " attack again");
        piece.owner.MakeMove(Game._instance.currentMatch);
    }

    private void EndThirst(Chessman attackingPiece, Chessman defendingPiece, bool isBounceReduced){
        if(attackingPiece==piece)
            Game._instance.isDecimating=false;
        if(attackingPiece==piece && thirsting){
            Debug.Log("Bounced, thirst over");
            thirsting=false;
            piece.moveProfile=startingProfile;
            Game._instance.currentMatch.BloodThirstOverride =false;
            Game._instance.isDecimating=false;
        }
    }

    private void ResetMoveProfile(PieceColor color){
        piece.moveProfile=startingProfile;
        thirsting=false;
    }
}
