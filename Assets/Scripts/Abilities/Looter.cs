using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Looter", menuName = "Abilities/Looter")]
public class Looter : Ability
{
    private Chessman piece;
    
    public Looter() : base("Looter", "+4 coins if survives to end of match") {}

    public override void Apply(Board board, Chessman piece)
    {
        this.piece = piece;
        board.EventHub.OnGameEnd.AddListener(Loot);
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnGameEnd.RemoveListener(Loot); 

    }
    public void Loot(PieceColor color){
        if(color == piece.color){
            //board.AbilityLogger.AddAbilityLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">PickPocket</gradient></color>", $"<color=yellow>+3</color> coins");
            piece.owner.playerCoins+=4;
        }
    }

}