using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "FreeTrade", menuName = "KingsOrders/FreeTrade")]
public class FreeTrade : KingsOrder
{

    public FreeTrade() : base("Free Trade", "Sets reroll cost to 0, increases additional rerolls cost by +1") {}

    public override IEnumerator Use(Board board){
        board.RerollCost=0;
        board.RerollCostIncrease++;
        yield return null;
    }


}
