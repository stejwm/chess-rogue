using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Countermarch", menuName = "Abilities/Countermarch")]
public class Countermarch : Ability
{
    private MovementProfile startingProfile;
    public Countermarch() : base("Countermarch", "Can move and attack backwards") {}


    public override void Apply(Chessman piece)
    {
        
        if (piece.type != PieceType.Pawn)
            return;

        startingProfile=piece.moveProfile;
        piece.moveProfile = new CountermarchMovement();
        piece.info += " "+abilityName;
        piece.releaseCost+=10;
        base.Apply(piece);
        
    }

    public override void Remove(Chessman piece)
    {
        piece.moveProfile=startingProfile;
    }
}
