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

public class Game : MonoBehaviour
{
    //Reference from Unity IDE
    public GameObject pawn;
    public GameObject knight;
    public GameObject bishop;
    public GameObject rook;
    public GameObject queen;
    public GameObject king;
    public GameObject card;
    public ArrayList playerWhite;
    public ArrayList playerBlack;
    public PieceColor heroColor;
    public AudioSource audioSource;
    public AudioClip capture;
    public AudioClip bounce;
    public AudioClip ability;
    public float waitTime;
    public bool isInInventory;
    public List<Ability> AllAbilities; // Drag-and-drop ScriptableObject assets here in the Inspector
    public PlayerAgent opponent;
    public bool isBounceReduced;
    public bool readyForOpponent;
    Dictionary<GameObject, BoardPosition> startingPositions = new Dictionary<GameObject, BoardPosition>();
    private static Rand rng = new Rand();


    //Matrices needed, positions of each of the GameObjects
    //Also separate arrays for the players in order to easily keep track of them all
    //Keep in mind that the same objects are going to be in "positions" and "playerBlack"/"playerWhite"
    private GameObject[,] positions = new GameObject[8, 8];
    
    
    //Variables for handling attacks
    private int targetedX;
    private int targetedY;
    private int totalAttackPower;
    private int totalDefensePower;
    public int attackSupport;
    public int defenseSupport;
    public int baseAttack;
    public int baseDefense;
    private bool readyForCleanup;
    private ArrayList defendingUnits;
    private ArrayList attackingUnits;


    //Variables for selecting cards
    private Card selectedCard;
    private List<GameObject> cards = new List<GameObject>();
    [SerializeField] private List<Ability> abilities;
    private Chessman selectedPiece;
    private bool applyingAbility=false;



    //current turn
    private PieceColor currentPlayer = PieceColor.White;

    //Game Ending
    private bool gameOver = false;


    //Events
    public UnityEvent<Chessman> OnPieceCaptured = new UnityEvent<Chessman>();
    public UnityEvent<Chessman,int> OnAttack = new UnityEvent<Chessman,int>();
    public UnityEvent<Chessman, int> OnAttackEnd = new UnityEvent<Chessman, int>();
    public UnityEvent<Chessman> OnMove = new UnityEvent<Chessman>();
    public UnityEvent<Chessman, Chessman, bool> OnPieceBounced = new UnityEvent<Chessman,Chessman, bool>();
    public UnityEvent<Chessman> OnSupportAdded = new UnityEvent<Chessman>();
    public UnityEvent<PieceColor> OnGameEnd= new UnityEvent<PieceColor>();


