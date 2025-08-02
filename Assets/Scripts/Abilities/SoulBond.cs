using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System.Runtime.InteropServices;

[CreateAssetMenu(fileName = "SoulBond", menuName = "Abilities/SoulBond")]
public class SoulBond : Ability
{
    private Chessman piece;
    private int bonus = 5;
    List<Chessman> soulBondedPieces = new List<Chessman>();
    
    public SoulBond() : base("Soul Bond", "Permanently gain +5 to each stat for each soul bonded piece, when any soul bonded piece is captured, all are decimated") { }

    public override void Apply(Board board, Chessman piece)
    {
        if(piece.abilities.Contains(this)){
            return;
        }
        this.piece = piece;
        foreach (GameObject otherPiece in piece.owner.pieces)
        {
            if (otherPiece.GetComponent<Chessman>().abilities.Contains(this))
            {
                Debug.Log("Another piece already soul bonded lead");
                board.EventHub.RaiseSoulBonded(piece);
                base.Apply(board, piece);
                return;
            }
        }
        Debug.Log("First sol bonded piece");
        board.EventHub.OnPieceCaptured.AddListener(Capture);
        board.EventHub.OnSoulBonded.AddListener(Increase);
        board.EventHub.OnAttackStart.AddListener(Decimate);
        board.EventHub.OnAttackEnd.AddListener(RemoveDecimate);
        board.EventHub.OnSoulBondRemoved.AddListener(RemovePiece);
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnPieceCaptured.RemoveListener(Capture);
        eventHub.OnSoulBonded.RemoveListener(Increase);
        eventHub.OnAttackStart.RemoveListener(Decimate);
        eventHub.OnAttackEnd.RemoveListener(RemoveDecimate);
        board.EventHub.RaiseSoulBondRemoved(piece);

    }

    ~SoulBond()
    {
        if (piece != null)
        {
            eventHub.OnPieceCaptured.RemoveListener(Capture); 
            eventHub.OnSoulBonded.RemoveListener(Increase); 
            eventHub.OnAttackStart.RemoveListener(Decimate);
            eventHub.OnAttackEnd.RemoveListener(RemoveDecimate);
        }
    }

    public void Increase(Chessman cm)
    {
        if (cm.owner == piece.owner)
        {
            soulBondedPieces.Add(cm);
            piece.attack += bonus;
            piece.defense += bonus;
            piece.support += bonus;
            foreach (Chessman soulBonder in soulBondedPieces)
            {
                soulBonder.attack += bonus;
                soulBonder.defense += bonus;
                soulBonder.support += bonus;
            }
        }
        
        
    }
    public void Decimate(Chessman attacker, Chessman defender){
        if (soulBondedPieces.Contains(defender) || piece==defender)
        {
            board.CurrentMatch.isDecimating = true;
        }
    }
    public void RemovePiece(Chessman cm){
        soulBondedPieces.Remove(cm);
    }
    public void RemoveDecimate(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport)
    {
        if (soulBondedPieces.Contains(defender) || piece == defender)
            board.CurrentMatch.isDecimating = false;
    }
    public void Capture(Chessman attacker, Chessman defender){
        if (soulBondedPieces.Contains(defender) || defender == piece)
        {
            board.AbilityLogger.AddAbilityLogToQueue($"<sprite=\"{defender.color}{defender.type}\" name=\"{defender.color}{defender.type}\"><color=white><gradient=\"AbilityGradient\">Soul bond</gradient></color>", $"Bonds could not hold");
            eventHub.OnPieceCaptured.RemoveListener(Capture);
            eventHub.OnSoulBondRemoved.RemoveListener(RemovePiece);

            //destroy all other soulbonded pieces
            foreach (Chessman cm in soulBondedPieces)
            {
                if (cm == defender)
                    continue;
                board.AbilityLogger.AddLogToQueue($"<sprite=\"{cm.color}{cm.type}\" name=\"{cm.color}{cm.type}\"><color=white><gradient=\"AbilityGradient\">Soul bond</gradient></color>" + $" {cm.name} decimated on {BoardPosition.ConvertToChessNotation(cm.xBoard, cm.yBoard)}");
                board.ClearPosition(cm.xBoard, cm.yBoard);
                board.GetTileAt(cm.xBoard, cm.yBoard).SetBloodTile();
                eventHub.RaisePieceCaptured(attacker, cm);
                eventHub.RaisePieceRemoved(cm);
                if (cm.type == PieceType.King && cm.owner == board.Hero)
                {
                    board.CurrentMatch.EndGame();
                }
                else if (cm.type == PieceType.King)
                {
                    if (board.CurrentMatch != null)
                        board.CurrentMatch.EndMatch();
                }
                cm.DestroyPiece();
                if(attacker.owner == board.Hero)
                    board.Hero.enemiesDecimated++;
                else
                    board.Hero.myPieceDecimated++;
            }

            //destroy this piece if it's not the one decimated
            if (piece != defender)
            {
                board.AbilityLogger.AddLogToQueue($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Soul bond</gradient></color>" + $" {piece.name} decimated on {BoardPosition.ConvertToChessNotation(piece.xBoard, piece.yBoard)}");
                board.ClearPosition(piece.xBoard, piece.yBoard);
                board.GetTileAt(piece.xBoard, piece.yBoard).SetBloodTile();
                eventHub.RaisePieceCaptured(attacker, piece);
                eventHub.RaisePieceRemoved(piece);
                if (piece.type == PieceType.King && piece.owner == board.Hero)
                {
                    board.CurrentMatch.EndGame();
                }
                else if (piece.type == PieceType.King)
                {
                    if (board.CurrentMatch != null)
                        board.CurrentMatch.EndMatch();
                }
                piece.DestroyPiece();
                if(attacker.owner == board.Hero)
                    board.Hero.enemiesDecimated++;
                else
                    board.Hero.myPieceDecimated++;
            }
            if(board.CurrentMatch!=null)
                board.CurrentMatch.isDecimating = false;
        }
    }

}