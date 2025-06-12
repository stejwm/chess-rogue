using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Betrayer", menuName = "Abilities/Betrayer")]
public class Betrayer : Ability
{
    private MovementProfile startingProfile;
    public Betrayer() : base("Betrayer", "Can capture it's own pieces, supports as if it were an enemy") {}


    public override void Apply(Board board, Chessman piece)
    {
        startingProfile=piece.moveProfile;
        piece.moveProfile = new BetrayerMovement(board, startingProfile);
        piece.info += " "+abilityName;
        base.Apply(board, piece);
        
    }

    public override void Remove(Chessman piece)
    {
        piece.moveProfile=startingProfile;
    }
}
