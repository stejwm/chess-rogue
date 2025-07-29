using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rand= System.Random;

[CreateAssetMenu(fileName = "ArcaneResonance", menuName = "Abilities/ArcaneResonance")]
public class ArcaneResonance : Ability
{
    private Chessman piece;

    public ArcaneResonance() : base("Arcane Resonance", "Adds +1 stack to each stackable ability") { }


    public override void Apply(Board board, Chessman piece)
    {
        if (piece.abilities.Contains(this))
            return;
        this.piece = piece;
        piece.info += " " + abilityName;
        var abilitiesCopy = new List<Ability>(piece.abilities);
        foreach (Ability ability in abilitiesCopy)
        {
            //ability.Apply(board, piece);
            piece.AddAbility(board, ability.Clone());
        }
        eventHub.OnAbilityAdded.AddListener(AddStack);

        base.Apply(board, piece);
    }

    public override void Remove(Chessman piece)
    {
        eventHub.OnAbilityAdded.RemoveListener(AddStack);
        var abilitiesCopy = new List<Ability>(piece.abilities);
        foreach (Ability ability in abilitiesCopy)
        {
            if (ability == this)
                continue;
            ability.Remove(piece);
        }

    }

    public void AddStack(Chessman cm, Ability ability)
    {
        eventHub.OnAbilityAdded.RemoveListener(AddStack);
        piece.AddAbility(board, ability);
        eventHub.OnAbilityAdded.AddListener(AddStack);
    }

}
