using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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
    None
}
public abstract class Chessman : MonoBehaviour
{
    public GameObject movePlate;
    //public EventSystem eventSystem;

    //Position for this Chesspiece on the Board
    //The correct position will be set later
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

    public int attackBonus = 0;
    public int defenseBonus = 0;
    public int supportBonus = 0;
    public int releaseCost = 5;
    public int blood = 1;
    public string info = "";
    public PieceColor color;
    public Team team;
    public PieceType type;
    public List<Ability> abilities;
    public bool isValidForAttack =false;
    public Vector3 gridOrigin;

    public MMF_Player supportFloatingText;

    //public AbilityManager abilityManager; 


    public List<BoardPosition> validMoves = new List<BoardPosition>();

    //Variable for keeping track of the player it belongs to "black" or "white"
    private string player;

    //References to all the possible Sprites that this Chesspiece could be
    public Sprite blackSprite;
    public Sprite whiteSprite;

    public MMF_Player effectsFeedback;

    public abstract List<BoardPosition> GetValidMoves();
    public abstract List<BoardPosition> GetValidSupportMoves();
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
        abilities.Add(ability);
        ability.Apply(this);
        //abilityManager.AddAbility(ability, this);
    }

    private void OnMouseDown()
    {
        if (Game._instance.isInMenu)
        {
            return;
        }
        if (Game._instance.currentMatch !=null  && Game._instance.currentMatch.isSetUpPhase && Game._instance.hero.inventoryPieces.Contains(this.gameObject))
        {
            HandlePiecePlacement();
            return;
        }
        switch (Game._instance.state)
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
        Game._instance.PieceSelected(this);
    }
    public void HandleShopClick(){        
        Game._instance.PieceSelected(this);
    }
    public void HandleManagementClick(){        
        ManagementStatManager._instance.SetAndShowStats(this);
    }

    public void HandlePrisonersMarketClick(){        
        MarketManager._instance.AddPiece(this);
    }

    public void HandlePiecePlacement(){        
        BoardManager._instance.SelectPieceToPlace(this);
    }

    /* public void DestroyMovePlates()
    {
        //Destroy old MovePlates
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]); //Be careful with this function "Destroy" it is asynchronous
        }
    } */

/*      public List<Tuple<int,int>> GetValidMoves()
    {
        switch (this.name)
        {
            case "black_queen":
            case "white_queen":
                return ValidQueenMoves();
                
            case "black_knight":
            case "white_knight":
                return ValidKnightMoves();
                
            case "black_bishop":
            case "white_bishop":
                return ValidBishopMoves();
                
            case "black_king":
            case "white_king":
                return ValidKingMoves();
                
            case "black_rook":
            case "white_rook":
                return ValidRookMoves();
                
            case "black_pawn":
                return ValidPawnMoves(xBoard, yBoard - 1);
                
            case "white_pawn":
                return ValidPawnMoves(xBoard, yBoard + 1);
            default:
                return null;
            
        }
        
    }  */

/* public List<Tuple<int,int>> GetValidSupportMoves()
    {
        validMoves.Clear();
        switch (this.name)
        {
            case "black_queen":
            case "white_queen":
                return ValidQueenMoves();
                
            case "black_knight":
            case "white_knight":
                return ValidKnightMoves();
                
            case "black_bishop":
            case "white_bishop":
                return ValidBishopMoves();
                
            case "black_king":
            case "white_king":
                return ValidKingMoves();
                
            case "black_rook":
            case "white_rook":
                return ValidRookMoves();
                
            case "black_pawn":
                return ValidPawnSupportMoves(xBoard, yBoard - 1);
                
            case "white_pawn":
                return ValidPawnSupportMoves(xBoard, yBoard + 1);
            default:
                return null;
            
        }
        
    }  */

    public List<BoardPosition> DisplayValidMoves(){
        List<BoardPosition> theseValidMoves=new List<BoardPosition>();

        foreach (var coordinate in validMoves)
        {
            if (Game._instance.PositionOnBoard(coordinate.x, coordinate.y))
            {
                //GameObject cp = Game._instance.currentMatch.GetPieceAtPosition(coordinate.x, coordinate.y);
                //if (cp == null)
                //{
                    SetTileValidMove(coordinate.x, coordinate.y);
                    theseValidMoves.Add(new BoardPosition(coordinate.x, coordinate.y));
                //}
                 /* else if (cp.GetComponent<Chessman>().player != player)
                {
                    SetTileValidMove(coordinate.x, coordinate.y);
                    theseValidMoves.Add(new BoardPosition(coordinate.x, coordinate.y));
                }  */
            }
        }
        return theseValidMoves;
    }

    public List<BoardPosition>GetAllValidMoves(){
        List<BoardPosition> theseValidMoves=new List<BoardPosition>();

        foreach (var coordinate in validMoves)
        {
            if (Game._instance.PositionOnBoard(coordinate.x, coordinate.y))
            {
                GameObject cp = Game._instance.currentMatch.GetPieceAtPosition(coordinate.x, coordinate.y);
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
        if (Game._instance.PositionOnBoard(x, y))
        {
            GameObject cp = Game._instance.currentMatch.GetPieceAtPosition(x, y);

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
        BoardManager._instance.SetActiveTile(this, new BoardPosition(x,y));
    } 

     /* public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        //Get the board value in order to convert to xy coords
        float x = matrixX;
        float y = matrixY;

        //Adjust by variable offset
        x *= .96f;
        y *= .96f;

        //Add constants (pos 0,0)
        x += -3.33f;
        y += -3.33f;

        //Set actual unity values
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }  */

    private void OnMouseEnter(){
        RewardStatManager._instance.SetAndShowStats(this);
        ShopStatManager._instance.SetAndShowStats(this);
    } 

    private void OnMouseExit(){
        //StatBoxManager._instance.HideStats();
    }

    public override bool Equals(object obj)
    {
        var item = obj as Chessman;

        if (item == null)
        {
            return false;
        }

        return this.name ==item.name && this.startingPosition ==item.startingPosition;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.name, this.startingPosition);
    }
}