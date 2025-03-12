using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monk", menuName = "Abilities/Monk")]
public class Monk : Ability
{
    private Chessman piece;
    
    public Monk() : base("Monk", "Gain +1 to random stat for each turn piece does not move") {}
    private int attackIncrease=0;
    private int defenseIncrease=0;
    private int supportIncrease=0;

    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnRawMoveEnd.AddListener(RawMoveEnd);
        Game._instance.OnAttack.AddListener(Check);
        piece.releaseCost+=10;
        //game.OnGameEnd.AddListener(RemoveBonus);

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
            RemoveBonus(piece, null, false);
        }
    }
    public void Check(Chessman movedPiece, int support, bool isAttacking, BoardPosition targetedPosition){
        if(!isAttacking)
            return;
        if (movedPiece.color == piece.color && movedPiece!=piece){
            AddBonus(piece, null);
        }
        else if(movedPiece==piece && piece.canStationarySlash){
            Debug.Log("Monk is selected to move");
            Game._instance.OnAttack.AddListener(CheckAgain);
            
        }
        else if(movedPiece==piece){
            RemoveBonus(null, null, false);
        }
    }

    public void CheckAgain(Chessman attacker, int support, bool isAttacking, BoardPosition targetedPosition){
        if(attacker.color == piece.color && attacker!=piece){
            AddBonus(piece, null);
        }
        else if(attacker==piece && piece.canStationarySlash){
            Game._instance.OnPieceBounced.AddListener(RemoveBonus);
            Game._instance.OnPieceCaptured.AddListener(AddBonus);
        }
        else if(attacker==piece){
            RemoveBonus(null, null, false);
        }
    }

    public void RemoveBonus(Chessman attacker, Chessman defender, bool isReduced){
        if(attacker==piece){
            Debug.Log("Removing monk bonus");
            piece.attackBonus=Mathf.Max(0, piece.attackBonus - attackIncrease);
            piece.defenseBonus=Mathf.Max(0, piece.defenseBonus - defenseIncrease);
            piece.supportBonus=Mathf.Max(0, piece.supportBonus - supportIncrease);
            attackIncrease=0;
            defenseIncrease=0;
            supportIncrease=0;
            Game._instance.OnPieceBounced.RemoveListener(RemoveBonus);
            Game._instance.OnPieceCaptured.RemoveListener(AddBonus);
        }
        
    }

    public void AddBonus(Chessman attacker, Chessman defender){
        if(attacker==piece){
            Debug.Log("Adding monk bonus");
            piece.effectsFeedback.PlayFeedbacks();
            int s = Random.Range (0, 3);
            switch(s){
                case 0: attackIncrease++; piece.attackBonus++; break;
                case 1: defenseIncrease++; piece.defenseBonus++; break;
                case 2: supportIncrease++; piece.supportBonus++; break;

            }
            Game._instance.OnPieceBounced.RemoveListener(RemoveBonus);
            Game._instance.OnPieceCaptured.RemoveListener(AddBonus);
        }
}

}
