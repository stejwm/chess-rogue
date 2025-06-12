using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System.Linq;
using Rand= System.Random;
using MoreMountains.Feedbacks;
using System.Text.RegularExpressions;
using CI.QuickSave;


public enum ScreenState
{
    MainGameboard,
    RewardScreen,
    PrisonersMarket,
    ActiveMatch,
    Map,
    ShopScreen,
    ManagementScreen
}

public class GameManager : MonoBehaviour
{
    public Player hero;
    //public AIPlayer white;
    public Player opponent;
    [SerializeField] private Board board;
    [SerializeField] private GameInputRouter inputRouter;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private KingsOrderManager kingsOrderManager;

    public GameObject card;
    public PieceColor heroColor;
    public AudioSource audioSource;
    public AudioClip capture;
    public AudioClip bounce;
    public AudioClip move;
    public AudioClip ability;

    public int level = 0;
    public float waitTime;
    public ScreenState state = ScreenState.MainGameboard;
    private static Rand rng = new Rand();
    public Card selectedCard;
    private List<GameObject> cards = new List<GameObject>();
    private Chessman selectedPiece;
    public bool applyingAbility = false;
    public bool shopUsed = false;
    

    public void Start()
    {
        NameDatabase.LoadNames();
        if (SceneLoadManager.LoadPreviousSave)
        {
            LoadGame();
        }
        else
        {
            LetsBegin();
        }

    }
    public void Update()
    {
        if (selectedCard && selectedPiece)
        {
            if (!applyingAbility)
                StartCoroutine(ApplyAbility(selectedPiece));
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenuManager._instance.OpenMenu();
        }
    }
    public void LetsBegin()
    {
        heroColor = PieceColor.White;
        opponent.pieces = PieceFactory._instance.CreateKnightsOfTheRoundTable(board, opponent, opponent.color);
        hero.pieces = PieceFactory._instance.CreatePiecesForColor(board, hero, hero.color);
        hero.Initialize();
        opponent.Initialize();
        board.CreateNewMatch(hero, opponent);
        inputRouter.SetReceiver(board.CurrentMatch);
    }
    public void LoadGame()
    {
        var quickSaveReader = QuickSaveReader.Create("Game");
        PlayerData player;
        List<MapNodeData> mapNodes;
        quickSaveReader.TryRead<PlayerData>("Player", out player);
        quickSaveReader.TryRead<ScreenState>("State", out state);
        quickSaveReader.TryRead<int>("Level", out level);
        quickSaveReader.TryRead<bool>("Shop", out shopUsed);
        quickSaveReader.TryRead<List<MapNodeData>>("MapNodes", out mapNodes);

        Debug.Log($"Resuming state {state}");
        mapManager.LoadMap(mapNodes);
        hero.playerBlood = player.blood;
        hero.playerCoins = player.coins;

        hero.pieces = PieceFactory._instance.LoadPieces(board, player.pieces, hero);

        OpenMap();

    }
    public void EndMatch()
    {
        OpenMarket();
        state = ScreenState.PrisonersMarket;
    }
    public void OpenMarket()
    {
        kingsOrderManager.Hide();
        MarketManager._instance.OpenMarket();
        this.state = ScreenState.PrisonersMarket;
    }

/*     public void OpenReward()
    {
        board.ResetPlayerPieces();
        state = ScreenState.RewardScreen;
        InventoryManager._instance.OpenInventory();
    }
    public void CloseReward()
    {
        state = ScreenState.MainGameboard;
        OpenShop();
    } */
    public void OpenShop()
    {
        state = ScreenState.ShopScreen;
        //ShopManager._instance.OpenShop();
        shopUsed = true;
    }
    public void CloseShop()
    {
        state = ScreenState.ShopScreen;
        OpenMap();
    }
    public void OpenMap()
    {
        //KingsOrderManager._instance.Hide();
        //MapManager._instance.OpenMap();
        this.state = ScreenState.Map;
    }
    public void CloseMap()
    {
        state = ScreenState.MainGameboard;
    }
    public void NextMatch(EnemyType enemyType)
    {
        level++;
        //state=ScreenState.ActiveMatch;
        shopUsed = false;
        opponent.DestroyPieces();
        opponent.pieces = PieceFactory._instance.CreateOpponentPieces(board, opponent, enemyType);
        opponent.Initialize();
        opponent.LevelUp(level, enemyType);
        board.CreateNewMatch(hero, opponent);
    }

