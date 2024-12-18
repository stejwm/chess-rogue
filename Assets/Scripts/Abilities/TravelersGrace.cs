using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TravelersGrace", menuName = "Abilities/TravelersGrace")]
public class TravelersGrace : Ability
{
    private MovementProfile startingProfile;
    public TravelersGrace() : base("Traveler's Grace", "Piece can move to any open square, supports as normal, cannot attack") {}


    public override void Apply(Chessman piece)
    {
        startingProfile=piece.moveProfile;
        piece.moveProfile = new TravelersGraceMovement(startingProfile);
        piece.info += " "+abilityName;
        piece.releaseCost+=15;
    }

    public override void Remove(Chessman piece)
    {
        piece.moveProfile=startingProfile;
    }
}
