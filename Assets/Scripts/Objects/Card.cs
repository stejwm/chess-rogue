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
    
    public bool isDissolved=false;
    public GameObject price;
    private Material dissolveMaterial;
    public ParticleSystem flames;

    public Card(Ability ability)
    {
        this.ability = ability;
    }

    public void Use(Chessman target)
    {
        target.AddAbility(ability.Clone());
        target.flames.Stop();
    }

    public void Awake(){
        Canvas canvas =GetComponentInChildren<Canvas>();
         //GetComponent<Canvas>();
        canvas.sortingLayerName="Board Layer";
        canvas.sortingOrder=4;
        HidePrice();
    }

    public void Start(){
        dissolveMaterial = new Material(GetComponent<Renderer>().material);
        GetComponent<Renderer>().material = dissolveMaterial;
    }

    void OnMouseDown(){
        if (Game._instance.isInMenu)
        {
            return;
        }
        if(ability != null){
            if(Game._instance.hero.playerCoins>=ability.Cost || !price.activeSelf){
                Game._instance.GetComponent<Game>().CardSelected(this);
                this.GetComponent<MMSpringPosition>().BumpRandom();
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
        if (Game._instance.isInMenu)
        {
            return;
        }
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

    public IEnumerator Dissolve(){
        
        float dissolveAmount = 0f;
        float fadeAmount = 0.0f;
        float duration = .5f;
        flames.Stop();
        Color originalColor = title.color;
            while (dissolveAmount < duration)  // Stop when fully dissolved
            {
                dissolveAmount += Time.deltaTime * 0.3f;
                dissolveMaterial.SetFloat("_Weight", dissolveAmount);
                fadeAmount += Time.deltaTime;
                title.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - (fadeAmount / duration));
                effect.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - (fadeAmount / duration));

                yield return null; // Wait for next frame
            }
            isDissolved=true;
    }
}