    //Unity calls this right when the game starts, there are a few built in functions
    //that Unity can call for you
    public void Start()
    {
        playerWhite = new ArrayList(new GameObject[] { Create(PieceType.Rook,"white_rook", 0, 0,PieceColor.White, Team.Hero), Create(PieceType.Knight,"white_knight", 1, 0,PieceColor.White, Team.Hero),
            Create(PieceType.Bishop,"white_bishop", 2, 0,PieceColor.White, Team.Hero), Create(PieceType.Queen,"white_queen", 3, 0,PieceColor.White, Team.Hero), Create(PieceType.King,"white_king", 4, 0,PieceColor.White, Team.Hero),
            Create(PieceType.Bishop,"white_bishop", 5, 0,PieceColor.White, Team.Hero), Create(PieceType.Knight,"white_knight", 6, 0,PieceColor.White, Team.Hero), Create(PieceType.Rook,"white_rook", 7, 0,PieceColor.White, Team.Hero),
            Create(PieceType.Pawn,"white_pawn", 0, 1,PieceColor.White, Team.Hero), Create(PieceType.Pawn,"white_pawn", 1, 1,PieceColor.White, Team.Hero), Create(PieceType.Pawn,"white_pawn", 2, 1,PieceColor.White, Team.Hero),
            Create(PieceType.Pawn,"white_pawn", 3, 1,PieceColor.White, Team.Hero), Create(PieceType.Pawn,"white_pawn", 4, 1,PieceColor.White, Team.Hero), Create(PieceType.Pawn,"white_pawn", 5, 1,PieceColor.White, Team.Hero),
            Create(PieceType.Pawn,"white_pawn", 6, 1,PieceColor.White, Team.Hero), Create(PieceType.Pawn,"white_pawn", 7, 1,PieceColor.White, Team.Hero) });
        playerBlack = new ArrayList(new GameObject[] { Create(PieceType.Rook,"black_rook", 0, 7,PieceColor.Black, Team.Enemy), Create(PieceType.Knight,"black_knight",1,7,PieceColor.Black, Team.Enemy),
            Create(PieceType.Bishop,"black_bishop",2,7,PieceColor.Black, Team.Enemy), Create(PieceType.Queen,"black_queen",3,7,PieceColor.Black, Team.Enemy), Create(PieceType.King,"black_king",4,7,PieceColor.Black, Team.Enemy),
            Create(PieceType.Bishop,"black_bishop",5,7,PieceColor.Black, Team.Enemy), Create(PieceType.Knight,"black_knight",6,7,PieceColor.Black, Team.Enemy), Create(PieceType.Rook,"black_rook",7,7,PieceColor.Black, Team.Enemy),
            Create(PieceType.Pawn,"black_pawn", 0, 6,PieceColor.Black, Team.Enemy), Create(PieceType.Pawn,"black_pawn", 1, 6,PieceColor.Black, Team.Enemy), Create(PieceType.Pawn,"black_pawn", 2, 6,PieceColor.Black, Team.Enemy),
            Create(PieceType.Pawn,"black_pawn", 3, 6,PieceColor.Black, Team.Enemy), Create(PieceType.Pawn,"black_pawn", 4, 6,PieceColor.Black, Team.Enemy), Create(PieceType.Pawn,"black_pawn", 5, 6,PieceColor.Black, Team.Enemy),
            Create(PieceType.Pawn,"black_pawn", 6, 6,PieceColor.Black, Team.Enemy), Create(PieceType.Pawn,"black_pawn", 7, 6,PieceColor.Black, Team.Enemy) });

            heroColor=PieceColor.White;

        for (int i = 0; i < playerBlack.Count; i++)
        {
            var blackPieceObj = (GameObject)playerBlack[i];
            var blackPiece=blackPieceObj.GetComponent<Chessman>();
            var whitePieceObj = (GameObject)playerWhite[i];
            var whitePiece=whitePieceObj.GetComponent<Chessman>();            
            startingPositions.Add(blackPieceObj, new BoardPosition(blackPiece.xBoard,blackPiece.yBoard));
            startingPositions.Add(whitePieceObj, new BoardPosition(whitePiece.xBoard,whitePiece.yBoard));

        }
        //Set all piece positions on the positions board
        ResetBoard();
        
        SetWhiteTurn();
        opponent.StartUp();
        opponent.color=PieceColor.Black;
    }

    public void ResetBoard(){
        playerWhite.Clear();
        playerBlack.Clear();
        Array.Clear(positions, 0, positions.Length);
        foreach (GameObject key in startingPositions.Keys)
        {   
            if(key.GetComponent<Chessman>().color==PieceColor.Black)
                playerBlack.Add(key);
            else if (key.GetComponent<Chessman>().color==PieceColor.White)
                playerWhite.Add(key);
            key.SetActive(true);
            
            SetPosition(key,startingPositions[key].x,startingPositions[key].y); 
            key.GetComponent<Chessman>().SetCoords();
        }
        SetWhiteTurn();
    }

    public GameObject Create(PieceType type, string name, int x, int y, PieceColor color, Team team)
    {
        GameObject obj;
        switch (type)
        {
            case PieceType.Queen:
                obj = Instantiate(queen, new Vector3(0, 0, -1), Quaternion.identity);
                break;
            case PieceType.Knight:
                obj = Instantiate(knight, new Vector3(0, 0, -1), Quaternion.identity); 
                break;               
            case PieceType.Bishop:
                obj = Instantiate(bishop, new Vector3(0, 0, -1), Quaternion.identity);                
                break;
            case PieceType.King:
                obj = Instantiate(king, new Vector3(0, 0, -1), Quaternion.identity);                
                break;
            case PieceType.Rook:
                obj = Instantiate(rook, new Vector3(0, 0, -1), Quaternion.identity);                
                break;
            case PieceType.Pawn:
                obj = Instantiate(pawn, new Vector3(0, 0, -1), Quaternion.identity);                
                break;
            default:
                return null;
            
        }
        Chessman cm = obj.GetComponent<Chessman>(); //We have access to the GameObject, we need the script
        cm.color=color;
        cm.team=team;
        cm.name = name; //This is a built in variable that Unity has, so we did not have to declare it before
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        
        //cm.Activate(); //It has everything set up so it can now Activate()
        return obj;
    }
    
