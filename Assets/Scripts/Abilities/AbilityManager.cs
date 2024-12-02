using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AbilityManager: MonoBehaviour
{
    
    public List<Ability> abilities;

    public AbilityManager(){
        abilities=new List<Ability>();
    }
    public void AddAbility(Ability ability, Chessman piece)
    {
        abilities.Add(ability);
        ability.Apply(piece);
    }

    public void RemoveAbility(Ability ability, Chessman piece)
    {
        ability.Remove(piece);
        abilities.Remove(ability);
    }
}
