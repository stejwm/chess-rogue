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
        Player hero = GameManager._instance.hero;
        GameManager._instance.tileSelect=true;
        yield return new WaitUntil(() => Board._instance.selectedPosition !=null);
        GameManager._instance.tileSelect=false;
        BoardPosition targetPosition = Board._instance.selectedPosition;
        Board._instance.selectedPosition= null;
        var Chessobj = GameManager._instance.currentMatch.GetPieceAtPosition(targetPosition.x, targetPosition.y);
        if(Chessobj==null){
            Debug.Log("No piece at possition");
            yield break;
        }
        Chessman cm = Chessobj.GetComponent<Chessman>();
        GameManager._instance.hero.pieces.Remove(Chessobj);
        GameManager._instance.hero.openPositions.Add(cm.startingPosition);
        Chessobj.GetComponent<Chessman>().DestroyPiece();
        GameManager._instance.abandonedPieces++;
        ShopManager._instance.ModifyRarityWeight(Rarity.Uncommon, 1.5f);
        ShopManager._instance.ModifyRarityWeight(Rarity.Rare, 1.5f);


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
