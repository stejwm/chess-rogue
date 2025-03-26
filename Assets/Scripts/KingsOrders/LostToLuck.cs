using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "LostToLuck", menuName = "KingsOrders/LostToLuck")]
public class LostToLuck : KingsOrder
{
    private List<GameObject> civilians = new List<GameObject>();
    public LostToLuck() : base("Lost To Luck", "Abandon a piece in order to increase odds uncommon and rare abilities") {}

    public override IEnumerator Use(){
        Player hero = Game._instance.hero;
        Game._instance.tileSelect=true;
        yield return new WaitUntil(() => BoardManager._instance.selectedPosition !=null);
        Game._instance.tileSelect=false;
        BoardPosition targetPosition = BoardManager._instance.selectedPosition;
        BoardManager._instance.selectedPosition= null;
        var Chessobj = Game._instance.currentMatch.GetPieceAtPosition(targetPosition.x, targetPosition.y);
        if(Chessobj==null){
            Debug.Log("No piece at possition");
            yield break;
        }
        Chessman cm = Chessobj.GetComponent<Chessman>();
        Game._instance.hero.pieces.Remove(Chessobj);
        Game._instance.hero.openPositions.Add(cm.startingPosition);
        Chessobj.GetComponent<Chessman>().DestroyPiece();
        Game._instance.abandonedPieces++;
        ShopManager._instance.ModifyRarityWeight(Rarity.Uncommon, 1.5f);
        ShopManager._instance.ModifyRarityWeight(Rarity.Rare, 1.5f);


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
