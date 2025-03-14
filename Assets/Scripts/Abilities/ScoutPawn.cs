using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoutPawn", menuName = "Abilities/ScoutPawn")]
public class ScoutPawn : Ability
{
    private MovementProfile startingProfile;
    public ScoutPawn() : base("Scout", "Moves like queen. Captures & supports like pawn") {}


    public override void Apply(Chessman piece)
    {
        if (piece.type != PieceType.Pawn)
            return;

        startingProfile=piece.moveProfile;
        piece.moveProfile = new ScoutPawnMovement();
        piece.info += " "+abilityName;
        piece.releaseCost+=10;
        base.Apply(piece);
        
    }

    public override void Remove(Chessman piece)
    {
        piece.moveProfile=startingProfile;
    }
}
