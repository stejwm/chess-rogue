using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class StatEffect : ScriptableObject
{
    public abstract int CalculateAttack(Chessman piece);
    public abstract int CalculateDefense(Chessman piece);
    public abstract int CalculateSupport(Chessman piece);
}