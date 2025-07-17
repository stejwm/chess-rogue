using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UIElements;

public class Card : MonoBehaviour, IInteractable
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
    public bool cardFlipping =false;

    
    public bool isDissolved=false;
    public GameObject price;
    public Material dissolveMaterial;
    public ParticleSystem flames;

    public Card(Ability ability)
    {
        this.ability = ability;
    }

    public void Use(Board board, Chessman target)
    {
        target.AddAbility(board, ability.Clone());
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
        
    }

    /*    void OnMouseDown(){
           if (GameManager._instance.isInMenu || GameManager._instance.applyingAbility)
           {
               return;
           }
           if(ability != null){
               if(GameManager._instance.hero.playerCoins>=ability.Cost || !price.activeSelf){
                   GameManager._instance.GetComponent<GameManager>().CardSelected(this);
                   this.GetComponent<MMSpringPosition>().BumpRandom();
               }
               else{
                   this.GetComponent<MMSpringPosition>().BumpRandom();
               }
           }
           else
               if(order != null){
                   if(GameManager._instance.hero.playerCoins>=order.Cost){
                       GameManager._instance.hero.orders.Add(order);
                       GameManager._instance.hero.playerCoins-=order.Cost;
                       ShopManager._instance.UpdateCurrency();
                       Destroy(this.gameObject);
                   }else{
                       this.GetComponent<MMSpringPosition>().BumpRandom();
                   }
               }
       }
       void OnMouseEnter(){
           if (GameManager._instance.isInMenu)
           {
               return;
           }
           StartCoroutine(CardHovered());

       } */
    public IEnumerator CardHovered()
    {
        if (!cardFlipped && !cardFlipping)
        {
            cardFlipping = true;
            FlipPlayer.PlayFeedbacks();

            this.GetComponent<SpriteRenderer>().sprite = front;
            if (ability != null)
            {
                effect.text = ability.description;
                title.text = ability.abilityName;
            }
            else if (order != null)
            {
                effect.text = order.Description;
                title.text = order.Name;
            }
            yield return new WaitForSeconds(FlipPlayer.TotalDuration);
            cardFlipped = true;
            StartCoroutine(CardExpand());
        }
        
        
    }
    public IEnumerator CardExpand(){
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * 1.15f;
        float duration = 0.15f;
        float t = 0f;
        while (t < duration) {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t / duration);
            yield return null;
        }
        transform.localScale = targetScale;
    }
    
    public IEnumerator CardShrink(){
        Vector3 targetScale = new(.2f, .2f, .2f);
        Vector3 startScale = transform.localScale;
        float duration = 0.15f;
        float t = 0f;
        while (t < duration) {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t / duration);
            yield return null;
        }
        transform.localScale = targetScale;
    }
    
    public void ShowPrice() {
        if (ability != null)
            cost.text = ":" + ability.Cost.ToString();
        if (order != null)
            cost.text = ":" + order.Cost.ToString();
        price.SetActive(true);
    }
    public void HidePrice(){
        price.SetActive(false);
    }

    public IEnumerator Dissolve()
    {
        dissolveMaterial = new Material(dissolveMaterial);
        GetComponent<Renderer>().material = dissolveMaterial;
        dissolveMaterial.SetFloat("_Weight", 0);
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
        isDissolved = true;
    }

    public void OnClick(Board board)
    {
        switch (board.BoardState)
        {
            case BoardState.RewardScreen:
                HandleRewardScreenClick(board);
                break;
            case BoardState.ShopScreen:
                HandleShopScreenClick(board);
                break;
            case BoardState.ManagementScreen:
            case BoardState.KingsOrder:
                StartCoroutine(HandleManagementScreen(board));
                break;
        }
        
    }
    public void HandleRewardScreenClick(Board board)
    {
        if (ability != null)
        {
            if (board.Hero.playerCoins >= ability.Cost || !price.activeSelf)
            {
                board.RewardManager.SelectedCard(this);
            }
            else
            {
                this.GetComponent<MMSpringPosition>().BumpRandom();
            }
        }
    }
    public IEnumerator HandleManagementScreen(Board board)
    {

        if (order != null)
        {
            Debug.Log("Card management clicked");
            BoardState previousState = board.BoardState;
            board.BoardState = BoardState.KingsOrderActive;
            flames.Play();
            yield return StartCoroutine(order.Use(board));
            flames.Stop();
            yield return StartCoroutine(Dissolve());
            board.Hero.orders.Remove(order);
            board.KingsOrderManager.ResetCards();
            board.ClearSelectedPosition();
            board.BoardState = previousState;
            Destroy(gameObject);
        }
    }

    public void HandleShopScreenClick(Board board)
    {
        if (ability != null)
        {
            if (board.Hero.playerCoins >= ability.Cost || !price.activeSelf)
            {
                board.ShopManager.SelectedCard(this);
            }
            else
            {
                this.GetComponent<MMSpringPosition>().BumpRandom();
            }
        }
        else if (order != null)
        {
            board.ShopManager.SelectedOrder(this); 
        }
    }

    public void OnRightClick(Board board)
    {
        throw new System.NotImplementedException();
    }

    public void OnHover(Board board)
    {
        if (cardFlipped)
            StartCoroutine(CardExpand());
        else
            StartCoroutine(CardHovered());
        switch (board.BoardState)
        {
            case BoardState.KingsOrder:
                board.KingsOrderManager.HoverCard(this);
                break;
        }
    }

    public void OnHoverExit(Board board)
    {
        switch (board.BoardState)
        {
            case BoardState.KingsOrder:
                board.KingsOrderManager.ResetCards();
                break;
        }
        if(this != null)
            StartCoroutine(CardShrink());
    }
}
