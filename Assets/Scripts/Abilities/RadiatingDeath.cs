using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RadiatingDeath", menuName = "Abilities/RadiatingDeath")]
public class RadiatingDeath : Ability
{
    private Chessman piece;
    
    public RadiatingDeath() : base("Radiating Death", "Reduce all attackers stats by -1 when captured") {}

    public override void Apply(Chessman piece)
    {
        this.piece = piece;
        piece.info += " " + abilityName;
        Game._instance.OnPieceCaptured.AddListener(RadiateDeath);
        piece.releaseCost+=Cost;
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        Game._instance.OnPieceCaptured.RemoveListener(RadiateDeath); 

    }
    public void RadiateDeath(Chessman attacker, Chessman defender){
        if (defender==piece){
            piece.effectsFeedback.PlayFeedbacks();
            attacker.attackBonus--;
            attacker.supportBonus--;
            attacker.defenseBonus--;
        }
    }

}
