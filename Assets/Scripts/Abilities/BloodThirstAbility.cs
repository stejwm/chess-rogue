using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Game._instance.OnPieceBounced.AddListener(EndThirst);
        Game._instance.OnGameEnd.AddListener(ResetMoveProfile);
        piece.releaseCost+=20;
    }

    public override void Remove(Chessman piece)
    {

        //game.OnPieceCaptured -= Thirst;
        Game._instance.OnPieceCaptured.RemoveListener(Thirst);  // Unsubscribe from the event

    }

    public void Thirst(Chessman attacker)
    {
        //Debug.Log("Thirsting");
        if (attacker == piece)
        {
            thirsting=true;
            EnableSecondAttack();
        }
    }

    private void EnableSecondAttack()
    {   
        Game._instance.currentMatch.BloodThirstOverride =true;
        
        List<GameObject> pieces;
        piece.moveProfile = new AttackOnlyMovement(startingProfile);
        pieces = piece.owner.pieces;
        foreach (GameObject pieceObject in pieces)
        {
            pieceObject.GetComponent<Chessman>().isValidForAttack=false;
        }
        piece.isValidForAttack=true;
        Debug.Log("Only thirster valid for attack");
        Game._instance.currentMatch.MyTurn(piece.color);
        if (piece.moveProfile.GetValidMoves(piece).Count<=0){
            thirsting=false;
            Debug.Log("No valid attack found, thirst over");
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
                //Game._instance.currentMatch.NextTurn();
            }
    }

    private void ResetMoveProfile(PieceColor color){
        piece.moveProfile=startingProfile;
    }
}
