using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "SoulBond", menuName = "Abilities/SoulBond")]
public class SoulBond : Ability
{
    private Chessman piece;
    private int bonus = 0;
    
    public SoulBond() : base("Soul Bond", "Permanently gain +5 to each stat for each soul bonded piece, when any soul bonded piece is captured, all are decimated") {}

    public override void Apply(Board board, Chessman piece)
    {
        if(piece.abilities.Contains(this)){
            return;
        }
        this.piece = piece;
        piece.info += " " + abilityName;
        board.EventHub.OnPieceCaptured.AddListener(Capture);
        board.EventHub.OnSoulBonded.AddListener(Increase);
        board.EventHub.OnMove.AddListener(Decimate);
        board.EventHub.OnAttackEnd.AddListener(RemoveDecimate);
        piece.owner.soulBondedPieces++;
        board.EventHub.RaiseSoulBonded();
        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnPieceCaptured.RemoveListener(Capture); 
        eventHub.OnSoulBonded.RemoveListener(Increase); 
        eventHub.OnMove.RemoveListener(Decimate);
        eventHub.OnAttackEnd.RemoveListener(RemoveDecimate);
        piece.owner.soulBondedPieces--;
        eventHub.OnSoulBonded.Invoke();

    }

    ~SoulBond()
    {
        if (piece != null)
        {
            eventHub.OnPieceCaptured.RemoveListener(Capture); 
            eventHub.OnSoulBonded.RemoveListener(Increase); 
            eventHub.OnMove.RemoveListener(Decimate);
            eventHub.OnAttackEnd.RemoveListener(RemoveDecimate);
        }
    }

    public void Increase(){
        piece.attack-=bonus;
        piece.defense-=bonus;
        piece.support-=bonus;
        bonus = 5*(piece.owner.soulBondedPieces-1);
        piece.attack+=bonus;
        piece.defense+=bonus;
        piece.support+=bonus;
        
    }
    public void Decimate(Chessman attacker, Tile position){
        if(position.X== piece.xBoard && position.Y== piece.yBoard)
            board.CurrentMatch.isDecimating=true;
    }
    public void RemoveDecimate(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport){
        if(defender==piece || piece==null)
            board.CurrentMatch.isDecimating=false;
    }
    public void Capture(Chessman attacker, Chessman defender){
        if(defender.color == piece.color && defender!=piece && defender.abilities.OfType<SoulBond>().FirstOrDefault()!=null && !defender.hexed && !piece.hexed){
            if(piece.type==PieceType.King){
                board.CurrentMatch.EndMatch();
            }
            eventHub.OnPieceCaptured.RemoveListener(Capture); 
            board.CurrentMatch.SetPositionEmpty(piece.xBoard, piece.yBoard);
            board.GetTileAt(piece.xBoard, piece.yBoard).SetBloodTile();
            eventHub.OnPieceCaptured.Invoke(attacker, piece);
            piece.owner.pieces.Remove(piece.gameObject);
            CoroutineRunner.instance.StartCoroutine(PieceFactory._instance.DelayedDestroy(piece));
            
        }
    }

}