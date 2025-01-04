using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    public GameObject controller;
    public Ability ability;

    public TMP_Text title;
    public TMP_Text effect;

    public Sprite front;

    public MMF_Player FlipPlayer;
    public bool cardFlipped =false;

    public Card(Ability ability)
    {
        this.ability = ability;
    }

    public void Use(Chessman target)
    {
        target.AddAbility(ability.Clone());
    }

    public void Awake(){
        Canvas canvas =GetComponentInChildren<Canvas>();
         //GetComponent<Canvas>();
        canvas.sortingLayerName="Board Layer";
        canvas.sortingOrder=6;
    }

    void OnMouseDown(){
        controller = GameObject.FindGameObjectWithTag("GameController");
        controller.GetComponent<Game>().CardSelected(this);
    }
    void OnMouseOver(){
        FlipCard();

    }

    private void FlipCard(){
        if (!cardFlipped){
            FlipPlayer.PlayFeedbacks();
            this.GetComponent<SpriteRenderer>().sprite=front;
            effect.text= ability.description;
            title.text= ability.abilityName;
            cardFlipped=true;
        }
    }
}
