using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

public abstract class Ability : ScriptableObject
{   
    public string abilityName;
    public string description;
    public Sprite sprite;
    public GameObject AbilityLoggerPrefab;
    public GameObject PopUpMenu;
    public TMP_ColorGradient gradient;

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

    public override bool Equals(object obj)
    {
        var item = obj as Ability;

        if (item == null)
        {
            return false;
        }

        return this.abilityName ==item.abilityName && this.description == item.description;
    }
}