    public void SetWhiteTurn(){
        foreach (GameObject item in playerWhite)
            {
                item.GetComponent<Chessman>().isValidForAttack=true;
            }
        foreach (GameObject item in playerBlack)
            {
                item.GetComponent<Chessman>().isValidForAttack=false;
            }
    }
    public void SetBlackTurn(){
        foreach (GameObject item in playerBlack)
        {
            item.GetComponent<Chessman>().isValidForAttack=true;
        }
        foreach (GameObject item in playerWhite)
        {
            item.GetComponent<Chessman>().isValidForAttack=false;
        }
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();

        //Overwrites either empty space or whatever was there
        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public void SetPosition(GameObject obj, int x, int y)
    {
        Chessman cm = obj.GetComponent<Chessman>();
        cm.xBoard=x;
        cm.yBoard=y;
        SetPosition(obj);
    }

    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    public GameObject[,] GetPositions()
    {
        return positions;
    }
    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }

    public PieceColor GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void NextTurn()
    {
        if (currentPlayer == PieceColor.White)
        {
            currentPlayer = PieceColor.Black;
            SetBlackTurn();
        }
        else
        {
            currentPlayer = PieceColor.White;
            SetWhiteTurn();
        }

        if(opponent.color==currentPlayer){
            StartCoroutine(OpponentMove());
        }
    }

