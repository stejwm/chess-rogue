using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "CrownsFinest", menuName = "KingsOrders/CrownsFinest")]
public class CrownsFinest : KingsOrder
{
    public CrownsFinest() : base("Crown's Finest", "Rare Ability in next shop") {}

    public override IEnumerator Use(Board board)
    {
        board.CrownsFinest = true;
        yield return null;
    }

}
