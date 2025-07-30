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


    public override void Apply(Board board, Chessman piece)
    {
        if(piece.abilities.Contains(this)){
            return;
        }
        startingProfile =piece.moveProfile;
        this.piece = piece;
        piece.info += " " + abilityName;

        board.EventHub.OnPieceCaptured.AddListener(Thirst);
        board.EventHub.OnAttack.AddListener(Decimate);
        board.EventHub.OnPieceBounced.AddListener(EndThirst);
        board.EventHub.OnGameEnd.AddListener(ResetMoveProfile);
        base.Apply(board, piece);

    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnPieceCaptured.RemoveListener(Thirst);
        eventHub.OnAttack.RemoveListener(Decimate);
        eventHub.OnPieceBounced.RemoveListener(EndThirst);
        eventHub.OnGameEnd.RemoveListener(ResetMoveProfile);
    }

    public void Decimate(Chessman attacker, int support, Tile targetedPosition){
        if(attacker==piece){
            board.CurrentMatch.isDecimating=true;
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
            board.CurrentMatch.isDecimating=false;
            CoroutineRunner.instance.StartCoroutine(EnableSecondAttackCoroutine());
        }
        if(defender == piece && thirsting){
            board.CurrentMatch.isDecimating=false;
            List<GameObject> pieces;
            pieces = piece.owner.pieces;
            foreach (GameObject pieceObject in pieces)
            {
                pieceObject.GetComponent<Chessman>().isValidForAttack=true;
            }
            Debug.Log("Piece captured thirst over");
            thirsting=false;
            piece.moveProfile=startingProfile;
            board.CurrentMatch.BloodThirstOverride =false;
        }
    }

    public IEnumerator EnableSecondAttackCoroutine()
    {
        if (board.CurrentMatch.AvengerActive)
        {
            Debug.Log("Waiting for Avenging Strike to resolve...");
            yield return new WaitUntil(() => !board.CurrentMatch.AvengerActive); // Wait for AvengingStrike to finish
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
        board.CurrentMatch.BloodThirstOverride =true;
        Debug.Log("Blood thirst activated");
        //piece.effectsFeedback.PlayFeedbacks();
        List<GameObject> pieces;
        if (piece.abilities.OfType<Betrayer>().FirstOrDefault()!=null){
            piece.moveProfile = new BetrayerMovement(board, new AttackOnlyMovement(board, startingProfile));
        }
        else
            piece.moveProfile = new AttackOnlyMovement(board, startingProfile);
        pieces = piece.owner.pieces;
        foreach (GameObject pieceObject in pieces)
        {
            pieceObject.GetComponent<Chessman>().isValidForAttack=false;
        }
        piece.isValidForAttack=true;
        
        board.CurrentMatch.MyTurn(piece.color);
        
        if (piece.moveProfile.GetValidMoves(piece).Count<=0){
            thirsting=false;
            piece.moveProfile=startingProfile;
            board.CurrentMatch.BloodThirstOverride =false;
            board.CurrentMatch.isDecimating=false;
            return;
        }
        board.AbilityLogger.AddAbilityLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Blood Thirst</gradient></color>",  " attack again");
        piece.owner.MakeMove(board.CurrentMatch);
    }

    private void EndThirst(Chessman attackingPiece, Chessman defendingPiece){
        if(attackingPiece==piece)
            board.CurrentMatch.isDecimating=false;
        if(attackingPiece==piece && thirsting){
            board.AbilityLogger.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Blood Thirst</gradient></color>" +  "This will have to do. Thirst Over");
            thirsting =false;
            piece.moveProfile=startingProfile;
            board.CurrentMatch.BloodThirstOverride =false;
            board.CurrentMatch.isDecimating=false;
        }
    }

    private void ResetMoveProfile(PieceColor color){
        piece.moveProfile=startingProfile;
    }
}
