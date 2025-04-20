using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Countermarch", menuName = "Abilities/Countermarch")]
public class Countermarch : Ability
{
    private MovementProfile startingProfile;
    public Countermarch() : base("Countermarch", "Can move and support backwards") {}


    public override void Apply(Chessman piece)
    {
        
        if (piece.type != PieceType.Pawn)
            return;

        startingProfile=piece.moveProfile;

        if (piece.abilities.OfType<ScoutPawn>().FirstOrDefault()!=null){
            piece.moveProfile = new ScoutCounterMovement();
        }
        else{
            piece.moveProfile = new CountermarchMovement();
        }
        
        piece.info += " "+abilityName;
        base.Apply(piece);
        
    }

    public override void Remove(Chessman piece)
    {
        piece.moveProfile=startingProfile;
    }
}
