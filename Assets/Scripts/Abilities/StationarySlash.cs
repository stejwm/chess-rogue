using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "StationarySlash", menuName = "Abilities/StationarySlash")]
public class StationarySlash : Ability
{
    private Chessman piece;
    private int x;
    private int y;
    
    public StationarySlash() : base("Stationary Slash", "Does not move when capturing") {}


    public override void Apply(Board board, Chessman piece)
    {
        if (piece.abilities.OfType<StationarySlash>().FirstOrDefault()!=null){
           return;
        }
        this.piece = piece;
        piece.info += " " + abilityName;
        
        eventHub.OnPieceCaptured.AddListener(ListenForEnd);
        eventHub.OnPieceBounced.AddListener(ReplaceOnBoard);
        piece.canStationarySlash=true;
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnPieceCaptured.RemoveListener(ListenForEnd);
        eventHub.OnPieceBounced.RemoveListener(ReplaceOnBoard);
        piece.canStationarySlash=false; 

    }
    
    public void ListenForEnd(Chessman attacker, Chessman defender){
        if (attacker==piece){
            board.CurrentMatch.MovePiece(piece, piece.xBoard, piece.yBoard);
            AbilityLogger._instance.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Stationary Slash</gradient></color>",  $"Staying put on {BoardPosition.ConvertToChessNotation(piece.xBoard, piece.yBoard)}");
        }
    }

    public void ReplaceOnBoard(Chessman attacker, Chessman defender){
        if (attacker==piece){
            board.CurrentMatch.MovePiece(piece, piece.xBoard, piece.yBoard);
        }
    }

}
