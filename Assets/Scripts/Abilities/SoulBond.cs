using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "SoulBond", menuName = "Abilities/SoulBond")]
public class SoulBond : Ability
{
    private Chessman piece;
    
    public SoulBond() : base("Soul Bond", "Permanently gain +5 to each stat for each soul bonded piece, when any soul bonded piece is captured, all are decimated") {}

    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnPieceCaptured.AddListener(Capture);
        Game._instance.OnSoulBonded.AddListener(Increase);
        piece.owner.soulBondedPieces++;
        Game._instance.OnSoulBonded.Invoke();
        piece.releaseCost+=20;
        
        
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnPieceCaptured.RemoveListener(Capture); 

    }

    public void Increase(){
        piece.attack+= 5* (piece.owner.soulBondedPieces-1);
        piece.defense+= 5* (piece.owner.soulBondedPieces-1);
        piece.support+= 5* (piece.owner.soulBondedPieces-1);
    }
    public void Capture(Chessman attacker, Chessman defender){
        if(defender.color == piece.color && defender!=piece && defender.abilities.OfType<SoulBond>().FirstOrDefault()!=null && !defender.hexed && !piece.hexed){
            Game._instance.OnPieceCaptured.RemoveListener(Capture); 
            Game._instance.currentMatch.SetPositionEmpty(piece.xBoard, piece.yBoard);
            //piece.gameObject.SetActive(false);
            BoardManager._instance.GetTileAt(piece.xBoard, piece.yBoard).SetBloodTile();
            Game._instance.OnPieceCaptured.Invoke(attacker, piece);
            CoroutineRunner.instance.StartCoroutine(PieceFactory._instance.DelayedDestroy(piece));
            
        }
    }

}