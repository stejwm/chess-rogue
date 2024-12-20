using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IronResolve", menuName = "Abilities/IronResolve")]
public class IronResolve : Ability
{
    private Chessman piece;
    
    public IronResolve() : base("Iron Resolve", "Permanently gain +1 to defense every time you bounce an enemy") {}

    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnPieceBounced.AddListener(AddBonus);
        piece.releaseCost+=10;
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnPieceBounced.RemoveListener(AddBonus); 

    }
    public void AddBonus(Chessman attacker, Chessman defender, bool isBounceReduced){
        if (defender==piece){
            piece.defense+=1;
        }
    }

}