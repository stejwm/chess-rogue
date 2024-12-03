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
        target.AddAbility(ability.Clone());
    }

    void OnMouseDown(){
        controller = GameObject.FindGameObjectWithTag("GameController");
        controller.GetComponent<Game>().CardSelected(this);
    }
    void OnMouseOver(){
        this.GetComponent<SpriteRenderer>().sprite=ability.sprite;

    }
}
