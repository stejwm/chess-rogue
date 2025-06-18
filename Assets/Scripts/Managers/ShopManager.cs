using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Rand= System.Random;
using MoreMountains.Feedbacks;
public class ShopManager : MonoBehaviour
{

    public List<GameObject> pieces = new List<GameObject>();
    private List<GameObject> cards = new List<GameObject>();
    private List<GameObject> orders = new List<GameObject>();
    private Card selectedCard;
    private Chessman selectedPiece;
    private bool applyingAbility;

    [SerializeField] private Board board;


    //Unity calls this right when the game starts, there are a few built in functions
    //that Unity can call for you

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if (selectedPiece && selectedCard && !applyingAbility)
        {
            applyingAbility = true;
            StartCoroutine(ApplyAbility(selectedPiece));
        }
    }


    public void OpenShop(Board board)
    {
        this.board = board;
        gameObject.SetActive(true);

        CreatePieces();
        if (board.LastingLegacyAbility != null)
        {
            List<Ability> abilities = new List<Ability> { board.LastingLegacyAbility.Clone(), board.LastingLegacyAbility.Clone(), board.LastingLegacyAbility.Clone() };
            cards = CardFactory.Instance.CreateCardsWithAbilities(abilities);
            board.LastingLegacyAbility = null;
        }
        else
        {
            cards = CardFactory.Instance.CreateCards(3, board.Hero.RarityWeights);

        }
        int index = 0;
        foreach (var card in cards)
        {
            Vector3 localPosition = new(index * 2 - (1.96f * 2), 2, -2);
            card.transform.position = localPosition;
            card.GetComponent<Card>().ShowPrice();
            index++;
        }
        orders = CardFactory.Instance.CreateRandomKOCards(2);
        index = 0;
        foreach (var order in orders)
        {
            Vector3 localPosition = new(index * 2 + (1.96f * 2), 2, -2);
            order.transform.position = localPosition;
            order.GetComponent<Card>().ShowPrice();
            index++;
        }

    }

    private IEnumerator ApplyAbility(Chessman target)
    {
        if (selectedCard.price.activeSelf)
        {
            if (selectedCard.ability.Cost > board.Hero.playerCoins)
            {
                selectedCard.GetComponent<MMSpringPosition>().BumpRandom();
                selectedCard = null;
                selectedPiece = null;
                yield break;
            }
            board.Hero.playerCoins -= selectedCard.ability.Cost;
        }
        yield return new WaitForSeconds(Settings._instance.WaitTime);
        StartCoroutine(selectedCard.Dissolve());
        selectedCard.Use(board, target);
        yield return new WaitUntil(() => selectedCard.isDissolved);
        StatBoxManager._instance.SetAndShowStats(selectedPiece);
        Destroy(selectedCard.gameObject);
        ClearSelections();
        applyingAbility = false;
        yield return new WaitForSeconds(Settings._instance.WaitTime);
        yield break;
    }

    public void CreatePieces()
    {
        GameObject obj;
        for (int i = 0; i < 3; i++)
        {
            Vector2 localPosition = new Vector2(i - 4, 2);
            obj = PieceFactory._instance.CreateRandomPiece(board);
            obj.transform.position = localPosition;
            Chessman cm = obj.GetComponent<Chessman>();
            cm.xBoard = 5 + i;
            cm.yBoard = 3;
            board.PlacePiece(cm, board.GetTileAt(5 + i, 3));
            SpriteRenderer rend = obj.GetComponent<SpriteRenderer>();
            cm.flames.GetComponent<Renderer>().sortingOrder = 6;
            rend.sortingOrder = 5;
            pieces.Add(obj);
        }
    }

    public void SelectedCard(Card card)
    {
        if (selectedCard != null && selectedCard == card)
        {
            card.flames.Stop();
            selectedCard = null;
        }
        else if (selectedCard != null && selectedCard != card)
        {
            selectedCard.flames.Stop();
            selectedCard = card;
            selectedCard.flames.Play();

        }
        else
        {
            selectedCard = card;
            selectedCard.flames.Play();
        }
    }
    public void SelectedPiece(Chessman piece)
    {
        if (selectedPiece != null && selectedPiece == piece)
        {
            selectedPiece.flames.Stop();
            selectedPiece = null;
        }
        else if (selectedPiece != null && selectedPiece != piece)
        {
            selectedPiece.flames.Stop();
            selectedPiece = piece;
            selectedPiece.flames.Play();
            StatBoxManager._instance.SetAndShowStats(piece);
        }
        else
        {
            selectedPiece = piece;
            selectedPiece.flames.Play();
            StatBoxManager._instance.SetAndShowStats(piece);
        }
    }

    public void SelectedOrder(Card card)
    {
        board.Hero.orders.Add(card.order);
        Destroy(card.gameObject);
    }
    public void ClearSelections()
    {
        selectedCard = null;
        selectedPiece = null;
    }

    public void CloseShop()
    {
        foreach (var card in cards)
        {
            Destroy(card);
        }
        foreach (var order in orders)
        {
            Destroy(order);
        }
        foreach (var piece in pieces)
        {
            Destroy(piece);
        }
        gameObject.SetActive(false);
    }
    
