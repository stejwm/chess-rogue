using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IronResolve", menuName = "Abilities/IronResolve")]
public class IronResolve : Ability
{
    private Chessman piece;
    
    public IronResolve() : base("Iron Resolve", "Permanently gain +1 to defense every time an enemy is bounced") {}

    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnPieceBounced.AddListener(AddBonus);
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnPieceBounced.RemoveListener(AddBonus); 

    }
    public void AddBonus(Chessman attacker, Chessman defender, bool isBounceReduced){
        if (defender==piece){
                        AbilityLogger._instance.LogAbilityUsage($"<sprite=\"{piece.color}{piece.type}\" name=\"{piece.color}{piece.type}\"><color=white><gradient=\"AbilityGradient\">Iron Resolve</gradient></color>", "Try harder next time!");

            piece.defense+=1;
        }
    }

}