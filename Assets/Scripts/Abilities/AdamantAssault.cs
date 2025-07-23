using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AdamantAssault", menuName = "Abilities/AdamantAssault")]
public class AdamantAssault : Ability
{
    private Chessman piece;
    MovementProfile startingProfile;
    bool alreadyBounced = false;
    
    
    public AdamantAssault() : base("Adamant Assault", "Automatically trigger second attack after being bounced") { }


    public override void Apply(Board board, Chessman piece)
    {
        if(piece.abilities.Contains(this)){
            return;
        }
        //startingProfile=piece.moveProfile;
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnPieceBounced.AddListener(Assault);
        board.EventHub.OnPieceCaptured.AddListener(EndAssault);
        base.Apply(board, piece);

    }

    public override void Remove(Chessman piece)
    {

        eventHub.OnPieceBounced.RemoveListener(Assault);
        eventHub.OnPieceCaptured.RemoveListener(EndAssault);

    }

    public void Assault(Chessman attacker, Chessman defender)
    {   
        
        
        if (attacker == piece && !alreadyBounced)
        {
            Debug.Log("Overriding turn for adamant assault");
            piece.effectsFeedback.PlayFeedbacks();
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Adamant Assault</gradient></color>", "attacking again");
            board.CurrentMatch.AdamantAssaultOverride =true;
            //Game._instance.currentMatch.PlayerTurn();
            alreadyBounced=true;
            board.CurrentMatch.ExecuteTurn(attacker, defender.xBoard, defender.yBoard);
            
        }else if(attacker==piece){
            alreadyBounced=false;
            board.CurrentMatch.AdamantAssaultOverride =false;
        }
    }
    public void EndAssault(Chessman attacker, Chessman defender){
        if(attacker==piece){
            alreadyBounced=false;
            board.CurrentMatch.AdamantAssaultOverride =false;
        }
    }
}
