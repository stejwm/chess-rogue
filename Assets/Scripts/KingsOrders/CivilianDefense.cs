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
        this.board = board;
        Player hero = board.Hero;
        yield return new WaitUntil(() => board.selectedPosition !=null);
        Tile targetPosition = board.selectedPosition;
        board.selectedPosition= null;
        for (int i=0; i<8; i++){
            if(board.Positions[i, targetPosition.Y] == null){
                var piece = PieceFactory._instance.Create(board, PieceType.Pawn, i, targetPosition.Y, hero.color, hero);
                board.Hero.pieces.Add(piece);
                civilians.Add(piece);
                board.CurrentMatch.MovePiece(piece.GetComponent<Chessman>(), i, targetPosition.Y);

            }
        }
        board.EventHub.OnGameEnd.AddListener(RemoveCivilians);
        board.CurrentMatch.SetPiecesValidForAttack(hero); 
        yield return null;
    }

    public void RemoveCivilians(PieceColor color){
         foreach (var piece in civilians)
        {
            board.CurrentMatch.black.capturedPieces.Remove(piece);
            board.Hero.pieces.Remove(piece);
            Destroy(piece);
        }
        
    }

}
