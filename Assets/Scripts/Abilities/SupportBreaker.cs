using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "SupportBreaker", menuName = "Abilities/SupportBreaker")]
public class SupportBreaker : Ability
{
    private Chessman piece;
    private List<Chessman> supporters = new List<Chessman>();
    
    public SupportBreaker() : base("Support Breaker", "When bounced reduces all opponents supporting pieces support by 1") {}


    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnAttack.AddListener(CheckForSupport);
        Game._instance.OnPieceBounced.AddListener(ReduceSupport);
        Game._instance.OnPieceCaptured.AddListener(ClearSupporters);
        piece.releaseCost+=20;
        base.Apply(piece);
    }

    public void CheckForSupport(Chessman targetPiece, int support, bool isAttacking, BoardPosition targetedPosition){
        if (targetPiece==piece && isAttacking){
            Game._instance.OnSupportAdded.AddListener(GatherSupporters);
        }

    }
    public override void Remove(Chessman piece)
    {
        Game._instance.OnAttack.RemoveListener(CheckForSupport); 
        Game._instance.OnPieceCaptured.RemoveListener(ClearSupporters); 
        Game._instance.OnPieceBounced.RemoveListener(ReduceSupport); 

    }
    public void GatherSupporters(Chessman supporter, Chessman attacker, Chessman defender){
        supporters.Add(supporter);
    }

    public void ReduceSupport(Chessman attacker, Chessman defender, bool isReduced){
        if(attacker==piece){
            foreach (Chessman enemy in supporters){
                    if (enemy.support>0){
                        enemy.support-=1;
                    }
            }
            piece.effectsFeedback.PlayFeedbacks();
            AbilityLogger._instance.LogAbilityUsage($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Support Breaker</gradient></color>",  supporters.Count +" pieces -1 support");
            Game._instance.OnSupportAdded.RemoveListener(GatherSupporters);
            supporters.Clear();   
               
        }
    
        
    }
    public void ClearSupporters(Chessman attacker, Chessman defender){
        if(attacker==piece){
            Game._instance.OnSupportAdded.RemoveListener(GatherSupporters);
            supporters.Clear();  
        }
    }

}
