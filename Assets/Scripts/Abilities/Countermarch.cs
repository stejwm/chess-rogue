using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Countermarch", menuName = "Abilities/Countermarch")]
public class Countermarch : Ability
{
    private MovementProfile startingProfile;
    public Countermarch() : base("Countermarch", "Can move and support backwards") {}


    public override void Apply(Board board, Chessman piece)
    {
        if(piece.abilities.Contains(this))
            return;
        
        if (piece.type != PieceType.Pawn)
            return;

        startingProfile=piece.moveProfile;

        if (piece.abilities.OfType<ScoutPawn>().FirstOrDefault()!=null){
            piece.moveProfile = new ScoutCounterMovement(board);
        }
        else{
            piece.moveProfile = new CountermarchMovement(board);
        }
        
        piece.info += " "+abilityName;
        base.Apply(board, piece);
        
    }

    public override void Remove(Chessman piece)
    {
        piece.moveProfile=startingProfile;
    }
}
