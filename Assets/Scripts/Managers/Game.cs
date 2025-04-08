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
    public List<KingsOrder> AllOrders;
    public List<Dialogue> AllDialogues;

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
    public int abandonedPieces;
    public static Game _instance;

    //Variables for selecting cards
    public Card selectedCard;
    private List<GameObject> cards = new List<GameObject>();
    private Chessman selectedPiece;
    public bool applyingAbility=false;
    public bool isInMenu =false;
    public bool isDecimating =false;
    public bool pauseOverride =false;
    public bool pause =false;
    public bool endEpisode = false;
    public bool tileSelect = false;
    public bool shopUsed =false;
    public bool tutorial =false;



    //Events
    public UnityEvent<Chessman, Chessman> OnPieceCaptured = new UnityEvent<Chessman, Chessman>();
    public UnityEvent<Chessman, int, bool, BoardPosition> OnAttack = new UnityEvent<Chessman, int, bool, BoardPosition>();
    public UnityEvent<Chessman, Chessman> OnAttackStart = new UnityEvent<Chessman, Chessman>();
    public UnityEvent<Chessman, Chessman, int, int> OnAttackEnd = new UnityEvent<Chessman, Chessman, int, int>();
    public UnityEvent<Chessman, BoardPosition> OnMove = new UnityEvent<Chessman, BoardPosition>();
    public UnityEvent<Chessman, BoardPosition> OnRawMoveEnd = new UnityEvent<Chessman, BoardPosition>();
    public UnityEvent<Chessman, Chessman, bool> OnPieceBounced = new UnityEvent<Chessman,Chessman, bool>();
    public UnityEvent<Chessman, Chessman, Chessman> OnSupportAdded = new UnityEvent<Chessman, Chessman, Chessman>();
    public UnityEvent OnChessMatchStart = new UnityEvent();
    public UnityEvent<PieceColor> OnGameEnd= new UnityEvent<PieceColor>();
    public UnityEvent OnSoulBonded= new UnityEvent();
    public UnityEvent<Chessman> OnPieceAdded = new UnityEvent<Chessman>();

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
        NameDatabase.LoadNames();
        if(SceneLoadManager.LoadPreviousSave){
            LoadGame();
        }
        else{
            BoardManager._instance.CreateBoard();
            LetsBegin();
        }
        
    }

    public void LetsBegin(){
        DialogueManager._instance.StartDialogue(AllDialogues[0]);
        heroColor=PieceColor.White;
        opponent.pieces = PieceFactory._instance.CreateKnightsOfTheRoundTable(opponent, opponent.color, Team.Enemy);
        hero.pieces = PieceFactory._instance.CreatePiecesForColor(hero, hero.color, Team.Hero);
        hero.Initialize();
        opponent.Initialize();
        NewMatch(hero, opponent);
    }

    public void LoadGame(){
        var quickSaveReader = QuickSaveReader.Create("Game");
        PlayerData player;
        List<MapNodeData> mapNodes;
        quickSaveReader.TryRead<PlayerData>("Player", out player);
        quickSaveReader.TryRead<ScreenState>("State", out state);
        quickSaveReader.TryRead<int>("Level", out level);
        quickSaveReader.TryRead<List<MapNodeData>>("MapNodes", out mapNodes);

        Debug.Log($"Resuming state {state}");
        MapManager._instance.LoadMap(mapNodes);
        Game._instance.hero.playerBlood=player.blood;
        Game._instance.hero.playerCoins=player.coins;

        PieceFactory._instance.LoadPieces(player.pieces);

        OpenMap();
        
    }

    public void Tutorial(){
        heroColor=PieceColor.White;
        opponent.pieces = PieceFactory._instance.CreatePiecesForColor(opponent, opponent.color, Team.Enemy);
        hero.pieces = PieceFactory._instance.CreatePiecesForColor(hero, hero.color, Team.Hero);
        hero.Initialize();
        opponent.Initialize();
        ChessMatch match =  new ChessMatch(hero, opponent);
        currentMatch = match;
        match.TutorialMatch();
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isInMenu=true;
            PauseMenuManager._instance.OpenMenu();
        }
    }
    private IEnumerator ApplyAbility(Chessman target){
        if(selectedCard.price.activeSelf){
            if(selectedCard.ability.Cost>hero.playerCoins){
                selectedCard.GetComponent<MMSpringPosition>().BumpRandom();
                selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                selectedPiece.GetComponent<SpriteRenderer>().color = Color.white;
                selectedCard=null;
                selectedPiece=null;
                yield break;
            }
            hero.playerCoins-=selectedCard.ability.Cost;
            ShopManager._instance.UpdateCurrency();
        }
        applyingAbility=true;
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(selectedCard.Dissolve());
        selectedCard.Use(target);
        audioSource.clip = ability;
        yield return new WaitUntil(() => selectedCard.isDissolved);
        audioSource.Play();
        StatBoxManager._instance.SetAndShowStats(selectedPiece);
        Destroy(selectedCard.gameObject);
        ClearCard();
        ClearPiece(); 
        applyingAbility=false;
        yield return new WaitForSeconds(waitTime);
        if(state==ScreenState.RewardScreen)
            InventoryManager._instance.CloseInventory();
        yield break;
    }
    public void CardSelected(Card card){
        SpriteRenderer sprite;
        if (selectedCard != null && selectedCard == card){
            sprite= selectedCard.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
            card.flames.Stop();
            selectedCard=null;
        }
        else if(selectedCard != null && selectedCard != card){
            sprite= selectedCard.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
            selectedCard.flames.Stop();
            selectedCard = card;
            sprite = selectedCard.GetComponent<SpriteRenderer>();
            sprite.color = Color.green;
            selectedCard.flames.Play();
            
        }
        else{
            selectedCard = card;
            sprite = selectedCard.GetComponent<SpriteRenderer>();
            sprite.color = Color.green;
            selectedCard.flames.Play();
        }
    }
    public void ClearCard(){
        selectedCard = null;
        foreach (var card in cards)
        {
            if(card!=null)
            Destroy(card);
        }
        
    }
    /* public void CreateCards(){
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
        
    } */
    public void PieceSelected(Chessman piece){
        if (selectedPiece != null && selectedPiece == piece){
            //sprite= selectedPiece.GetComponent<SpriteRenderer>();
            //sprite.color = Color.white;
            selectedPiece.flames.Stop();
            selectedPiece=null;
        }
        else if(selectedPiece != null && selectedPiece != piece){
            //sprite= selectedPiece.GetComponent<SpriteRenderer>();
            
            selectedPiece.flames.Stop();
            selectedPiece = piece;
            //sprite = selectedPiece.GetComponent<SpriteRenderer>();
            //sprite.color = Color.green;
            selectedPiece.flames.Play();
            StatBoxManager._instance.SetAndShowStats(piece);
        }
        else{
            selectedPiece = piece;
            //sprite = selectedPiece.GetComponent<SpriteRenderer>();
            //sprite.color = Color.green;
            selectedPiece.flames.Play();
            StatBoxManager._instance.SetAndShowStats(piece);
        }
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

    public void OpenArmyManagement(){
        //ResetPlayerPieces();
        state=ScreenState.ManagementScreen;
        ArmyManager._instance.OpenShop();
    }

    public void CloseArmyManagement(){
        //ResetPlayerPieces();
        state=ScreenState.Map;
        OpenMap();
    }

    public void OpenShop(){
        state=ScreenState.ShopScreen;
        ShopManager._instance.OpenShop();
        shopUsed=true;
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
        shopUsed=false;
        opponent.DestroyPieces();
        BoardManager._instance.CreateBoard();
        opponent.pieces = PieceFactory._instance.CreateOpponentPieces(opponent, enemyType);
        opponent.Initialize();
        opponent.LevelUp(level, enemyType);
        NewMatch(hero, opponent);
    }

    public void CloseShop(){
        state=ScreenState.Map;
        OpenMap();
    }
    public void OpenMap(){
        KingsOrderManager._instance.Hide();
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

    public void StopHeroFlames(){
        foreach (var piece in hero.pieces)
        {
            piece.GetComponent<Chessman>().flames.Stop();
        }
    }
    public void StopHeroHighlights(){
        foreach (var piece in hero.pieces)
        {
            piece.GetComponent<Chessman>().highlightedParticles.Stop();
        }
    }
}