    public void OpenArmyManagement()
    {
        state = ScreenState.ManagementScreen;
        //ArmyManager._instance.OpenShop();
    }

    public void CloseArmyManagement()
    {
        //ResetPlayerPieces();
        state = ScreenState.ShopScreen; ;
        //ShopManager._instance.UnHideShop();
    }

    

    

    
    
    #region These should be moved to a different class
    private IEnumerator ApplyAbility(Chessman target)
    {
        if (selectedCard.price.activeSelf)
        {
            if (selectedCard.ability.Cost > hero.playerCoins)
            {
                selectedCard.GetComponent<MMSpringPosition>().BumpRandom();
                selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                selectedPiece.GetComponent<SpriteRenderer>().color = Color.white;
                selectedCard = null;
                selectedPiece = null;
                yield break;
            }
            hero.playerCoins -= selectedCard.ability.Cost;
            //ShopManager._instance.UpdateCurrency();
        }
        applyingAbility = true;
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(selectedCard.Dissolve());
        selectedCard.Use(board, target);
        audioSource.clip = ability;
        yield return new WaitUntil(() => selectedCard.isDissolved);
        audioSource.Play();
        StatBoxManager._instance.SetAndShowStats(selectedPiece);
        Destroy(selectedCard.gameObject);
        ClearCard();
        ClearPiece();
        applyingAbility = false;
        yield return new WaitForSeconds(waitTime);
        if (state == ScreenState.RewardScreen)
            //InventoryManager._instance.CloseInventory();
        yield break;
    }
    public void CardSelected(Card card)
    {
        SpriteRenderer sprite;
        if (selectedCard != null && selectedCard == card)
        {
            sprite = selectedCard.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
            card.flames.Stop();
            selectedCard = null;
        }
        else if (selectedCard != null && selectedCard != card)
        {
            sprite = selectedCard.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
            selectedCard.flames.Stop();
            selectedCard = card;
            sprite = selectedCard.GetComponent<SpriteRenderer>();
            //sprite.color = Color.green;
            selectedCard.flames.Play();

        }
        else
        {
            selectedCard = card;
            sprite = selectedCard.GetComponent<SpriteRenderer>();
            selectedCard.flames.Play();
        }
    }
    public void ClearCard()
    {
        selectedCard = null;
        foreach (var card in cards)
        {
            if (card != null)
                Destroy(card);
        }

    }
    public void PieceSelected(Chessman piece)
    {
        if (selectedPiece != null && selectedPiece == piece)
        {
            //sprite= selectedPiece.GetComponent<SpriteRenderer>();
            //sprite.color = Color.white;
            selectedPiece.flames.Stop();
            selectedPiece = null;
        }
        else if (selectedPiece != null && selectedPiece != piece)
        {
            //sprite= selectedPiece.GetComponent<SpriteRenderer>();

            selectedPiece.flames.Stop();
            selectedPiece = piece;
            //sprite = selectedPiece.GetComponent<SpriteRenderer>();
            //sprite.color = Color.green;
            selectedPiece.flames.Play();
            StatBoxManager._instance.SetAndShowStats(piece);
        }
        else
        {
            selectedPiece = piece;
            //sprite = selectedPiece.GetComponent<SpriteRenderer>();
            //sprite.color = Color.green;
            selectedPiece.flames.Play();
            StatBoxManager._instance.SetAndShowStats(piece);
        }
    }
    public void ClearPiece()
    {
        SpriteRenderer sprite;
        if (selectedPiece != null)
        {
            sprite = selectedPiece.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
        }
        selectedPiece = null;

    }
    #endregion


    
    

    
    

    


    

}
