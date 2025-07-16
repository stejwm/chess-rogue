using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoutPawn", menuName = "Abilities/ScoutPawn")]
public class ScoutPawn : Ability
{
    private MovementProfile startingProfile;
    public ScoutPawn() : base("Scout (Pawn only)", "Moves like queen, attacks & supports like pawn") {}


    public override void Apply(Board board, Chessman piece)
    {
        if(piece.abilities.Contains(this))
            return;
        if (piece.type != PieceType.Pawn)
            return;
        startingProfile=piece.moveProfile;
        if (piece.abilities.OfType<Countermarch>().FirstOrDefault()!=null){
            piece.moveProfile = new ScoutCounterMovement(board);
        }
        else{
            piece.moveProfile = new ScoutPawnMovement(board);
        }
        
        piece.info += " "+abilityName;
        base.Apply(board, piece);
        
    }

    public override void Remove(Chessman piece)
    {
        piece.moveProfile=startingProfile;
    }
}
