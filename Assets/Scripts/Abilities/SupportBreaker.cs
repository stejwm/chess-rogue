using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "SupportBreaker", menuName = "Abilities/SupportBreaker")]
public class SupportBreaker : Ability
{
    private Chessman piece;
    private List<Chessman> supporters = new List<Chessman>();
    
    public SupportBreaker() : base("Support Breaker", "When bounced, reduce all supporting enemies support by -1") {}


    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnAttack.AddListener(CheckForSupport);
        board.EventHub.OnPieceBounced.AddListener(ReduceSupport);
        board.EventHub.OnPieceCaptured.AddListener(ClearSupporters);
        base.Apply(board, piece);
    }

    public void CheckForSupport(Chessman targetPiece, int support, Tile targetedPosition){
        if (targetPiece==piece){
            eventHub.OnSupportAdded.AddListener(GatherSupporters);
        }

    }
    public override void Remove(Chessman piece)
    {
        eventHub.OnAttack.RemoveListener(CheckForSupport); 
        eventHub.OnPieceCaptured.RemoveListener(ClearSupporters); 
        eventHub.OnPieceBounced.RemoveListener(ReduceSupport); 

    }
    public void GatherSupporters(Chessman supporter, Chessman attacker, Chessman defender){
        supporters.Add(supporter);
    }

    public void ReduceSupport(Chessman attacker, Chessman defender){
        if(attacker==piece){
            foreach (Chessman enemy in supporters){
                    if (enemy.support>0){
                        enemy.support-=1;
                    }
            }
            piece.effectsFeedback.PlayFeedbacks();
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Support Breaker</gradient></color>",  supporters.Count +" pieces <color=red>-1</color> support");
            eventHub.OnSupportAdded.RemoveListener(GatherSupporters);
            supporters.Clear();   
               
        }
    
        
    }
    public void ClearSupporters(Chessman attacker, Chessman defender){
        if(attacker==piece){
            eventHub.OnSupportAdded.RemoveListener(GatherSupporters);
            supporters.Clear();  
        }
    }

}
