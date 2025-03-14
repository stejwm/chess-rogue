using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpectralStride", menuName = "Abilities/SpectralStride")]
public class SpectralStride : Ability
{
    private MovementProfile startingProfile;
    public SpectralStride() : base("Spectral Stride", "Piece can move through it's own pieces according to it's typical movement") {}


    public override void Apply(Chessman piece)
    {
        startingProfile=piece.moveProfile;
        piece.moveProfile = new SpectralStrideMovement(startingProfile);
        piece.info += " "+abilityName;
        piece.releaseCost+=15;
        base.Apply(piece);
    }

    public override void Remove(Chessman piece)
    {
        piece.moveProfile=startingProfile;
    }
}
