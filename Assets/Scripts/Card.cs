using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public GameObject controller;
    public Ability ability;

    public void Use(GameObject target){
        if (ability.profile != null)
            target.GetComponent<Chessman>().moveProfile=ability.profile;
        if(ability.statEffect!=null)
            target.GetComponent<Chessman>().statEffects.Add(ability.statEffect);
        target.GetComponent<Chessman>().info += " "+ability.abilityName;
    }

    void OnMouseDown(){
        controller = GameObject.FindGameObjectWithTag("GameController");
        controller.GetComponent<Game>().CardSelected(this);
    }
}
