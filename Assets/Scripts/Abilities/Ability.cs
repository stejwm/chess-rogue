using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{   
    public string abilityName;
    public string description;
    public Sprite sprite;

    public abstract void Apply(Chessman piece);
    public abstract void Remove(Chessman piece);
    protected Game game;

    protected Ability(string name, string description)
    {
        abilityName = name;
        this.description = description;
    }
    public Ability Clone()
    {
    return Instantiate(this);
    }
}
