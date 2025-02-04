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
    //public AIPlayer white;
    public Player opponent;
    //public AIPlayer black;
    public GameObject card;
    public PieceColor heroColor;
    public AudioSource audioSource;
    public AudioClip capture;
    public AudioClip bounce;
    public AudioClip move;
    public AudioClip ability;
    public ChessMatch currentMatch;
    public int level=0;
    public float waitTime;
    public List<Ability> AllAbilities; // Drag-and-drop ScriptableObject assets here in the Inspector

    public List<GameObject> AllOpponents;
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
    public bool pauseOverride =false;
    public bool pause =false;
    public bool endEpisode = false;



    //Events
    public UnityEvent<Chessman, Chessman> OnPieceCaptured = new UnityEvent<Chessman, Chessman>();
    public UnityEvent<Chessman,int, bool, BoardPosition> OnAttack = new UnityEvent<Chessman,int, bool, BoardPosition>();
    public UnityEvent<Chessman, Chessman, int, int> OnAttackEnd = new UnityEvent<Chessman, Chessman, int, int>();
    public UnityEvent<Chessman> OnMove = new UnityEvent<Chessman>();
    public UnityEvent<Chessman, Chessman, bool> OnPieceBounced = new UnityEvent<Chessman,Chessman, bool>();
    public UnityEvent<Chessman, Chessman, Chessman> OnSupportAdded = new UnityEvent<Chessman, Chessman, Chessman>();
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
        //Time.timeScale = 0.5f;
        BoardManager._instance.CreateBoard();
        heroColor=PieceColor.White;
        opponent.pieces = PieceFactory._instance.CreateKnightsOfTheRoundTable(opponent, opponent.color, Team.Enemy);
        hero.pieces = PieceFactory._instance.CreatePiecesForColor(hero, hero.color, Team.Hero);
        hero.Initialize();
        opponent.Initialize();
        NewMatch(hero, opponent);
        
    }

    public void OpenMarket(){
        MarketManager._instance.OpenMarket();
        this.state=ScreenState.PrisonersMarket;
    }
    public void NewMatch(Player white, Player black){
        state = ScreenState.ActiveMatch;
        currentMatch = new ChessMatch(white, black);
        currentMatch.CheckInventory();
    }

    public void Pause(){
        pauseOverride=!pauseOverride;
        pause=true;

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
        if(pause && !pauseOverride){
            currentMatch.NextTurn();
            pause=false;
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

    public void NextMatch(EnemyType enemyType){
        level++;
        state=ScreenState.ActiveMatch;
        opponent.DestroyPieces();
        BoardManager._instance.CreateBoard();
        opponent.pieces = PieceFactory._instance.CreateOpponentPieces(opponent, enemyType);
        opponent.Initialize();
        opponent.LevelUp(level);
        NewMatch(hero, opponent);
    }

    public void CloseShop(){
        state=ScreenState.MainGameboard;
        OpenMap();
    }
    public void OpenMap(){
        MapManager._instance.OpenMap();
        this.state=ScreenState.Map;
    }

    public void toggleAllPieceColliders(bool active){
        foreach (var piece in hero.pieces)
        {
            piece.GetComponent<BoxCollider2D>().enabled=active;
        }
        foreach (var piece in opponent.pieces)
        {
            piece.GetComponent<BoxCollider2D>().enabled=active;
        }
        foreach (var piece in hero.capturedPieces)
        {
            piece.GetComponent<BoxCollider2D>().enabled=active;
        }
        foreach (var piece in opponent.capturedPieces)
        {
            piece.GetComponent<BoxCollider2D>().enabled=active;
        }
    }
    public void togglePieceColliders(List<GameObject> pieces, bool active){
        foreach (var piece in pieces)
        {
            piece.GetComponent<BoxCollider2D>().enabled=active;
        }
    }
}
