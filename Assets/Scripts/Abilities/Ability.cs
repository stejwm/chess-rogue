using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Hidden,
}
public abstract class Ability : ScriptableObject
{   
    public string abilityName;
    public string description;
    protected Board board;
    protected EventHub eventHub;
    public Sprite sprite;
    public int Cost = 10;
    public Rarity rarity;
    

    public virtual void Apply(Board board, Chessman piece)
    {
        //piece.releaseCost+= (int)(rarity + 1);
        this.board = board;
        this.eventHub = board.EventHub;
        piece.abilities.Add(this);
        eventHub.OnAbilityAdded.Invoke(piece, this);
    }
    public abstract void Remove(Chessman piece);

    protected Ability(string name, string description)
    {
        abilityName = name;
        this.description = description;
        Cost=10;
    }
    public Ability Clone()
    {
        return Instantiate(this);
    }

    public override bool Equals(object obj)
    {
        var item = obj as Ability;

        if (item == null)
        {
            return false;
        }

        return this.abilityName ==item.abilityName && this.description == item.description;
    }
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (abilityName != null ? abilityName.GetHashCode() : 0);
            hash = hash * 23 + (description != null ? description.GetHashCode() : 0);
            return hash;
        }
    }
}
