using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "HexManiac", menuName = "KingsOrders/HexManiac")]
public class HexManiac : KingsOrder
{
    public HexManiac() : base("Hex Maniac", "Hexes all opponent pieces") {}

    public override IEnumerator Use(Board board)
    {
        /* List<GameObject> enemies = GameManager._instance.opponent.pieces;
        foreach (var obj in enemies)
        {
            Chessman piece = obj.GetComponent<Chessman>();
            piece.hexed=true;
            foreach (var ability in piece.abilities)
            {
                ability.Remove(piece);
            }
        }
        yield break; */
        yield return null;
    }

}
