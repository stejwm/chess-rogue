using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "LastingLegacy", menuName = "KingsOrders/LastingLegacy")]
public class LastingLegacy : KingsOrder
{
    private List<GameObject> civilians = new List<GameObject>();
    public LastingLegacy() : base("Lasting Legacy", "Abandon a piece to create 3 copies of one of its ability in the next shop") {}

    public override IEnumerator Use(Board board)
    {
        Player hero = board.Hero;
        yield return new WaitUntil(() => board.selectedPosition !=null);
        Tile targetPosition = board.selectedPosition;
        board.selectedPosition= null;
        var Chessobj = board.GetPieceAtPosition(targetPosition.X, targetPosition.Y);
        if(Chessobj==null){
            Debug.Log("No piece at position");
            yield break;
        }
        Chessman cm = Chessobj.GetComponent<Chessman>();
        board.LastingLegacyAbility = cm.abilities[Random.Range(0, cm.abilities.Count)].Clone();
        Chessobj.GetComponent<Chessman>().DestroyPiece();
        board.Hero.AbandonedPieces++;

        yield return null;

    }

}
