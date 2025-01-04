using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{   
    public string abilityName;
    public string description;
    public Sprite sprite;
    public GameObject AbilityLoggerPrefab;
    public GameObject PopUpMenu;

    public abstract void Apply(Chessman piece);
    public abstract void Remove(Chessman piece);

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
