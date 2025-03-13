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
    
    public BloodThirstAbility() : base("Blood Thirst", "Attack again after capture.") {}


    public override void Apply(Chessman piece)
    {
        startingProfile=piece.moveProfile;
        this.piece = piece;
        piece.info += " " + abilityName;

        Game._instance.OnPieceCaptured.AddListener(Thirst);
        Game._instance.OnAttack.AddListener(Decimate);
        Game._instance.OnPieceBounced.AddListener(EndThirst);
        Game._instance.OnGameEnd.AddListener(ResetMoveProfile);
        piece.releaseCost+=20;
        base.Apply(piece);

    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnPieceCaptured.RemoveListener(Thirst);
        Game._instance.OnAttack.RemoveListener(Decimate);
        Game._instance.OnPieceBounced.RemoveListener(EndThirst);
        Game._instance.OnGameEnd.RemoveListener(ResetMoveProfile);
    }

    public void Decimate(Chessman attacker, int support, bool isAttacking, BoardPosition targetedPosition){
        if(attacker==piece && isAttacking)
            Game._instance.isDecimating=true;
    }
    public void Thirst(Chessman attacker, Chessman defender)
    {
        //Debug.Log("Thirsting");
        if (attacker == piece)
        {
            thirsting=true;
            Game._instance.isDecimating=false;
            CoroutineRunner.instance.StartCoroutine(EnableSecondAttackCoroutine());
        }
        if(defender == piece && thirsting){
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
        if(!piece.gameObject.activeSelf)
            return;
        Game._instance.currentMatch.BloodThirstOverride =true;
        Debug.Log("Blood thirst activated");
        piece.effectsFeedback.PlayFeedbacks();
        List<GameObject> pieces;
        startingProfile = piece.moveProfile;
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
        
        Game._instance.currentMatch.MyTurn(piece.color);
        
        if (piece.moveProfile.GetValidMoves(piece).Count<=0){
            thirsting=false;
            piece.moveProfile=startingProfile;
            Game._instance.currentMatch.BloodThirstOverride =false;
            return;
        }
        AbilityLogger._instance.LogAbilityUsage($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Blood Thirst</gradient></color>",  " attack again");
        piece.owner.MakeMove(Game._instance.currentMatch);
    }

    private void EndThirst(Chessman attackingPiece, Chessman defendingPiece, bool isBounceReduced){
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
    }
}
