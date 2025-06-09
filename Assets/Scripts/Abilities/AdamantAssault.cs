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
        GameManager._instance.OnPieceBounced.AddListener(Assault);
        GameManager._instance.OnPieceCaptured.AddListener(EndAssault);
        base.Apply(piece);

    }

    public override void Remove(Chessman piece)
    {

        GameManager._instance.OnPieceBounced.RemoveListener(Assault);
        GameManager._instance.OnPieceCaptured.RemoveListener(EndAssault);

    }

    public void Assault(Chessman attacker, Chessman defender, bool isBounceReduced)
    {   
        
        
        if (attacker == piece && !alreadyBounced)
        {
            Debug.Log("Overriding turn for adamant assault");
            piece.effectsFeedback.PlayFeedbacks();
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Adamant Assault</gradient></color>", "attacking again");
            GameManager._instance.currentMatch.AdamantAssaultOverride =true;
            //Game._instance.currentMatch.PlayerTurn();
            GameManager._instance.currentMatch.ExecuteTurn(attacker, defender.xBoard, defender.yBoard);
            alreadyBounced=true;
        }else if(attacker==piece){
            alreadyBounced=false;
            GameManager._instance.currentMatch.AdamantAssaultOverride =false;
        }
    }
    public void EndAssault(Chessman attacker, Chessman defender){
        if(attacker==piece){
            alreadyBounced=false;
            GameManager._instance.currentMatch.AdamantAssaultOverride =false;
        }
    }
}