    public void PlayerTurn(){
        currentPlayer=heroColor;
    }
    public void Update()
    {
        /* if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;

            //Using UnityEngine.SceneManagement is needed here
            SceneManager.LoadScene("Game"); //Restarts the game by loading the scene over again
        } */

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
    
    public void Winner(string playerWinner)
    {
        gameOver = true;

        //Using UnityEngine.UI is needed here
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = playerWinner + " is the winner";

        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
    }

    public void HandleMove(Chessman piece, int x, int y, bool attack){
        Chessman movingPiece = piece;
        
        OnMove.Invoke(piece);
        //Set the Chesspiece's original location to be empty
        SetPositionEmpty(movingPiece.xBoard, movingPiece.yBoard);

        //Destroy the victim Chesspiece
        if (attack)
        {
            Chessman attackedPiece = GetPosition(x,y).GetComponent<Chessman>();
            HandleAttack(movingPiece, attackedPiece, x, y);   
        }
        else{

            //Move reference chess piece to this position
            movingPiece.SetXBoard(x);
            movingPiece.SetYBoard(y);
            movingPiece.SetCoords();

            //Update the matrix
            SetPosition(movingPiece.gameObject);

            

            //Destroy the move plates including self
            movingPiece.DestroyMovePlates();
            //Switch Current Player
            NextTurn();
        }
    }
    public void HandleAttack(Chessman movingPiece, Chessman attackedPiece, int x, int y){
        targetedX = x;
        targetedY = y;
        //Set attacked piece to empty for valid support
        SetPositionEmpty(attackedPiece.GetXBoard(), attackedPiece.GetYBoard());
        
        if(attackedPiece.color==PieceColor.White){
            defendingUnits = new ArrayList(playerWhite);
            attackingUnits = new ArrayList(playerBlack);
        }
        else if(attackedPiece.color==PieceColor.Black){
            defendingUnits = new ArrayList(playerBlack);
            attackingUnits = new ArrayList(playerWhite);
        }
        /* Debug.Log("All Black Pieces: ");
        foreach( GameObject piece in playerBlack) {
            Debug.Log(piece.name);
        }
        Debug.Log("All Defending units: ");
        foreach( GameObject piece in defendingUnits) {
            Debug.Log(piece.name);
        } */
        //Calculate attacking support first
        StartCoroutine(AddSupport(movingPiece, attackedPiece, true));
    }

    private IEnumerator AddSupport(Chessman movingPiece, Chessman attackedPiece, bool isAttacking){
        ArrayList pieces;
        Chessman targetPiece;
        int supportPower=0;
        if(isAttacking){
            pieces=attackingUnits;
            targetPiece=movingPiece;
        }
        else{
            pieces=defendingUnits;
            targetPiece=attackedPiece;
        }
        
        foreach (GameObject pieceObject in pieces)
        {
            Debug.Log("Checking support from "+ pieceObject.name);
            var piece =pieceObject.GetComponent<Chessman>() ;
            if (piece == targetPiece)
                continue; 
            foreach (var coordinate in piece.GetValidSupportMoves()){
                if (coordinate.x==targetedX && coordinate.y==targetedY){
                    supportPower+= piece.CalculateSupport();
                    Vector2 localPosition = new Vector2(2, 2);
                    var bonusPopUpInstance= SpawnsBonusPopups.Instance.BonusAdded(piece.CalculateSupport(), localPosition);
                    RectTransform rt = bonusPopUpInstance.GetComponent<RectTransform>();
                    rt.position = pieceObject.transform.position;
                    Debug.Log("Spawned bonus for " + piece.name + " at position " + rt.position);
                    yield return new WaitForSeconds(waitTime/2);
                    OnSupportAdded.Invoke(piece);
                    break;
                }
            }
        }
        OnAttack.Invoke(targetPiece, supportPower); 
        yield return new WaitForSeconds(waitTime);
        if(isAttacking){
            baseAttack=targetPiece.CalculateAttack();
            totalAttackPower=baseAttack+supportPower;
            attackSupport=supportPower;
        }else{   
            baseDefense=targetPiece.CalculateDefense();
            totalDefensePower = baseDefense+supportPower;
            defenseSupport=supportPower;
        }
        Debug.Log("Ready for battle panel?: "+ readyForCleanup);
        if(readyForCleanup){
            RunBattlePanel(movingPiece, attackedPiece);
            yield break;
        }
        readyForCleanup=true;
        Debug.Log("Running support check on other pieces.");
        yield return StartCoroutine(AddSupport(movingPiece, attackedPiece, false));
        
    }
    
    private void RunBattlePanel(Chessman movingPiece, Chessman attackedPiece){
        Debug.Log("Starting Battle Panel");

        //if (attackedPiece.name == "white_king") Winner("black");
        //if (attackedPiece.name == "black_king") Winner("white");

        

        StartCoroutine(ShowBattlePanel(movingPiece, attackedPiece));
    }
    private void AttackCleanUp(Chessman movingPiece){
        OnAttackEnd.Invoke(movingPiece, attackSupport); 
        //Move reference chess piece to this position
        movingPiece.xBoard=targetedX;
        movingPiece.yBoard=targetedY;
        movingPiece.SetCoords();

        //Update the matrix
        SetPosition(movingPiece.gameObject);

        

        //Destroy the move plates including self
        movingPiece.DestroyMovePlates();

        //reset all variables
        targetedX = -1;
        targetedY =-1;
        totalAttackPower=0;
        totalDefensePower=0;
        attackSupport=0;
        defenseSupport=0;
        baseAttack=0;
        baseDefense=0;
        readyForCleanup=false;
        defendingUnits.Clear();
        attackingUnits.Clear();
        //Switch Current Player
        NextTurn();
    }

    private IEnumerator ShowBattlePanel(Chessman movingPiece, Chessman attackedPiece){

        var attackVal = movingPiece.CalculateAttack();
        var supportVal= attackSupport;
        var sprite = movingPiece.GetComponent<SpriteRenderer>().sprite;

        var defendVal = attackedPiece.CalculateDefense();
        var defendSupportVal= defenseSupport;
        var defendSprite = attackedPiece.GetComponent<SpriteRenderer>().sprite;
        //If hero is attacking
        if(movingPiece.team==Team.Hero){
            BattlePanel._instance.SetAndShowAttackingStats(attackVal.ToString(),supportVal.ToString(),totalAttackPower.ToString(),movingPiece.name,
            sprite,defendVal.ToString(),defendSupportVal.ToString(),totalDefensePower.ToString(),attackedPiece.name,defendSprite);
            BattlePanel._instance.SetAndShowHeroAttack(attackVal);
            yield return new WaitForSeconds(waitTime/2);
            BattlePanel._instance.SetAndShowHeroSupport(supportVal);
            yield return new WaitForSeconds(waitTime/2);
            BattlePanel._instance.SetAndShowHeroTotal(totalAttackPower);
            yield return new WaitForSeconds(waitTime/2);
            BattlePanel._instance.SetAndShowEnemyAttack(defendVal);
            yield return new WaitForSeconds(waitTime/2);
            BattlePanel._instance.SetAndShowEnemySupport(defendSupportVal);
            yield return new WaitForSeconds(waitTime/2);
            BattlePanel._instance.SetAndShowEnemyTotal(totalDefensePower);

        }
        //If enemy is attacking
        else if(attackedPiece.team==Team.Hero){
            BattlePanel._instance.SetAndShowDefendingStats(defendVal.ToString(),defendSupportVal.ToString(),totalDefensePower.ToString(),attackedPiece.name,
            defendSprite,attackVal.ToString(),supportVal.ToString(),totalAttackPower.ToString(),movingPiece.name,sprite);
            
            BattlePanel._instance.SetAndShowEnemyAttack(attackVal);
            yield return new WaitForSeconds(waitTime/2);
            BattlePanel._instance.SetAndShowEnemySupport(supportVal);
            yield return new WaitForSeconds(waitTime/2);
            BattlePanel._instance.SetAndShowEnemyTotal(totalAttackPower);
            yield return new WaitForSeconds(waitTime/2);

            BattlePanel._instance.SetAndShowHeroAttack(defendVal);
            yield return new WaitForSeconds(waitTime/2);
            BattlePanel._instance.SetAndShowHeroSupport(defendSupportVal);
            yield return new WaitForSeconds(waitTime/2);
            BattlePanel._instance.SetAndShowHeroTotal(totalDefensePower);
            //yield return new WaitForSeconds(waitTime);
            
        }
        

        yield return new WaitForSeconds(waitTime);
        if(totalAttackPower>=totalDefensePower){
            if (attackedPiece.type==PieceType.King){
                movingPiece.DestroyMovePlates();
                EndGame();
                yield break;
            }
            Debug.Log(movingPiece.name + " captures "+ attackedPiece.name);
            
            if (playerBlack.Contains(attackedPiece.gameObject))
                playerBlack.Remove(attackedPiece.gameObject);
            if(playerWhite.Contains(attackedPiece.gameObject))
                playerWhite.Remove(attackedPiece.gameObject);
            //Debug.Log("Setting capture tone");
            audioSource.clip = capture; 
            BattlePanel._instance.SetAndShowResults("Capture!");   
            attackedPiece.gameObject.SetActive(false);
            AttackCleanUp(movingPiece);
            OnPieceCaptured.Invoke(movingPiece);  // Trigger the event
              
        }
        else{
            Debug.Log(movingPiece.name + " failed to capture "+ attackedPiece.name);
            isBounceReduced=false;

            //Reset attacked pieces position if capture failed 
            attackedPiece.SetXBoard(targetedX);
            attackedPiece.SetYBoard(targetedY);
            //update x and y coords with original location
            targetedX = movingPiece.GetXBoard();
            targetedY = movingPiece.GetYBoard();
            if(attackedPiece.CalculateAttack()>0){
                attackedPiece.attackBonus-=1;
                isBounceReduced =true;
            }
            if(attackedPiece.CalculateDefense()>0){
                attackedPiece.defenseBonus-=1;
                isBounceReduced =true;
            }
            SetPosition(attackedPiece.gameObject);
            attackedPiece.SetCoords();
            //Debug.Log("Setting bounce tone");
            audioSource.clip = bounce;
            BattlePanel._instance.SetAndShowResults("Bounce!"); 
            AttackCleanUp(movingPiece);
            OnPieceBounced.Invoke(movingPiece, attackedPiece, isBounceReduced);

            
            

        }
        
        //Debug.Log("Playing audio");
        audioSource.Play();
        yield return new WaitForSeconds(waitTime);
        BattlePanel._instance.HideResults();   
        BattlePanel._instance.HideStats();
        
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
        //Debug.Log("Piece selected");
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
            sprite= selectedPiece.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
        }
        selectedPiece = null;
        
    }

    public void ExecuteTurn(Chessman piece, int x, int y){
        bool attack =false;
        if (GetPosition(x,y)!=null)
            attack=true;
        HandleMove(piece,x,y,attack);
    }

    private IEnumerator OpponentMove(){
        yield return new WaitForSeconds(waitTime);
        if(currentPlayer==opponent.color)
            opponent.RequestDecision();
        yield break;
    }

    private void EndGame(){
        BattlePanel._instance.HideResults();   
        BattlePanel._instance.HideStats();
        ResetBoard();
        InventoryManager._instance.OpenInventory();
        //SceneManager.LoadScene(0);
    }
}
