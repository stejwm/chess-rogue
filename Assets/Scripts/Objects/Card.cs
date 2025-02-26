using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Card : MonoBehaviour
{
    public GameObject controller;
    public Ability ability;
    public KingsOrder order;

    public TMP_Text title;
    public TMP_Text effect;
    public TMP_Text cost;

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
        HidePrice();
    }

    void OnMouseDown(){
        if(ability != null){
            if(Game._instance.hero.playerCoins>=ability.Cost || !price.activeSelf){
                Game._instance.GetComponent<Game>().CardSelected(this);
            }
            else{
                this.GetComponent<MMSpringPosition>().BumpRandom();
            }
        }
        else
            if(order != null){
                if(Game._instance.hero.playerCoins>=order.Cost){
                    Game._instance.hero.orders.Add(order);
                    Game._instance.hero.playerCoins-=order.Cost;
                    ShopManager._instance.UpdateCurrency();
                    Destroy(this.gameObject);
                }else{
                    this.GetComponent<MMSpringPosition>().BumpRandom();
                }
            }
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
        if(ability!=null)
            cost.text=":"+ability.Cost.ToString();
        if(order!=null)
            cost.text=":"+order.Cost.ToString();
        price.SetActive(true);
    }
    public void HidePrice(){
        price.SetActive(false);
    }
}
