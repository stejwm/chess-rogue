using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    public GameObject controller;
    public Ability ability;
    public KingsOrder order;

    public TMP_Text title;
    public TMP_Text effect;

    public Sprite front;

    public MMF_Player FlipPlayer;
    public bool cardFlipped =false;
    public GameObject price;

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
        if(ability!=null)
            Game._instance.GetComponent<Game>().CardSelected(this);
        else
            Game._instance.hero.orders.Add(order);
    }
    void OnMouseOver(){
        FlipCard();

    }

    private void FlipCard(){
        if (!cardFlipped){
            FlipPlayer.PlayFeedbacks();
            this.GetComponent<SpriteRenderer>().sprite=front;
            if(ability!=null){
                effect.text= ability.description;
                title.text= ability.abilityName;
            }else if(order!=null){
                effect.text= order.Description;
                title.text= order.Name;
            }
            cardFlipped=true;
        }
    }
    public void ShowPrice(){
        price.SetActive(true);
    }
}
