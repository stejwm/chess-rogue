using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParalyzingBlow", menuName = "Abilities/ParalyzingBlow")]
public class ParalyzingBlow : Ability
{
    private Chessman piece;
    
    public ParalyzingBlow() : base("Paralyzing Blow", "When bounced, prevents the defender from moving next turn") {}


    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        GameManager._instance.OnPieceBounced.AddListener(Paralyze);
        base.Apply(piece);

        
    }

    public override void Remove(Chessman piece)
    {
        GameManager._instance.OnPieceBounced.RemoveListener(Paralyze); 

    }
    public void Paralyze(Chessman attacker, Chessman defender, bool isBounceReduced){
        if (attacker==piece){
            defender.paralyzed=true;
            piece.effectsFeedback.PlayFeedbacks();
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Paralyzing Blow</gradient></color>", " Paralyzed");
        }
    }

}
