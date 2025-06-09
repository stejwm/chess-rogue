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
        this.piece = piece;
        piece.info += " " + abilityName;

        GameManager._instance.OnPieceCaptured.AddListener(Thirst);
        GameManager._instance.OnAttack.AddListener(Decimate);
        GameManager._instance.OnPieceBounced.AddListener(EndThirst);
        GameManager._instance.OnGameEnd.AddListener(ResetMoveProfile);
        base.Apply(piece);

    }

    public override void Remove(Chessman piece)
    {
        GameManager._instance.OnPieceCaptured.RemoveListener(Thirst);
        GameManager._instance.OnAttack.RemoveListener(Decimate);
        GameManager._instance.OnPieceBounced.RemoveListener(EndThirst);
        GameManager._instance.OnGameEnd.RemoveListener(ResetMoveProfile);
    }

    public void Decimate(Chessman attacker, int support, bool isAttacking, BoardPosition targetedPosition){
        if(attacker==piece && isAttacking){
            GameManager._instance.isDecimating=true;
            if(!thirsting)
                startingProfile=piece.moveProfile;
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
            GameManager._instance.isDecimating=false;
            CoroutineRunner.instance.StartCoroutine(EnableSecondAttackCoroutine());
        }
        if(defender == piece && thirsting){
            GameManager._instance.isDecimating=false;
            List<GameObject> pieces;
            pieces = piece.owner.pieces;
            foreach (GameObject pieceObject in pieces)
            {
                pieceObject.GetComponent<Chessman>().isValidForAttack=true;
            }
            Debug.Log("Piece captured thirst over");
            thirsting=false;
            piece.moveProfile=startingProfile;
            GameManager._instance.currentMatch.BloodThirstOverride =false;
        }
    }

    public IEnumerator EnableSecondAttackCoroutine()
    {
        if (GameManager._instance.currentMatch.AvengerActive)
        {
            Debug.Log("Waiting for Avenging Strike to resolve...");
            yield return new WaitUntil(() => !GameManager._instance.currentMatch.AvengerActive); // Wait for AvengingStrike to finish
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
        GameManager._instance.currentMatch.BloodThirstOverride =true;
        Debug.Log("Blood thirst activated");
        piece.effectsFeedback.PlayFeedbacks();
        List<GameObject> pieces;
        if (piece.abilities.OfType<Betrayer>().FirstOrDefault()!=null){
            piece.moveProfile = new BetrayerMovement(new AttackOnlyMovement(startingProfile));
        }
        else
            piece.moveProfile = new AttackOnlyMovement(startingProfile);
        pieces = piece.owner.pieces;
        foreach (GameObject pieceObject in pieces)
        {
            pieceObject.GetComponent<Chessman>().isValidForAttack=false;
        }
        piece.isValidForAttack=true;
        
        GameManager._instance.currentMatch.MyTurn(piece.color);
        
        if (piece.moveProfile.GetValidMoves(piece).Count<=0){
            thirsting=false;
            piece.moveProfile=startingProfile;
            GameManager._instance.currentMatch.BloodThirstOverride =false;
            GameManager._instance.isDecimating=false;
            return;
        }
        AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Blood Thirst</gradient></color>",  " attack again");
        piece.owner.MakeMove(GameManager._instance.currentMatch);
    }

    private void EndThirst(Chessman attackingPiece, Chessman defendingPiece, bool isBounceReduced){
        if(attackingPiece==piece)
            GameManager._instance.isDecimating=false;
        if(attackingPiece==piece && thirsting){
            Debug.Log("Bounced, thirst over");
            thirsting=false;
            piece.moveProfile=startingProfile;
            GameManager._instance.currentMatch.BloodThirstOverride =false;
            GameManager._instance.isDecimating=false;
        }
    }

    private void ResetMoveProfile(PieceColor color){
        piece.moveProfile=startingProfile;
    }
}
