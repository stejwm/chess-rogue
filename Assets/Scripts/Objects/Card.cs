using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
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
    public IEnumerator CardHovered(){
        if (!cardFlipped && !cardFlipping){
            cardFlipping=true;
            FlipPlayer.PlayFeedbacks();
            
            this.GetComponent<SpriteRenderer>().sprite=front;
            if(ability!=null){
                effect.text= ability.description;
                title.text= ability.abilityName;
            }else if(order!=null){
                effect.text= order.Description;
                title.text= order.Name;
            }
            yield return new WaitForSeconds(FlipPlayer.TotalDuration);
            cardFlipped=true;
        }
        if(cardFlipped){
            //gameObject.GetComponent<MMSpringPosition>().MoveToAdditive(new Vector3(0,1,0));
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
            isDissolved=true;
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
            if (board.Hero.playerCoins >= order.Cost)
            {
                board.ShopManager.SelectedOrder(this);
            }
            else
            {
                this.GetComponent<MMSpringPosition>().BumpRandom();
            }
        }
    }

    public void OnRightClick(Board board)
    {
        throw new System.NotImplementedException();
    }

    public void OnHover(Board board)
    {
        StartCoroutine(CardHovered());
    }

    public void OnHoverExit(Board board)
    {
        Debug.Log("Card hover exited");
    }
}
