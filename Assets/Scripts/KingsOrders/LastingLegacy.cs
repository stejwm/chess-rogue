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

    public override IEnumerator Use(){
        Player hero = Game._instance.hero;
        Game._instance.tileSelect=true;
        yield return new WaitUntil(() => BoardManager._instance.selectedPosition !=null);
        Game._instance.tileSelect=false;
        BoardPosition targetPosition = BoardManager._instance.selectedPosition;
        BoardManager._instance.selectedPosition= null;
        var Chessobj = Game._instance.currentMatch.GetPieceAtPosition(targetPosition.x, targetPosition.y);
        if(Chessobj==null){
            Debug.Log("No piece at position");
            yield break;
        }
        Chessman cm = Chessobj.GetComponent<Chessman>();
        Game._instance.lastingLegacyAbility = cm.abilities[Random.Range(0, cm.abilities.Count)].Clone();
        Game._instance.hero.pieces.Remove(Chessobj);
        Game._instance.hero.openPositions.Add(cm.startingPosition);
        Chessobj.GetComponent<Chessman>().DestroyPiece();
        Game._instance.abandonedPieces++;
        
        


    }

    public void RemoveCivilians(PieceColor color){
        foreach (var piece in civilians)
        {
            Game._instance.currentMatch.black.capturedPieces.Remove(piece);
            Game._instance.hero.pieces.Remove(piece);
            Destroy(piece);
        }
    }

}
