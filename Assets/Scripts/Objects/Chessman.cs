using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using System.Linq;

public enum Team
{
    Enemy,
    Hero,
}
public enum PieceColor
{
    White,
    Black,
    None
}

public enum PieceType
{
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King,
    None,
    Jester,
}
public abstract class Chessman : MonoBehaviour
{
    private static int nextId = 0; // Auto-incrementing counter

    
    public int uniqueId;
    public int xBoard = -1;
    public int yBoard = -1;
    public Player owner;

    public BoardPosition startingPosition;
    public MovementProfile moveProfile;
    protected Sprite sprite;

    public GameObject droppingSprite;
    public GameObject attackingSprite;

    public int attack = 1;
    public int defense = 1;
    public int support = 1;
    public int diplomacy = 1;

    public int captures = 0;
    public int captured = 0;
    public int bounced = 0;
    public int bouncing = 0;
    public int supportsAttacking = 0;
    public int supportsDefending = 0;


    public int attackBonus = 0;
    public int defenseBonus = 0;
    public int supportBonus = 0;
    public int releaseCost = 1;
    public int blood = 1;
    public string info = "";
    public PieceColor color;
    public Team team;
    public PieceType type;
    public List<Ability> abilities;
    public bool isValidForAttack =false;
    public bool paralyzed=false;
    public bool hexed=false;
    public bool wasHexed=false;
    public bool canStationarySlash =false;

    public MMF_Player supportFloatingText;

    //public AbilityManager abilityManager; 


    public List<BoardPosition> validMoves = new List<BoardPosition>();

    //Variable for keeping track of the player it belongs to "black" or "white"
    private string player;

    //References to all the possible Sprites that this Chesspiece could be
    public Sprite blackSprite;
    public Sprite whiteSprite;

    public MMF_Player effectsFeedback;
    public ParticleSystem flames;
    public ParticleSystem highlightedParticles;
    public ParticleSystem hexedParticles;

    public abstract List<BoardPosition> GetValidMoves();
    public abstract List<BoardPosition> GetValidSupportMoves();
    public event Action<bool> OnChessmanStateChanged;

    protected virtual void Awake()
    {
        uniqueId = nextId++; // Assign unique ID and increment counter
    }
    public int CalculateSupport(){
        return support+supportBonus;
    }
    public int CalculateAttack(){
        return attack+attackBonus;
    }
    public int CalculateDefense(){
        return defense+defenseBonus;
    }
    public void SetValidMoves(){
        validMoves=GetValidMoves();
    }
    public void SetUniqueId(int id) // Allow manual ID assignment in specific cases
    {
        uniqueId = id;
    }

    void OnEnable()
    {
        OnChessmanStateChanged?.Invoke(true); // Notify that the object is enabled
    }

    void OnDisable()
    {
        OnChessmanStateChanged?.Invoke(false); // Notify that the object is disabled
    }
    public void Activate()
    {
        //Take the instantiated location and adjust transform
        UpdateUIPosition();
        switch (this.color)
        {
            case PieceColor.White: this.GetComponent<SpriteRenderer>().sprite = whiteSprite; player = "white"; break;
            case PieceColor.Black: this.GetComponent<SpriteRenderer>().sprite = blackSprite; player = "black"; break;
        }
        
        
    }
    public void ResetBonuses(){
        this.attackBonus=0;
        this.defenseBonus=0;
        this.supportBonus=0;
    }

