using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Vampire", menuName = "Abilities/Vampire")]
public class Vampire : Ability
{
    private Chessman piece;
    private int bonus = 0;

    public Vampire() : base("Vampire", "+1 to all stats on dark squares, -1 to all stats on light squares. transfer ability when attacking") { }

    public override void Apply(Board board, Chessman piece)
    {
        if (piece.abilities.Contains(this))
        {
            return;
        }
        this.piece = piece;

        board.EventHub.OnMove.AddListener(AddBonus);
        //board.EventHub.OnAttackEnd.AddListener(SuckBlood);
        board.EventHub.OnPieceCaptured.AddListener(SuckBlood);
        board.EventHub.OnChessMatchStart.AddListener(MatchStartBonus);
        board.EventHub.OnPieceBounced.AddListener(PossibleReset);
        this.board = board;
        if (board.CurrentMatch != null)
        {
            AddBonus(piece, board.GetTileAt(piece.xBoard, piece.yBoard));
        }
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnMove.RemoveListener(AddBonus);
        eventHub.OnPieceCaptured.RemoveListener(SuckBlood);
        eventHub.OnChessMatchStart.RemoveListener(MatchStartBonus);
        eventHub.OnPieceBounced.RemoveListener(PossibleReset);
    }
    public void MatchStartBonus()
    {
        if (piece)
        {
            //Debug.Log($"Resetting previous bonus to  0 and applying vamp at match start for {piece.name}");
            bonus = 0;
            AddBonus(piece, piece.startingPosition);
        }
        else
        {
            Remove(null);
            Destroy(this);
        }
    }

    public void AddBonus(Chessman mover, Tile targetPosition)
    {
        int currentBonus = bonus;
        if (mover == piece)
        {
            if (targetPosition.GetColor() == PieceColor.Black)
            {
                bonus = 1;
            }
            else
            {
                bonus = -1;
            }

            if (currentBonus != bonus && currentBonus != 0)
            {
                AdjustBonus(piece, bonus * 2);
            }
            else if (currentBonus == 0)
            {
                AdjustBonus(piece, bonus);
            }
        }
    }

    private void AdjustBonus(Chessman piece, int bonusChange)
    {
        if (piece == null) return;
        
        //Debug.Log($"Bonus change of {bonusChange} being applied to {piece.name}. Current bonuses are a:{piece.attackBonus}, d:{piece.defenseBonus}, s:{piece.supportBonus}");
        piece.SetBonus(StatType.Attack, Mathf.Max(-piece.attack, piece.attackBonus + bonusChange), abilityName);
        piece.SetBonus(StatType.Defense, Mathf.Max(-piece.defense, piece.defenseBonus + bonusChange), abilityName);
        piece.SetBonus(StatType.Support, Mathf.Max(-piece.support, piece.supportBonus + bonusChange), abilityName);
        //Debug.Log($"Bonus change of {bonusChange} should be applied to {piece.name}. Bonuses are now:{piece.attackBonus}, d:{piece.defenseBonus}, s:{piece.supportBonus}");

        


         if (bonusChange > 0)
            board.AbilityLogger.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Vampire</gradient></color>"+ $"<color=green>+1</color> to all stats on {BoardPosition.ConvertToChessNotation(piece.xBoard, piece.yBoard)}");
        else
            board.AbilityLogger.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Vampire</gradient></color>"+ $"<color=red>-1</color> to all stats on {BoardPosition.ConvertToChessNotation(piece.xBoard, piece.yBoard)}");
 

    }

    public void SuckBlood(Chessman attacker, Chessman defender)
    {
        if (attacker == piece)
        {
            //defender.AddAbility(board, AbilityDatabase.Instance.GetAbilityByName("Vampire"));
            piece.owner.playerBlood++;
            board.AbilityLogger.AddAbilityLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Vampire</gradient></color>", $"<color=red>+1</color> blood");

        }
    }
    public void RemoveBonus()
    {
        //Debug.Log($"removing bonus of {bonus} on {piece.name}. Current bonuses are a:{piece.attackBonus}, d:{piece.defenseBonus}, s:{piece.supportBonus}");
        piece.SetBonus(StatType.Attack, Mathf.Max(-piece.attack, piece.attackBonus - bonus), abilityName);
        piece.SetBonus(StatType.Defense, Mathf.Max(-piece.defense, piece.defenseBonus - bonus), abilityName);
        piece.SetBonus(StatType.Support, Mathf.Max(-piece.support, piece.supportBonus - bonus), abilityName);
        //Debug.Log($"bonuses of {bonus} should be removed on {piece.name}. Bonuses are now a:{piece.attackBonus}, d:{piece.defenseBonus}, s:{piece.supportBonus}");
        bonus = 0;
    }
    public void PossibleReset(Chessman attacker, Chessman defender)
    {
        if (attacker == piece)
        {
            AddBonus(piece, board.GetTileAt(piece.xBoard, piece.yBoard));
        }
    }
    ~Vampire()
    {
        if (piece != null)
        {
            eventHub.OnMove.RemoveListener(AddBonus);
            eventHub.OnPieceCaptured.RemoveListener(SuckBlood);
            eventHub.OnChessMatchStart.RemoveListener(MatchStartBonus);
            eventHub.OnPieceBounced.RemoveListener(PossibleReset);
        }
    }
}
