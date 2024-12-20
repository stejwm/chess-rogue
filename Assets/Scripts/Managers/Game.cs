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


public enum ScreenState
{
    MainGameboard,
    RewardScreen,
    PrisonersMarket,
    ActiveMatch,
    Map,
    ShopScreen,
}
public class Game : MonoBehaviour
{
    public Player hero;
    public Player opponent;
    public GameObject card;
    public PieceColor heroColor;
    public AudioSource audioSource;
    public AudioClip capture;
    public AudioClip bounce;
    public AudioClip ability;
    public ChessMatch currentMatch;
    public int level=0;
    public float waitTime;
    public List<Ability> AllAbilities; // Drag-and-drop ScriptableObject assets here in the Inspector
    //public PlayerAgent opponent;
    public ScreenState state = ScreenState.MainGameboard;
    private static Rand rng = new Rand();



    //Matrices needed, positions of each of the GameObjects
    //Also separate arrays for the players in order to easily keep track of them all
    //Keep in mind that the same objects are going to be in "positions" and "playerBlack"/"playerWhite"
    private GameObject[,] positions = new GameObject[8, 8];
    public int attackSupport;
    public int defenseSupport;
    public int baseAttack;
    public int baseDefense;
    public static Game _instance;

    //Variables for selecting cards
    private Card selectedCard;
    private List<GameObject> cards = new List<GameObject>();
    private Chessman selectedPiece;
    private bool applyingAbility=false;
    public bool isInMenu =false;



    //Events
    public UnityEvent<Chessman> OnPieceCaptured = new UnityEvent<Chessman>();
    public UnityEvent<Chessman,int> OnAttack = new UnityEvent<Chessman,int>();
    public UnityEvent<Chessman, int> OnAttackEnd = new UnityEvent<Chessman, int>();
    public UnityEvent<Chessman> OnMove = new UnityEvent<Chessman>();
    public UnityEvent<Chessman, Chessman, bool> OnPieceBounced = new UnityEvent<Chessman,Chessman, bool>();
    public UnityEvent<Chessman> OnSupportAdded = new UnityEvent<Chessman>();
    public UnityEvent<PieceColor> OnGameEnd= new UnityEvent<PieceColor>();

    public void Awake(){
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }
    public void Start()
    {
        
        hero.pieces = PieceFactory._instance.CreateWhitePieces(hero);
        heroColor=PieceColor.White;
        opponent.pieces = PieceFactory._instance.CreateBlackPieces(opponent);
        hero.Initialize();
        opponent.Initialize();
        NewMatch();
        //ResetBoard();
        //SetWhiteTurn();
        //opponent.StartUp();
        //opponent.color=PieceColor.Black;
    }

    public void OpenMarket(){
        MarketManager._instance.OpenMarket();
        this.state=ScreenState.PrisonersMarket;
    }
    public void NewMatch(){
        state = ScreenState.ActiveMatch;
        currentMatch = new ChessMatch(hero, opponent);
    }

    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }

    public void Update()
    {
        if(selectedCard && selectedPiece){
            if(!applyingAbility)
                StartCoroutine(ApplyAbility(selectedPiece)); 
        }
    }
    private IEnumerator ApplyAbility(Chessman target){
        applyingAbility=true;
        selectedCard.Use(target);
        audioSource.clip = ability;
        audioSource.Play();
        yield return new WaitForSeconds(waitTime);
        RewardStatManager._instance.SetAndShowStats(selectedPiece);
        ClearCard();
        ClearPiece(); 
        applyingAbility=false;
        yield break;
    }
    public void CardSelected(Card card){
        SpriteRenderer sprite;
        Debug.Log("Card selected");
        if (selectedCard != null){
            sprite= selectedCard.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
        }
        selectedCard = card;
        sprite = selectedCard.GetComponent<SpriteRenderer>();
        sprite.color = Color.green;
    }
    public void ClearCard(){
        selectedCard = null;
        foreach (var card in cards)
        {
            Destroy(card);
        }
        
    }
    public void CreateCards(){
        
        Debug.Log("Creating Cards");
        GameObject obj;
        List<Ability> shuffledcards = AllAbilities.OrderBy(_ => rng.Next()).ToList();
        for(int i=0; i<3;i++){
            Vector2 localPosition = new Vector2(i+i, 2);
            obj = Instantiate(card, localPosition, Quaternion.identity);
            //AllAbilities.Sort();
            //int s = Random.Range (0, AllAbilities.Count);
            
            obj.GetComponent<Card>().ability = shuffledcards[i].Clone();
            cards.Add(obj);
        }
        
    }
    public void PieceSelected(Chessman piece){
        SpriteRenderer sprite;
        Debug.Log(piece.name+" selected");
        if (selectedPiece != null){
            sprite= selectedPiece.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
        }
        selectedPiece = piece;
        sprite = selectedPiece.GetComponent<SpriteRenderer>();
        sprite.color = Color.green;
        RewardStatManager._instance.SetAndShowStats(piece);
    }
    public void ClearPiece(){
        SpriteRenderer sprite;
        if (selectedPiece != null){
            sprite = selectedPiece.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
        }
        selectedPiece = null;
        
    }
    
    public void EndMatch(){
        //currentMatch=null;
        OpenMarket();
        state=ScreenState.PrisonersMarket;
    }

    public void OpenReward(){
        ResetPlayerPieces();
        state=ScreenState.RewardScreen;
        InventoryManager._instance.OpenInventory();
    }
    public void CloseReward(){
        state=ScreenState.Map;
        this.currentMatch=null;
        MapManager._instance.OpenMap();
        //InventoryManager._instance.OpenInventory();
    }

    public void CloseMap(){
        state=ScreenState.MainGameboard;
    }

    public void OpenShop(){
        //ResetPlayerPieces();
        state=ScreenState.ShopScreen;
        ShopManager._instance.OpenShop();
    }
    public void ResetPlayerPieces(){
        foreach (GameObject piece in hero.pieces)
        {
            //Debug.Log(piece.name);
            piece.SetActive(true);
            Chessman cm = piece.GetComponent<Chessman>();
            piece.GetComponent<Chessman>().ResetBonuses();
            currentMatch.MovePiece(cm, cm.startingPosition.x,cm.startingPosition.y);        
        }
    }

    public void NextMatch(){
        level++;
        IncreaseDifficultyMatch();
    }
    public void IncreaseDifficultyMatch(){
        state=ScreenState.ActiveMatch;
        opponent.DestroyPieces();

        opponent.pieces=PieceFactory._instance.CreateBlackPieces(opponent);
        opponent.Initialize();
        for (int i =0; i<level; i++)
            foreach (GameObject piece in opponent.pieces)
            {
                Chessman cm = piece.GetComponent<Chessman>();
                switch (rng.Next(3)){
                    case 0:
                        cm.defense+=1;
                        break;
                    case 1:
                        cm.attack+=1;
                        break;
                    case 2:
                        cm.support+=1;
                        break;
                }
            }
        NewMatch();
    }

    public void CloseShop(){
        state=ScreenState.MainGameboard;
        OpenMap();
    }
    public void OpenMap(){
        MapManager._instance.OpenMap();
        this.state=ScreenState.Map;
    }
}
