using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public GameObject controller;
    public Ability ability;

    public Card(Ability ability)
    {
        this.ability = ability;
    }

    public void Use(Chessman target)
    {
        target.AddAbility(ability);
    }

    void OnMouseDown(){
        controller = GameObject.FindGameObjectWithTag("GameController");
        controller.GetComponent<Game>().CardSelected(this);
    }
}
