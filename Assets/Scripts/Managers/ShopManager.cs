using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Rand= System.Random;
public class ShopManager : MonoBehaviour
{
    public List<GameObject> myPieces;
    public TMP_Text bloodText;
    public TMP_Text coinText;
    public TMP_Text rerollCostText;

    public List<GameObject> pieces = new List<GameObject>();

    //current turn
    public static ShopManager _instance;
    private static Rand rng = new Rand();
    private List<GameObject> cards = new List<GameObject>();
    private List<GameObject> orders = new List<GameObject>();
    public int rerollCost = 5;
    public int rerollCostIncrease = 5;
    private Dictionary<Rarity, float> rarityWeights = new Dictionary<Rarity, float>()
    {
        { Rarity.Common, 50f },
        { Rarity.Uncommon, 30f },
        { Rarity.Rare, 20f }
    };


    void Awake()
    {
        
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }

    //Unity calls this right when the game starts, there are a few built in functions
    //that Unity can call for you
    public void Start()
    {
        gameObject.SetActive(false);
        
    }

    public void OpenShop(){
        Game._instance.isInMenu=false;
        gameObject.SetActive(true);
        UpdateCurrency();
        myPieces=Game._instance.hero.pieces;

        foreach (GameObject piece in myPieces)
        {
            if (piece.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
                rend.sortingOrder = 5;
                piece.GetComponent<Chessman>().flames.GetComponent<Renderer>().sortingOrder=6;
            }
            
        }
        if(!Game._instance.shopUsed){
            CreatePieces();
            CreateCards();
            CreateOrders();
        }
        
    }

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

    public void CreateCards(float rareMultiplier = 1f)
    {
        GameObject obj;
        // Group abilities by rarity
        var groupedAbilities = Game._instance.AllAbilities
            .GroupBy(a => a.rarity)
            .ToDictionary(g => g.Key, g => g.ToList());

        for (int i = 0; i < 3; i++)
        {
            Vector2 localPosition;
            if(Game._instance.state==ScreenState.ShopScreen)
                localPosition = new Vector2(i + i - 4, 2);
            else
                localPosition = new Vector2(i+i, 2);
                
            obj = Instantiate(Game._instance.card, localPosition, Quaternion.identity);

            // Select rarity based on weights
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

            // Select random ability of chosen rarity
            if (groupedAbilities.ContainsKey(selectedRarity) && groupedAbilities[selectedRarity].Any())
            {
                var availableAbilities = groupedAbilities[selectedRarity];
                int randomIndex = rng.Next(availableAbilities.Count);
                obj.GetComponent<Card>().ability = availableAbilities[randomIndex].Clone();
            }
            else
            {
                // Fallback to any random ability if no abilities of selected rarity exist
                List<Ability> shuffledcards = Game._instance.AllAbilities.OrderBy(_ => rng.Next()).ToList();
                obj.GetComponent<Card>().ability = shuffledcards[0].Clone();
            }

            cards.Add(obj);
            if(Game._instance.state==ScreenState.ShopScreen)
                obj.GetComponent<Card>().ShowPrice();
        }
    }
    public void CreateOrders(){
        GameObject obj;
        List<KingsOrder> shuffledcards = Game._instance.AllOrders.OrderBy(_ => rng.Next()).ToList();
        for(int i=0; i<2;i++){
            Vector2 localPosition = new Vector2(i+i+4, 2);
            obj = Instantiate(Game._instance.card, localPosition, Quaternion.identity);
            //AllAbilities.Sort();
            //int s = Random.Range (0, AllAbilities.Count);
            orders.Add(obj);
            obj.GetComponent<Card>().order = shuffledcards[i].Clone();
            //cards.Add(obj);
            obj.GetComponent<Card>().ShowPrice();
        }
        
    }

    public void CreatePieces(){
        GameObject obj;
        for(int i=0; i<3;i++){
            Vector2 localPosition = new Vector2(i, 2);
            obj = PieceFactory._instance.CreateRandomPiece();
            obj.transform.position=localPosition;
            Chessman cm = obj.GetComponent<Chessman>();
            cm.xBoard=7+i;
            cm.yBoard = 3;
            cm.UpdateUIPosition();
            SpriteRenderer rend = obj.GetComponent<SpriteRenderer>();
            rend.sortingOrder = 5;
            pieces.Add(obj);
        }
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

    public void toggleCardColliders(){
        foreach (var card in cards)
        {
            if(card!=null)
                card.GetComponent<BoxCollider2D>().enabled = !card.GetComponent<BoxCollider2D>().enabled;
        }
        foreach (var order in orders)
        {
            if(order!=null)
                order.GetComponent<BoxCollider2D>().enabled = !order.GetComponent<BoxCollider2D>().enabled;
        }
    }

    public void RerollAbilities(){
        if(Game._instance.hero.playerCoins>=rerollCost){
            Game._instance.hero.playerCoins-=rerollCost;
            rerollCost+=rerollCostIncrease;
            rerollCostText.text = rerollCost.ToString();
            UpdateCurrency();
            ClearCards();
            CreateCards();
        }
        
    }

    public void UpdateCurrency(){
        bloodText.text = ": "+Game._instance.hero.playerBlood;
        coinText.text = ": "+Game._instance.hero.playerCoins;
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
        foreach (GameObject piece in Game._instance.hero.inventoryPieces)
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
        Game._instance.CloseShop();
        gameObject.SetActive(false);
    }

    


}
