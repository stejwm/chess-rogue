using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monk", menuName = "Abilities/Monk")]
public class Monk : Ability
{
    private Chessman piece;
    
    public Monk() : base("Monk", "Gain +1 to random stat every turn piece does not move, loses bonuses after first move is complete") {}
    private int attackIncrease=0;
    private int defenseIncrease=0;
    private int supportIncrease=0;

    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnRawMoveEnd.AddListener(RawMoveEnd);
        board.EventHub.OnAttack.AddListener(Check);
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnAttack.RemoveListener(Check); 
        eventHub.OnRawMoveEnd.RemoveListener(RawMoveEnd);

    }
    public void RawMoveEnd(Chessman movedPiece, Tile targetPosition){
        if(movedPiece.color==piece.color && movedPiece!=piece){
            AddBonus(piece, null);
        }
        else if(movedPiece==piece){
            RemoveBonus(piece, null, 0, 0);
        }
    }
    public void Check(Chessman movedPiece, int support, Tile targetedPosition){
        if (movedPiece.color == piece.color && movedPiece!=piece){
            AddBonus(piece, null);
        }
        else if(movedPiece==piece && piece.canStationarySlash){
            eventHub.OnPieceBounced.AddListener(AddBonusBounce);
            eventHub.OnPieceCaptured.AddListener(AddBonus);
            
        }
        else if(movedPiece==piece){
            eventHub.OnAttackEnd.AddListener(RemoveBonus);
        }
    }

    public void RemoveBonus(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport){
        if(attacker==piece){
            Debug.Log("Removing monk bonus");
            piece.attackBonus=Mathf.Max(-piece.attack, piece.attackBonus - attackIncrease);
            piece.defenseBonus=Mathf.Max(-piece.defense, piece.defenseBonus - defenseIncrease);
            piece.supportBonus=Mathf.Max(-piece.support, piece.supportBonus - supportIncrease);
            attackIncrease=0;
            defenseIncrease=0;
            supportIncrease=0;
            eventHub.OnPieceBounced.RemoveListener(AddBonusBounce);
            eventHub.OnPieceCaptured.RemoveListener(AddBonus);
        }
    }

    public void AddBonus(Chessman attacker, Chessman defender){
        if(attacker==piece){
            int s = Random.Range (0, 3);
            switch(s){
                case 0: attackIncrease++; piece.attackBonus++; 
                        //AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Monk</gradient></color>", $"<color=green>+1</color> attack");
                        break;
                case 1: defenseIncrease++; piece.defenseBonus++; 
                        //AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Monk</gradient></color>", $"<color=green>+1</color> defense");
                        break;
                case 2: supportIncrease++; piece.supportBonus++; 
                        //AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Monk</gradient></color>", $"<color=green>+1</color> support");
                        break;

            }
            eventHub.OnPieceBounced.RemoveListener(AddBonusBounce);
            eventHub.OnPieceCaptured.RemoveListener(AddBonus);
        }
    }
    public void AddBonusBounce(Chessman attacker, Chessman defender){
        if(attacker==piece){
            piece.effectsFeedback.PlayFeedbacks();
            int s = Random.Range (0, 3);
            switch(s){
                case 0: attackIncrease++; piece.attackBonus++; break;
                case 1: defenseIncrease++; piece.defenseBonus++; break;
                case 2: supportIncrease++; piece.supportBonus++; break;

            }
            eventHub.OnPieceBounced.RemoveListener(AddBonusBounce);
            eventHub.OnPieceCaptured.RemoveListener(AddBonus);
        }
    }

}
