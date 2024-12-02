using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{   
    public MovementProfile profile;
    public StatEffect statEffect;

    public string abilityName;
    public string description;

    public Ability(string name, string description, MovementProfile profile, StatEffect statEffect){
        this.abilityName=name;
        this.description=description;
        this.profile=profile;
        this.statEffect=statEffect;
    }
}
