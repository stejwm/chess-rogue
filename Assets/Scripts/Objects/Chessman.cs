using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;

public enum Team
{
    Enemy,
    Hero,
}
public enum Gender
{
    Male,
    Female,
}
public enum StatType
{
    Attack,
    Defense,
    Support
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
public abstract class Chessman : MonoBehaviour, IInteractable
{
    private static int nextId = 0; // Auto-incrementing counter


    public int uniqueId;
    public int xBoard = -1;
    public int yBoard = -1;
    public Player owner;

    public Tile startingPosition;
    public MovementProfile moveProfile;
    protected Sprite sprite;

    public GameObject droppingSprite;
    public GameObject attackingSprite;

    public int attack = 1;
    public int defense = 1;
    public int support = 1;
    public int diplomacy = 1;
    public Gender gender = Gender.Male;
    public int age = 25;
    public int weight = 145;
    public int height = 184;

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
    public bool isValidForAttack = false;
    public bool paralyzed = false;
    public bool hexed = false;
    public bool wasHexed = false;
    public bool canStationarySlash = false;

    public MMF_Player supportFloatingText;

    private Dictionary<string, int> attackBonuses = new Dictionary<string, int>();
    private Dictionary<string, int> defenseBonuses = new Dictionary<string, int>();
    private Dictionary<string, int> supportBonuses = new Dictionary<string, int>();

    public List<Tile> validMoves = new List<Tile>();
    public Sprite blackSprite;
    public Sprite whiteSprite;
    public Sprite isometricSprite;

    public MMF_Player effectsFeedback;
    public ParticleSystem flames;
    public ParticleSystem highlightedParticles;
    public ParticleSystem hexedParticles;

    public abstract List<Tile> GetValidMoves();
    public abstract List<Tile> GetValidSupportMoves();
    public abstract void Initialize(Board board);
    public event Action<bool> OnChessmanStateChanged;

    protected virtual void Awake()
    {
        uniqueId = nextId++; // Assign unique ID and increment counter
    }
    public int CalculateSupport()
    {
        return support + supportBonus;
    }
    public int CalculateAttack()
    {
        return attack + attackBonus;
    }
    public int CalculateDefense()
    {
        return defense + defenseBonus;
    }
    public void SetValidMoves()
    {
        validMoves = GetValidMoves();
    }

    public void AddBonus(StatType stat, int value, string source)
    {
        switch (stat)
        {
            case StatType.Attack:
                attackBonuses[source] = attackBonuses.GetValueOrDefault(source) + value;
                attackBonus += value;
                break;
            case StatType.Defense:
                defenseBonuses[source] = defenseBonuses.GetValueOrDefault(source) + value;
                defenseBonus += value;
                break;
            case StatType.Support:
                supportBonuses[source] = supportBonuses.GetValueOrDefault(source) + value;
                supportBonus += value;
                break;
        }
    }
    public void RemoveBonus(StatType stat, int value, string source)
    {
        switch (stat)
        {
            case StatType.Attack:
                attackBonuses[source] = attackBonuses.GetValueOrDefault(source) - value;
                attackBonus -= value;
                break;
            case StatType.Defense:
                defenseBonuses[source] = defenseBonuses.GetValueOrDefault(source) - value;
                defenseBonus -= value;
                break;
            case StatType.Support:
                supportBonuses[source] = supportBonuses.GetValueOrDefault(source) - value;
                supportBonus -= value;
                break;
        }
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
        Debug.Log("Activating Chessman: " + this.name);
        UpdateUIPosition();
        switch (this.color)
        {
            case PieceColor.White: this.GetComponent<SpriteRenderer>().sprite = whiteSprite; break;
            case PieceColor.Black: this.GetComponent<SpriteRenderer>().sprite = blackSprite; break;
        }


    }
    public void ResetBonuses()
    {
        this.attackBonus = 0;
        this.defenseBonus = 0;
        this.supportBonus = 0;
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
        if (this.transform.position == new Vector3(x, y, -1.0f))
            this.GetComponent<MMSpringPosition>().BumpRandom();
        else
            this.GetComponent<MMSpringPosition>().MoveTo(new Vector3(x, y, -1.0f));
        //this.transform.position = new Vector3(x, y, -1.0f);
    }

    public void AddAbility(Board board, Ability ability)
    {
        Betrayer betrayerAbility = abilities.OfType<Betrayer>().FirstOrDefault();
        bool hadBetrayer = betrayerAbility != null;
        if (hadBetrayer)
        {
            betrayerAbility.Remove(this);
            abilities.Remove(betrayerAbility);
        }
        ability.Apply(board, this);

        if (hadBetrayer)
        {
            betrayerAbility.Apply(board, this);
        }
    }
    /*
    private void OnMouseDown()
    {
        if (GameManager._instance.isInMenu || GameManager._instance.applyingAbility)
        {
            return;
        }
        if (board.CurrentMatch != null && board.CurrentMatch.isSetUpPhase && GameManager._instance.hero.inventoryPieces.Contains(this.gameObject))
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
        //.SelectPieceToPlace(this);
    }

    public List<Tile> DisplayValidMoves(){
        List<Tile> theseValidMoves=new List<Tile>();

        foreach (var coordinate in validMoves)
        {
            if (BoardPosition.IsPositionOnBoard(coordinate.x, coordinate.y))
            {
                SetTileValidMove(coordinate.x, coordinate.y);
                theseValidMoves.Add(new BoardPosition(coordinate.x, coordinate.y));
            }
        }
        return theseValidMoves;
    }

    public List<Tile>GetAllValidMoves(){
        List<Tile> theseValidMoves=new List<Tile>();

        foreach (var coordinate in validMoves)
        {
            if (BoardPosition.IsPositionOnBoard(coordinate.x, coordinate.y))
            {
                GameObject cp = board.CurrentMatch.GetPieceAtPosition(coordinate.x, coordinate.y);
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
    }*/

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

    public void LevelUp(int level)
    {
        for (int i = 0; i < level; i++)
            switch (UnityEngine.Random.Range(0, 3))
            {
                case 0:
                    defense += 1;
                    break;
                case 1:
                    attack += 1;
                    break;
                case 2:
                    support += 1;
                    break;
            }

    }

    public void DestroyPiece()
    {
        owner.openPositions.Add(this.startingPosition);
        owner.pieces.Remove(this.gameObject);
        foreach (var ability in abilities)
            ability.Remove(this);
        Destroy(this.gameObject);
    }

    public void OnClick(Board board)
    {
        switch (board.BoardState)
        {
            case BoardState.PrisonersMarket:
                break;
            case BoardState.ShopScreen:
                break;
            case BoardState.ManagementScreen:
                break;
            default:
                break;
        }
    }

    public void OnRightClick(Board board)
    {
        throw new NotImplementedException();
    }

    public void OnHover(Board board)
    {
        Debug.Log($"Nothing good here yet, or maybe at all?");
    }

    public void OnHoverExit(Board board)
    {
    }
}