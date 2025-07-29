using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "KillingField", menuName = "KingsOrders/KillingField")]
public class KillingField : KingsOrder
{

    public KillingField() : base("Killing Field", "Doubles blood earned in next market") {}

    public override IEnumerator Use(Board board){
        board.MarketManager.killingField = true;
        yield return null;
    }


}
