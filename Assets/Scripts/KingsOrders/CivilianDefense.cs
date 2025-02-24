using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "CivilianDefense", menuName = "KingsOrders/CivilianDefense")]
public class CivilianDefense : KingsOrder
{
    private List<GameObject> civilians = new List<GameObject>();
    public CivilianDefense() : base("Civilian Defense", "Creates a pawn in any open spaces at selected row") {}

    public override IEnumerator Use(){
        Debug.Log("Civilian Defense");
        Player hero = Game._instance.hero;
        Game._instance.tileSelect=true;
        yield return new WaitUntil(() => BoardManager._instance.selectedPosition !=null);
        Game._instance.tileSelect=false;
        BoardPosition targetPosition = BoardManager._instance.selectedPosition;
        BoardManager._instance.selectedPosition= null;
        for (int i=0; i<8; i++){
            if(Game._instance.currentMatch.GetPositions()[i, targetPosition.y] == null){
                var piece = PieceFactory._instance.Create(PieceType.Pawn, "Civilian", i, targetPosition.y, hero.color, Team.Hero, hero);
                Game._instance.hero.pieces.Add(piece);
                civilians.Add(piece);
                Game._instance.currentMatch.MovePiece(piece.GetComponent<Chessman>(), i, targetPosition.y);
                
            }
        }
        Game._instance.togglePieceColliders(civilians, false);
        Game._instance.currentMatch.SetPiecesValidForAttack(hero);
        Debug.Log("Pieces created");
    }

}