    public void UpdateUIPosition()
    {
        //Get the board value in order to convert to xy coords
        float x = xBoard;
        float y = yBoard;

        //Adjust by variable offset
        x *= .96f;
        y *= .96f;

        //Add constants (pos 0,0)
        x += -3.33f;
        y += -3.33f;

        //Debug.Log("positions: "+x+","+y);
        //Set actual unity values
         if(this.transform.position == new Vector3(x, y, -1.0f))
            this.GetComponent<MMSpringPosition>().BumpRandom();
        else
            this.GetComponent<MMSpringPosition>().MoveTo(new Vector3(x, y, -1.0f));
        //this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetXBoard()
    {
        return xBoard;
    }

    public int GetYBoard()
    {
        return yBoard;
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    public void AddAbility(Ability ability)
    {
        Betrayer betrayerAbility = abilities.OfType<Betrayer>().FirstOrDefault();
        bool hadBetrayer = betrayerAbility != null;
        if (hadBetrayer)
        {
            betrayerAbility.Remove(this);
            abilities.Remove(betrayerAbility);
        }
        ability.Apply(this);

        if (hadBetrayer)
        {
            betrayerAbility.Apply(this);
        }
    }

    private void OnMouseDown()
    {
        if (GameManager._instance.isInMenu || GameManager._instance.applyingAbility)
        {
            return;
        }
        if (GameManager._instance.currentMatch !=null  && GameManager._instance.currentMatch.isSetUpPhase && GameManager._instance.hero.inventoryPieces.Contains(this.gameObject))
        {
            HandlePiecePlacement();
            return;
        }
        switch (GameManager._instance.state)
        {
            case ScreenState.RewardScreen:
                HandleRewardScreenClick();
                break;
            case ScreenState.PrisonersMarket:
                HandlePrisonersMarketClick();
                break;
            case ScreenState.ShopScreen:
                HandleShopClick();
                break;
            case ScreenState.ManagementScreen:
                HandleManagementClick();
                break;
            default: break;
        }

    }

    public void HandleRewardScreenClick(){        
        GameManager._instance.PieceSelected(this);
    }
    public void HandleShopClick(){   
        if(this.owner==null) 
            ManagementStatManager._instance.SetAndShowStats(this);     
        else 
            if (GameManager._instance.selectedCard==null)
                ManagementStatManager._instance.SetAndShowStats(this); 
            else
                GameManager._instance.PieceSelected(this);
    }
    public void HandleManagementClick(){        
        ArmyManager._instance.PieceSelect(this);
    }

    public void HandlePrisonersMarketClick(){   
        MarketManager._instance.AddPiece(this);
    }

    public void HandlePiecePlacement(){        
        Board._instance.SelectPieceToPlace(this);
    }

    public List<BoardPosition> DisplayValidMoves(){
        List<BoardPosition> theseValidMoves=new List<BoardPosition>();

        foreach (var coordinate in validMoves)
        {
            if (board.IsPositionOnBoard(coordinate.x, coordinate.y))
            {
                SetTileValidMove(coordinate.x, coordinate.y);
                theseValidMoves.Add(new BoardPosition(coordinate.x, coordinate.y));
            }
        }
        return theseValidMoves;
    }

    public List<BoardPosition>GetAllValidMoves(){
        List<BoardPosition> theseValidMoves=new List<BoardPosition>();

        foreach (var coordinate in validMoves)
        {
            if (board.IsPositionOnBoard(coordinate.x, coordinate.y))
            {
                GameObject cp = GameManager._instance.currentMatch.GetPieceAtPosition(coordinate.x, coordinate.y);
                if (cp == null)
                {
                    theseValidMoves.Add(new BoardPosition(coordinate.x, coordinate.y));
                }
                else if (cp.GetComponent<Chessman>().player != player)
                {
                    theseValidMoves.Add(new BoardPosition(coordinate.x, coordinate.y));
                }
            }
        }
        return theseValidMoves;
    }
     public void PointMovePlate(int x, int y)
    {
        if (board.IsPositionOnBoard(x, y))
        {
            GameObject cp = GameManager._instance.currentMatch.GetPieceAtPosition(x, y);

            if (cp == null)
            {
                SetTileValidMove(x, y);
            }
            else if (cp.GetComponent<Chessman>().player != player)
            {
                SetTileValidMove(x, y);
            }
        }
    } 

    public void showSupportFloatingText(){
        supportFloatingText.PlayFeedbacks();
    }

     public void SetTileValidMove(int x, int y)
    {
        Board._instance.SetActiveTile(this, new BoardPosition(x,y));
    } 

    private void OnMouseEnter(){
        if (GameManager._instance.isInMenu)
        {
            return;
        }
        if(GameManager._instance.state==ScreenState.PrisonersMarket)
            PopUpManager._instance.SetAndShowValues(this);
        StatBoxManager._instance.SetAndShowStats(this);
    } 

    private void OnMouseExit(){
        if(GameManager._instance.state==ScreenState.PrisonersMarket)
            PopUpManager._instance.HideValues();
    }

    public override bool Equals(object obj)
    {
        if (obj is Chessman other)
        {
            return uniqueId == other.uniqueId;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return uniqueId.GetHashCode();
    }

    public void LevelUp(int level){
        for (int i =0; i<level; i++)
            switch (UnityEngine.Random.Range(0,3)){
                case 0:
                    defense+=1;
                    break;
                case 1:
                    attack+=1;
                    break;
                case 2:
                    support+=1;
                    break;
            }
            
    }

    public void DestroyPiece()
    {
        owner.openPositions.Add(this.startingPosition);
        owner.pieces.Remove(this.gameObject);
        foreach(var ability in abilities)
            ability.Remove(this);
        Destroy(this.gameObject);
    }
}