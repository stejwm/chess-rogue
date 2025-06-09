using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Momentum", menuName = "Abilities/Momentum")]
public class Momentum : Ability
{
    private Chessman piece;
    private int bonus;
    public Momentum() : base("Momentum", "Gain +1 to attack per move, resets after being bounced or not moving") {}

    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        GameManager._instance.OnRawMoveEnd.AddListener(RawMoveEnd);
        GameManager._instance.OnPieceBounced.AddListener(RemoveBounce);
        GameManager._instance.OnPieceCaptured.AddListener(AddCapture);
        GameManager._instance.OnGameEnd.AddListener(GameEndRemove);
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        //Game._instance.OnAttack.RemoveListener(Check); 
        GameManager._instance.OnRawMoveEnd.RemoveListener(RawMoveEnd);

    }
    public void RawMoveEnd(Chessman movedPiece, BoardPosition targetPosition){
        if(movedPiece==piece){
            AddBonus();
        }
        else if(movedPiece.color==piece.color && movedPiece!=piece){
            RemoveBonus();
        }
    }

    public void RemoveBonus(){
        piece.attackBonus-=bonus;
        bonus=0;
    }

    public void RemoveBounce(Chessman attacker, Chessman defender, bool didBounceReduce){
        if(attacker==piece){
            RemoveBonus();
        }
    }

    public void GameEndRemove(PieceColor color){
        RemoveBonus();
    }

    public void AddBonus(){
        bonus+=1;
        piece.attackBonus+=1;
        AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Momentum</gradient></color>", $"<color=green>+{bonus} attack, keep moving</color>");
    }

    public void AddCapture(Chessman attacker, Chessman defender){
        if(attacker==piece){
            AddBonus();
        }
        
    }

}
