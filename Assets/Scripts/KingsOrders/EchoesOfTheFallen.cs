using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "EchoesOfTheFallen", menuName = "KingsOrders/EchoesOfTheFallen")]
public class EchoesOfTheFallen : KingsOrder
{
    public EchoesOfTheFallen() : base("Echoes of the Fallen", "All pieces gain +1 to random stat for each piece abandoned this run") {}

    public override IEnumerator Use(Board board)
    {
        List<GameObject> pieces = board.Hero.pieces;
        for (int i=0; i< board.Hero.AbandonedPieces; i++){
        foreach (var obj in pieces)
            {
                Chessman piece = obj.GetComponent<Chessman>();
                
                switch (Random.Range(0,3)){
                    case 0:
                        piece.attack++;
                        break;
                    case 1:
                        piece.defense++;
                        break;
                    case 2:
                        piece.support++;
                        break;
                }
                
            }
        }
        yield break;
    }

}
