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
        Player hero = GameManager._instance.hero;
        GameManager._instance.tileSelect=true;
        yield return new WaitUntil(() => Board._instance.selectedPosition !=null);
        GameManager._instance.tileSelect=false;
        BoardPosition targetPosition = Board._instance.selectedPosition;
        Board._instance.selectedPosition= null;
        for (int i=0; i<8; i++){
            if(GameManager._instance.currentMatch.GetPositions()[i, targetPosition.y] == null){
                var piece = PieceFactory._instance.Create(PieceType.Pawn, i, targetPosition.y, hero.color, Team.Hero, hero);
                GameManager._instance.hero.pieces.Add(piece);
                civilians.Add(piece);
                GameManager._instance.currentMatch.MovePiece(piece.GetComponent<Chessman>(), i, targetPosition.y);
                
            }
        }
        GameManager._instance.OnGameEnd.AddListener(RemoveCivilians);
        GameManager._instance.togglePieceColliders(civilians, false);
        GameManager._instance.currentMatch.SetPiecesValidForAttack(hero);
    }

    public void RemoveCivilians(PieceColor color){
        foreach (var piece in civilians)
        {
            GameManager._instance.currentMatch.black.capturedPieces.Remove(piece);
            GameManager._instance.hero.pieces.Remove(piece);
            Destroy(piece);
        }
    }

}
