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

    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnRawMoveEnd.AddListener(RawMoveEnd);
        Game._instance.OnAttack.AddListener(Check);
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnAttack.RemoveListener(Check); 
        Game._instance.OnRawMoveEnd.RemoveListener(RawMoveEnd);

    }
    public void RawMoveEnd(Chessman movedPiece, BoardPosition targetPosition){
        if(movedPiece.color==piece.color && movedPiece!=piece){
            AddBonus(piece, null);
        }
        else if(movedPiece==piece){
            RemoveBonus(piece, null, 0, 0);
        }
    }
    public void Check(Chessman movedPiece, int support, bool isAttacking, BoardPosition targetedPosition){
        if(!isAttacking)
            return;
        if (movedPiece.color == piece.color && movedPiece!=piece){
            AddBonus(piece, null);
        }
        else if(movedPiece==piece && piece.canStationarySlash){
            Game._instance.OnPieceBounced.AddListener(AddBonusBounce);
            Game._instance.OnPieceCaptured.AddListener(AddBonus);
            
        }
        else if(movedPiece==piece){
            Game._instance.OnAttackEnd.AddListener(RemoveBonus);
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
            Game._instance.OnPieceBounced.RemoveListener(AddBonusBounce);
            Game._instance.OnPieceCaptured.RemoveListener(AddBonus);
        }
    }

    public void AddBonus(Chessman attacker, Chessman defender){
        if(attacker==piece){
            int s = Random.Range (0, 3);
            switch(s){
                case 0: attackIncrease++; piece.attackBonus++; 
                        AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Monk</gradient></color>", $"<color=green>+1</color> attack");
                        break;
                case 1: defenseIncrease++; piece.defenseBonus++; 
                        AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Monk</gradient></color>", $"<color=green>+1</color> defense");
                        break;
                case 2: supportIncrease++; piece.supportBonus++; 
                        AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Monk</gradient></color>", $"<color=green>+1</color> support");
                        break;

            }
            Game._instance.OnPieceBounced.RemoveListener(AddBonusBounce);
            Game._instance.OnPieceCaptured.RemoveListener(AddBonus);
        }
    }
    public void AddBonusBounce(Chessman attacker, Chessman defender, bool isReduced){
        if(attacker==piece){
            piece.effectsFeedback.PlayFeedbacks();
            int s = Random.Range (0, 3);
            switch(s){
                case 0: attackIncrease++; piece.attackBonus++; break;
                case 1: defenseIncrease++; piece.defenseBonus++; break;
                case 2: supportIncrease++; piece.supportBonus++; break;

            }
            Game._instance.OnPieceBounced.RemoveListener(AddBonusBounce);
            Game._instance.OnPieceCaptured.RemoveListener(AddBonus);
        }
    }

}
