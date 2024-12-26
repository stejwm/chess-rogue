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
public class MoveHistory
{
    public Chessman piece;
    public int fromX;
    public int fromY;
    public int toX;
    public int toY;
    public GameObject capturedPiece;
    public bool wasAttack;
    public bool wasPawnFirstMove;

    public MoveHistory(Chessman piece, int fromX, int fromY, int toX, int toY, GameObject capturedPiece = null, bool wasAttack = false)
    {
        this.piece = piece;
        this.fromX = fromX;
        this.fromY = fromY;
        this.toX = toX;
        this.toY = toY;
        this.capturedPiece = capturedPiece;
        this.wasAttack = wasAttack;
        
        if (piece is Pawn pawn)
        {
            this.wasPawnFirstMove = !pawn.HasMovedBefore();
        }
    }
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
    public UnityEvent<Chessman,int, bool, BoardPosition> OnAttack = new UnityEvent<Chessman,int, bool, BoardPosition>();
    public UnityEvent<Chessman, Chessman, int, int> OnAttackEnd = new UnityEvent<Chessman, Chessman, int, int>();
    public UnityEvent<Chessman> OnMove = new UnityEvent<Chessman>();
    public UnityEvent<Chessman, Chessman, bool> OnPieceBounced = new UnityEvent<Chessman,Chessman, bool>();
    public UnityEvent<Chessman> OnSupportAdded = new UnityEvent<Chessman>();
    public UnityEvent<PieceColor> OnGameEnd= new UnityEvent<PieceColor>();

    public bool debugMode = false;
    private bool previousCapsLockState = false;

    // Add these for undo functionality
    private ChessMatch previousMatchState;
    private List<GameObject> previousWhitePieces = new List<GameObject>();
    private List<GameObject> previousBlackPieces = new List<GameObject>();

    private Stack<MoveHistory> moveHistory = new Stack<MoveHistory>();
    private Stack<MoveHistory> redoHistory = new Stack<MoveHistory>();

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
        // Check for Ctrl+D
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D))
        {
            debugMode = !debugMode;
            Debug.Log($"Debug Mode: {(debugMode ? "Enabled" : "Disabled")}");
        }
        
        // Debug controls
        if (debugMode)
        {
            // Check for Shift+R (Reset)
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Debug.Log("Debug: Resetting Board");
                    currentMatch = new ChessMatch(hero, opponent);
                    currentMatch.ResetBoard();
                    currentMatch.PlayerTurn();
                    moveHistory.Clear();
                    redoHistory.Clear();
                }
            }
            // Regular R (Redo)
            else if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("Debug: Attempting Redo");
                RedoLastMove();
            }
            
            // Undo
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log("Debug: Attempting Undo");
                UndoLastMove();
            }
        }

        // Add debug info to console
        if (debugMode && Input.anyKeyDown)
        {
            Debug.Log($"Key pressed: {Input.inputString}");
        }

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

    // Add this method to store the current state
    public void SaveCurrentState()
    {
        if (!debugMode) return;
        
        previousWhitePieces.Clear();
        previousBlackPieces.Clear();
        
        foreach (var piece in hero.pieces)
        {
            if (piece.activeSelf)
            {
                var chessman = piece.GetComponent<Chessman>();
                previousWhitePieces.Add(piece);
            }
        }
        
        foreach (var piece in opponent.pieces)
        {
            if (piece.activeSelf)
            {
                var chessman = piece.GetComponent<Chessman>();
                previousBlackPieces.Add(piece);
            }
        }
    }

    public void SaveMove(Chessman piece, int fromX, int fromY, int toX, int toY, GameObject capturedPiece = null, bool wasAttack = false)
    {
        if (!debugMode) return;
        moveHistory.Push(new MoveHistory(piece, fromX, fromY, toX, toY, capturedPiece, wasAttack));
        redoHistory.Clear();
        Debug.Log($"Move saved: {piece.name} from {fromX},{fromY} to {toX},{toY}");
    }

    private void UndoLastMove()
    {
        if (!debugMode || moveHistory.Count == 0) return;

        var lastMove = moveHistory.Pop();
        redoHistory.Push(lastMove);
        Debug.Log($"Undoing move: {lastMove.piece.name}");

        // Check if this was a castling move
        if (lastMove.piece is King && Math.Abs(lastMove.toX - lastMove.fromX) == 2)
        {
            // Undo rook move
            bool isKingside = lastMove.toX > lastMove.fromX;
            int rookStartX = isKingside ? lastMove.fromX + 3 : lastMove.fromX - 4;
            int rookEndX = isKingside ? lastMove.toX - 1 : lastMove.toX + 1;
            
            var rookObj = currentMatch.GetPieceAtPosition(rookEndX, lastMove.fromY);
            if (rookObj != null)
            {
                var rook = rookObj.GetComponent<Rook>();
                rook.SetXBoard(rookStartX);
                rook.SetYBoard(lastMove.fromY);
                rook.hasMoved = false;
                rook.UpdateUIPosition();
            }
        }

        // Move piece back
        lastMove.piece.SetXBoard(lastMove.fromX);
        lastMove.piece.SetYBoard(lastMove.fromY);
        lastMove.piece.UpdateUIPosition();
        lastMove.piece.hasMoved = false;  // Reset hasMoved status
        
        // Reset pawn state if applicable
        if (lastMove.piece is Pawn pawn && lastMove.wasPawnFirstMove)
        {
            var pawnField = typeof(Pawn).GetField("hasMovedBefore", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            pawnField.SetValue(pawn, false);
        }
        
        // If it was an attack, restore the captured piece
        if (lastMove.wasAttack && lastMove.capturedPiece != null)
        {
            lastMove.capturedPiece.SetActive(true);
            var capturedPiece = lastMove.capturedPiece.GetComponent<Chessman>();
            capturedPiece.SetXBoard(lastMove.toX);
            capturedPiece.SetYBoard(lastMove.toY);
            capturedPiece.UpdateUIPosition();
        }

        currentMatch.UpdateBoard();
        currentMatch.PlayerTurn();
    }

    private void RedoLastMove()
    {
        if (!debugMode || redoHistory.Count == 0) return;

        var moveToRedo = redoHistory.Pop();
        Debug.Log($"Redoing move: {moveToRedo.piece.name}");

        // Save current state to undo stack before redoing
        SaveMove(moveToRedo.piece, moveToRedo.fromX, moveToRedo.fromY, moveToRedo.toX, moveToRedo.toY, moveToRedo.capturedPiece, moveToRedo.wasAttack);

        // Execute the move
        currentMatch.MovePiece(moveToRedo.piece, moveToRedo.toX, moveToRedo.toY);
        
        if (moveToRedo.wasAttack && moveToRedo.capturedPiece != null)
        {
            moveToRedo.capturedPiece.SetActive(false);
        }

        currentMatch.UpdateBoard();
        currentMatch.PlayerTurn();
    }
}
