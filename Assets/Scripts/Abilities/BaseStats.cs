using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStats : StatEffect
{
    public override int CalculateAttack(Chessman piece) {
        return piece.attack;
     }
    public override int CalculateSupport(Chessman piece) {
        return piece.support;
     }

     public override int CalculateDefense(Chessman piece) {
        return piece.defense;
     }
}
