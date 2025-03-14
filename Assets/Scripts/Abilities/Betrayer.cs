using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Betrayer", menuName = "Abilities/Betrayer")]
public class Betrayer : Ability
{
    private MovementProfile startingProfile;
    public Betrayer() : base("Betrayer", "Can capture it's own pieces, supports as if it were an enemy") {}


    public override void Apply(Chessman piece)
    {
        startingProfile=piece.moveProfile;
        piece.moveProfile = new BetrayerMovement(startingProfile);
        piece.info += " "+abilityName;
        piece.releaseCost+=10;
        base.Apply(piece);
        
    }

    public override void Remove(Chessman piece)
    {
        piece.moveProfile=startingProfile;
    }
}
