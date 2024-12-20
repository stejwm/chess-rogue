using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AdamantAssault", menuName = "Abilities/AdamantAssault")]
public class AdamantAssault : Ability
{
    private Chessman piece;
    MovementProfile startingProfile;
    bool alreadyBounced = false;
    
    public AdamantAssault() : base("Adamant Assault", "Automatically trigger second attack after being bounced") {}


    public override void Apply(Chessman piece)
    {
        //startingProfile=piece.moveProfile;
        this.piece = piece;
        piece.info += " " + abilityName;
        piece.releaseCost+=15;
        //game.OnPieceCaptured += Thirst;
        //Debug.Log(game==null);

        Game._instance.OnPieceBounced.AddListener(Assault);
        Game._instance.OnPieceCaptured.AddListener(EndAssault);
    }

    public override void Remove(Chessman piece)
    {

        //game.OnPieceCaptured -= Thirst;
        Game._instance.OnPieceBounced.RemoveListener(Assault);  // Unsubscribe from the event

    }

    public void Assault(Chessman attacker, Chessman defender, bool isBounceReduced)
    {   
        
        
        if (attacker == piece && !alreadyBounced)
        {
            Debug.Log("turn override");
            Game._instance.currentMatch.turnOverride =true;
            //Game._instance.currentMatch.PlayerTurn();
            Game._instance.currentMatch.ExecuteTurn(attacker, defender.xBoard, defender.yBoard);
            alreadyBounced=true;
        }else if(attacker==piece){
            alreadyBounced=false;
            Game._instance.currentMatch.turnOverride =false;
        }
    }
    public void EndAssault(Chessman attacker){
        if(attacker==piece){
            alreadyBounced=false;
            Game._instance.currentMatch.turnOverride =false;
        }
    }
}
