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
        /* Player hero = GameManager._instance.hero;
        GameManager._instance.tileSelect=true;
        yield return new WaitUntil(() => Board._instance.selectedPosition !=null);
        GameManager._instance.tileSelect=false;
        BoardPosition targetPosition = Board._instance.selectedPosition;
        Board._instance.selectedPosition= null;
        var Chessobj = board.CurrentMatch.GetPieceAtPosition(targetPosition.x, targetPosition.y);
        if(Chessobj==null){
            Debug.Log("No piece at position");
            yield break;
        }
        Chessman cm = Chessobj.GetComponent<Chessman>();
        GameManager._instance.lastingLegacyAbility = cm.abilities[Random.Range(0, cm.abilities.Count)].Clone();
        GameManager._instance.hero.pieces.Remove(Chessobj);
        GameManager._instance.hero.openPositions.Add(cm.startingPosition);
        Chessobj.GetComponent<Chessman>().DestroyPiece();
        GameManager._instance.abandonedPieces++;
        
        
 */
        yield return null;

    }

    public void RemoveCivilians(PieceColor color)
    {
        /*  foreach (var piece in civilians)
         {
             board.CurrentMatch.black.capturedPieces.Remove(piece);
             GameManager._instance.hero.pieces.Remove(piece);
             Destroy(piece);
         } */
    }

}
