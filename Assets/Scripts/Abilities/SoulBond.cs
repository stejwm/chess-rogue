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
        if(piece.abilities.Contains(this)){
            return;
        }
        this.piece = piece;
        piece.info += " " + abilityName;
        GameManager._instance.OnPieceCaptured.AddListener(Capture);
        GameManager._instance.OnSoulBonded.AddListener(Increase);
        GameManager._instance.OnMove.AddListener(Decimate);
        GameManager._instance.OnAttackEnd.AddListener(RemoveDecimate);
        piece.owner.soulBondedPieces++;
        GameManager._instance.OnSoulBonded.Invoke();
        
        
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        GameManager._instance.OnPieceCaptured.RemoveListener(Capture); 
        GameManager._instance.OnSoulBonded.RemoveListener(Increase); 
        GameManager._instance.OnMove.RemoveListener(Decimate);
        GameManager._instance.OnAttackEnd.RemoveListener(RemoveDecimate);
        piece.owner.soulBondedPieces--;
        GameManager._instance.OnSoulBonded.Invoke();

    }

    ~SoulBond()
    {
        if (piece != null)
        {
            GameManager._instance.OnPieceCaptured.RemoveListener(Capture); 
            GameManager._instance.OnSoulBonded.RemoveListener(Increase); 
            GameManager._instance.OnMove.RemoveListener(Decimate);
            GameManager._instance.OnAttackEnd.RemoveListener(RemoveDecimate);
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
            GameManager._instance.isDecimating=true;
    }
    public void RemoveDecimate(Chessman attacker, Chessman defender, int attackSupport, int defenseSupport){
        if(defender==piece || piece==null)
            GameManager._instance.isDecimating=false;
    }
    public void Capture(Chessman attacker, Chessman defender){
        if(defender.color == piece.color && defender!=piece && defender.abilities.OfType<SoulBond>().FirstOrDefault()!=null && !defender.hexed && !piece.hexed){
            if(piece.type==PieceType.King){
                MoveManager._instance.gameOver=true;
            }
            GameManager._instance.OnPieceCaptured.RemoveListener(Capture); 
            GameManager._instance.currentMatch.SetPositionEmpty(piece.xBoard, piece.yBoard);
            Board._instance.GetTileAt(piece.xBoard, piece.yBoard).SetBloodTile();
            GameManager._instance.OnPieceCaptured.Invoke(attacker, piece);
            piece.owner.pieces.Remove(piece.gameObject);
            CoroutineRunner.instance.StartCoroutine(PieceFactory._instance.DelayedDestroy(piece));
            
        }
    }

}