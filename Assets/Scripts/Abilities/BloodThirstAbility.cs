using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBloodThirstyAbility", menuName = "Abilities/BloodThirstyAbility")]
public class BloodThirstAbility : Ability
{
    private Chessman piece;
    private GameObject controller;
    MovementProfile startingProfile;
    
    public BloodThirstAbility() : base("Blood Thirst", "Attack again after capture.") {}


    public override void Apply(Chessman piece)
    {
        GameObject controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();
        startingProfile=piece.moveProfile;
        this.piece = piece;
        piece.info += " " + abilityName;
        //game.OnPieceCaptured += Thirst;
        Debug.Log(game==null);

        game.OnPieceCaptured.AddListener(Thirst);
        Debug.Log("Blood Thirst ability applied, subscribed to OnAttackTriggered");
    }

    public override void Remove(Chessman piece)
    {

        //game.OnPieceCaptured -= Thirst;
        game.OnPieceCaptured.RemoveListener(Thirst);  // Unsubscribe from the event

    }

    public void Thirst(Chessman attacker)
    {
        Debug.Log("Thirsting");
        if (attacker == piece)
        {
            EnableSecondAttack();
        }
    }

    private void EnableSecondAttack()
    {
        ArrayList pieces;
        piece.moveProfile = new AttackOnlyMovement(startingProfile);
        game.NextTurn();
        if (piece.color==PieceColor.White){
            pieces=game.playerWhite;
        }else
            pieces=game.playerBlack;
        foreach (GameObject pieceObject in pieces)
        {
            pieceObject.GetComponent<Chessman>().isValidForAttack=false;
        }
        piece.isValidForAttack=true;
        if (piece.moveProfile.GetValidMoves(piece).Count<=0){
            Debug.Log("No valid attack found, thirst over");
            piece.moveProfile=startingProfile;
            game.NextTurn();
        }

        
    }
}
