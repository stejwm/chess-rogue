using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoutPawn", menuName = "Abilities/ScoutPawn")]
public class ScoutPawn : Ability
{
    private MovementProfile startingProfile;
    public ScoutPawn() : base("Scout (Pawn only)", "Moves like queen, attacks & supports like pawn") {}


    public override void Apply(Chessman piece)
    {
        if (piece.type != PieceType.Pawn)
            return;
        startingProfile=piece.moveProfile;
        if (piece.abilities.OfType<Countermarch>().FirstOrDefault()!=null){
            piece.moveProfile = new ScoutCounterMovement();
        }
        else{
            piece.moveProfile = new ScoutPawnMovement();
        }
        
        piece.info += " "+abilityName;
        base.Apply(piece);
        
    }

    public override void Remove(Chessman piece)
    {
        piece.moveProfile=startingProfile;
    }
}
