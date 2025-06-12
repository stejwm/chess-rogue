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

    public override IEnumerator Use(Board board)
    {
        /*  Player hero = GameManager._instance.hero;
         GameManager._instance.tileSelect=true;
         yield return new WaitUntil(() => Board._instance.selectedPosition !=null);
         GameManager._instance.tileSelect=false;
         BoardPosition targetPosition = Board._instance.selectedPosition;
         Board._instance.selectedPosition= null;
         for (int i=0; i<8; i++){
             if(board.CurrentMatch.GetPositions()[i, targetPosition.y] == null){
                 var piece = PieceFactory._instance.Create(board, PieceType.Pawn, i, targetPosition.y, hero.color, Team.Hero, hero);
                 GameManager._instance.hero.pieces.Add(piece);
                 civilians.Add(piece);
                 board.CurrentMatch.MovePiece(piece.GetComponent<Chessman>(), i, targetPosition.y);

             }
         }
         GameManager._instance.OnGameEnd.AddListener(RemoveCivilians);
         GameManager._instance.togglePieceColliders(civilians, false);
         board.CurrentMatch.SetPiecesValidForAttack(hero); */
        yield return null;
    }

    public void RemoveCivilians(PieceColor color){
        /* foreach (var piece in civilians)
        {
            board.CurrentMatch.black.capturedPieces.Remove(piece);
            GameManager._instance.hero.pieces.Remove(piece);
            Destroy(piece);
        } */
    }

}
