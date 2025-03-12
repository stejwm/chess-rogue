using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StationarySlash", menuName = "Abilities/StationarySlash")]
public class StationarySlash : Ability
{
    private Chessman piece;
    private int x;
    private int y;
    
    public StationarySlash() : base("Stationary Slash", "Piece stays in position when it captures") {}


    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        //Game._instance.OnAttack.AddListener(AddBonus);
        Game._instance.OnPieceCaptured.AddListener(ListenForEnd);
        Game._instance.OnPieceBounced.AddListener(ReplaceOnBoard);
        piece.releaseCost+=20;
        piece.canStationarySlash=true;

    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnPieceCaptured.RemoveListener(ListenForEnd);
        piece.canStationarySlash=false; 

    }
    
    public void ListenForEnd(Chessman attacker, Chessman defender){
        if (attacker==piece){
            Debug.Log("Not moving");
            Game._instance.currentMatch.MovePiece(piece, piece.xBoard, piece.yBoard);
        }
    }

    public void ReplaceOnBoard(Chessman attacker, Chessman defender, bool isReduced){
        if (attacker==piece){
            Debug.Log("Not moving");
            Game._instance.currentMatch.MovePiece(piece, piece.xBoard, piece.yBoard);
        }
    }

}