/*
                                            public void ModifyRarityWeight(Rarity rarity, float multiplier)
                                            {
                                                if (rarityWeights.ContainsKey(rarity))
                                                {
                                                    rarityWeights[rarity] *= multiplier;
                                                    // Normalize weights to ensure they sum to 100
                                                    float total = rarityWeights.Values.Sum();
                                                    foreach (var key in rarityWeights.Keys.ToList())
                                                    {
                                                        rarityWeights[key] = (rarityWeights[key] / total) * 100f;
                                                    }
                                                }
                                            }

                                            private List<Ability> SelectRandomAbilities(int count, Rarity? targetRarity = null)
                                            {
                                                List<Ability> selectedAbilities = new List<Ability>();
                                                var groupedAbilities = GameManager._instance.AllAbilities
                                                    .GroupBy(a => a.rarity)
                                                    .ToDictionary(g => g.Key, g => g.ToList());

                                                for (int i = 0; i < count; i++)
                                                {
                                                    Ability selectedAbility;

                                                    if (targetRarity.HasValue)
                                                    {
                                                        // Select from specific rarity
                                                        if (groupedAbilities.ContainsKey(targetRarity.Value) && groupedAbilities[targetRarity.Value].Any())
                                                        {
                                                            var availableAbilities = groupedAbilities[targetRarity.Value];
                                                            int randomIndex = rng.Next(availableAbilities.Count);
                                                            selectedAbility = availableAbilities[randomIndex].Clone();
                                                        }
                                                        else
                                                        {
                                                            // Fallback if no abilities of target rarity exist
                                                            List<Ability> shuffledCards = GameManager._instance.AllAbilities.OrderBy(_ => rng.Next()).ToList();
                                                            selectedAbility = shuffledCards[0].Clone();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // Select based on rarity weights
                                                        float random = Random.Range(0f, 100f);
                                                        float cumulative = 0f;
                                                        Rarity selectedRarity = Rarity.Common;

                                                        foreach (var rarity in rarityWeights.Keys)
                                                        {
                                                            cumulative += rarityWeights[rarity];
                                                            if (random <= cumulative)
                                                            {
                                                                selectedRarity = rarity;
                                                                break;
                                                            }
                                                        }

                                                        if (groupedAbilities.ContainsKey(selectedRarity) && groupedAbilities[selectedRarity].Any())
                                                        {
                                                            var availableAbilities = groupedAbilities[selectedRarity];
                                                            int randomIndex = rng.Next(availableAbilities.Count);
                                                            selectedAbility = availableAbilities[randomIndex].Clone();
                                                        }
                                                        else
                                                        {
                                                            List<Ability> shuffledCards = GameManager._instance.AllAbilities.OrderBy(_ => rng.Next()).ToList();
                                                            selectedAbility = shuffledCards[0].Clone();
                                                        }
                                                    }

                                                    selectedAbilities.Add(selectedAbility);
                                                }

                                                return selectedAbilities;
                                            }

                                            public void CreateCardsWithAbilities(List<Ability> abilities)
                                            {
                                                for (int i = 0; i < abilities.Count; i++)
                                                {
                                                    Vector2 localPosition;
                                                    if (GameManager._instance.state == ScreenState.ShopScreen)
                                                        localPosition = new Vector2(i + i - 4, 2);
                                                    else
                                                        localPosition = new Vector2(i + i, 2);

                                                    GameObject obj = Instantiate(GameManager._instance.card, localPosition, Quaternion.identity);
                                                    obj.GetComponent<Card>().ability = abilities[i];


                                                    if(obj.GetComponent<Card>().ability.rarity >0){
                                                        obj.GetComponent<SpriteRenderer>().material = holoMaterial;
                                                    }

                                                    cards.Add(obj);


                                                    if (GameManager._instance.state == ScreenState.ShopScreen)
                                                        obj.GetComponent<Card>().ShowPrice();
                                                }
                                            }

                                            public void CreateCards(int count = 3, Rarity? targetRarity = null)
                                            {
                                                List<Ability> abilities = SelectRandomAbilities(count, targetRarity);
                                                CreateCardsWithAbilities(abilities);
                                            }




                                            public void ClearCards(){
                                                foreach (var card in cards)
                                                {
                                                    if(card!=null)
                                                    Destroy(card);
                                                }

                                            }
                                            public void ClearOrders(){
                                                foreach (var order in orders)
                                                {
                                                    if(order!=null)
                                                        Destroy(order);
                                                }

                                            }


                                            public void RerollAbilities(){
                                                if(GameManager._instance.applyingAbility)
                                                    return;
                                                if(board.Hero.playerCoins>=rerollCost){
                                                    board.Hero.playerCoins-=rerollCost;
                                                    rerollCost+=rerollCostIncrease;
                                                    rerollCostText.text = rerollCost.ToString();
                                                    UpdateCurrency();
                                                    ClearCards();
                                                    CreateCards();
                                                }

                                            }

                                            public void UpdateCurrency(){
                                                bloodText.text = ": "+board.Hero.playerBlood;
                                                coinText.text = ": "+board.Hero.playerCoins;
                                                rerollCostText.text = rerollCost.ToString();
                                            }

                                            public void CloseShop(){
                                                ManagementStatManager._instance.HideStats();
                                                ClearCards();
                                                ClearOrders();
                                                foreach (GameObject piece in myPieces)
                                                {
                                                    if (piece != null && piece.GetComponent<SpriteRenderer>())
                                                    {
                                                        SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                                                        rend.sortingOrder = 1;
                                                        piece.GetComponent<Chessman>().flames.GetComponent<Renderer>().sortingOrder=2;
                                                        piece.GetComponent<Chessman>().highlightedParticles.GetComponent<Renderer>().sortingOrder=0;
                                                    }
                                                }
                                                foreach (GameObject piece in board.Hero.inventoryPieces)
                                                {
                                                    if (piece != null &&  piece.GetComponent<SpriteRenderer>())
                                                    {
                                                        SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                                                        rend.sortingOrder = 1;
                                                        piece.GetComponent<Chessman>().flames.GetComponent<Renderer>().sortingOrder=2;
                                                        piece.GetComponent<Chessman>().highlightedParticles.GetComponent<Renderer>().sortingOrder=0;
                                                    }
                                                }
                                                foreach (GameObject piece in pieces)
                                                {
                                                    if(piece != null)
                                                        Destroy(piece);
                                                }
                                                GameManager._instance.CloseShop();
                                                gameObject.SetActive(false);
                                            }

                                            public void HideShop(){
                                                this.gameObject.SetActive(false);
                                                foreach (GameObject card in cards){
                                                    if(card!=null)
                                                        card.SetActive(false);
                                                }
                                                foreach (GameObject order in orders){
                                                    if(order!=null)
                                                        order.SetActive(false);
                                                }
                                                foreach (GameObject piece in pieces){
                                                    if(piece != null)
                                                        piece.SetActive(false);
                                                }
                                            }
                                            public void UnHideShop(){
                                                this.gameObject.SetActive(true);
                                                UpdateCurrency();
                                                foreach (GameObject card in cards){
                                                    if(card!=null){
                                                        card.SetActive(true);
                                                        card.GetComponent<Collider2D>().enabled = true;
                                                    }
                                                }
                                                foreach (GameObject order in orders){
                                                    if(order!=null){
                                                        order.SetActive(true);
                                                        order.GetComponent<Collider2D>().enabled = true;
                                                    }
                                                }
                                                foreach (GameObject piece in pieces){
                                                    if(piece != null)
                                                        piece.SetActive(true);
                                                }
                                            }

                                             */


}
