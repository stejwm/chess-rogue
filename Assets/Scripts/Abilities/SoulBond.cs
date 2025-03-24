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

    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnPieceCaptured.AddListener(Capture);
        Game._instance.OnSoulBonded.AddListener(Increase);
        Game._instance.OnMove.AddListener(Decimate);
        Game._instance.OnAttackEnd.AddListener(RemoveDecimate);
        piece.owner.soulBondedPieces++;
        Game._instance.OnSoulBonded.Invoke();
        piece.releaseCost+=20;
        
        
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnPieceCaptured.RemoveListener(Capture); 
        Game._instance.OnSoulBonded.RemoveListener(Increase); 
        Game._instance.OnMove.RemoveListener(Decimate);
        Game._instance.OnAttackEnd.RemoveListener(RemoveDecimate);
        piece.owner.soulBondedPieces--;
        Game._instance.OnSoulBonded.Invoke();

    }

    ~SoulBond()
    {
        if (piece != null)
        {
            Game._instance.OnPieceCaptured.RemoveListener(Capture); 
            Game._instance.OnSoulBonded.RemoveListener(Increase); 
            Game._instance.OnMove.RemoveListener(Decimate);
            Game._instance.OnAttackEnd.RemoveListener(RemoveDecimate);
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
    public void Decimate(Chessman attacker, BoardPosition position){
        if(position.x== piece.xBoard && position.y== piece.yBoard)
            Game._instance.isDecimating=true;
    }
    public void RemoveDecimate(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport){
        if(defender==piece)
            Game._instance.isDecimating=false;
    }
    public void Capture(Chessman attacker, Chessman defender){
        if(defender.color == piece.color && defender!=piece && defender.abilities.OfType<SoulBond>().FirstOrDefault()!=null && !defender.hexed && !piece.hexed){
            if(piece.type==PieceType.King){
                MoveManager._instance.gameOver=true;
            }
            Game._instance.OnPieceCaptured.RemoveListener(Capture); 
            Game._instance.currentMatch.SetPositionEmpty(piece.xBoard, piece.yBoard);
            BoardManager._instance.GetTileAt(piece.xBoard, piece.yBoard).SetBloodTile();
            Game._instance.OnPieceCaptured.Invoke(attacker, piece);
            piece.owner.pieces.Remove(piece.gameObject);
            CoroutineRunner.instance.StartCoroutine(PieceFactory._instance.DelayedDestroy(piece));
            
        }
    }

}