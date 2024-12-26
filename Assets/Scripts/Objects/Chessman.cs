using System;
using System.Collections;
using System.Collections.Generic;
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

    public bool hasMoved = false;

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
    public float cellSize =1f; //.95f

    //public AbilityManager abilityManager; 


    public List<BoardPosition> validMoves = new List<BoardPosition>();

    //Variable for keeping track of the player it belongs to "black" or "white"
    private string player;

    //References to all the possible Sprites that this Chesspiece could be
    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

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

        //Choose correct sprite based on piece's name
        if (this.name.Contains("black_pawn"))
        {
            this.GetComponent<SpriteRenderer>().sprite = black_pawn;
            player = "black";
        }
        else if (this.name.Contains("white_pawn"))
        {
            this.GetComponent<SpriteRenderer>().sprite = white_pawn;
            player = "white";
        }
        else
        switch (this.name)
        {
            case "black_queen": this.GetComponent<SpriteRenderer>().sprite = black_queen; player = "black"; break;
            case "black_knight": this.GetComponent<SpriteRenderer>().sprite = black_knight; player = "black"; break;
            case "black_bishop": this.GetComponent<SpriteRenderer>().sprite = black_bishop; player = "black"; break;
            case "black_king": this.GetComponent<SpriteRenderer>().sprite = black_king; player = "black"; break;
            case "black_rook": this.GetComponent<SpriteRenderer>().sprite = black_rook; player = "black"; break;
            case "black_pawn": this.GetComponent<SpriteRenderer>().sprite = black_pawn; player = "black"; break;
            case "white_queen": this.GetComponent<SpriteRenderer>().sprite = white_queen; player = "white"; break;
            case "white_knight": this.GetComponent<SpriteRenderer>().sprite = white_knight; player = "white"; break;
            case "white_bishop": this.GetComponent<SpriteRenderer>().sprite = white_bishop; player = "white"; break;
            case "white_king": this.GetComponent<SpriteRenderer>().sprite = white_king; player = "white"; break;
            case "white_rook": this.GetComponent<SpriteRenderer>().sprite = white_rook; player = "white"; break;
            case "white_pawn": this.GetComponent<SpriteRenderer>().sprite = white_pawn; player = "white"; break;
        }
        
        
    }
    public virtual void ResetBonuses()
    {
        attackBonus = 0;
        defenseBonus = 0;
        supportBonus = 0;
        hasMoved = false;
    }

    public void UpdateUIPosition()
    {
        //Get the board value in order to convert to xy coords
        float x = xBoard;
        float y = yBoard;

        //Adjust by variable offset
        x *= .95f;
        y *= .95f;

        //Add constants (pos 0,0)
        x += -3.33f;
        y += -3.33f;

        //Debug.Log("positions: "+x+","+y);
        //Set actual unity values
        this.transform.position = new Vector3(x, y, -1.0f);
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
        switch (Game._instance.state)
        {
            case ScreenState.ActiveMatch:
                HandleMainGameboardClick();
                break;

            case ScreenState.RewardScreen:
                HandleRewardScreenClick();
                break;

            case ScreenState.PrisonersMarket:
                HandlePrisonersMarketClick();
                break;
            case ScreenState.ShopScreen:
                HandleShopClick();
                break;
        }
    } 
    public void HandleMainGameboardClick(){
        Debug.Log(this.name+ " piece clicked");
            if (isValidForAttack)
            {
                //Remove all moveplates relating to previously selected piece
                DestroyMovePlates();
                validMoves.Clear();

                //Create new MovePlates
                validMoves=GetValidMoves();
                DisplayValidMoves();
            }
    }

    public void HandleRewardScreenClick(){        
        Game._instance.PieceSelected(this);
    }
    public void HandleShopClick(){        
        ShopStatManager._instance.SetAndShowStats(this);
    }

    public void HandlePrisonersMarketClick(){        
        MarketManager._instance.AddPiece(this);
    }

    public void DestroyMovePlates()
    {
        //Destroy old MovePlates
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]); //Be careful with this function "Destroy" it is asynchronous
        }
    }

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
                GameObject cp = Game._instance.currentMatch.GetPieceAtPosition(coordinate.x, coordinate.y);
                if (cp == null)
                {
                    MovePlateSpawn(coordinate.x, coordinate.y);
                    theseValidMoves.Add(new BoardPosition(coordinate.x, coordinate.y));
                }
                else if (cp.GetComponent<Chessman>().player != player)
                {
                    MovePlateAttackSpawn(coordinate.x, coordinate.y);
                    theseValidMoves.Add(new BoardPosition(coordinate.x, coordinate.y));
                }
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
                MovePlateSpawn(x, y);
            }
            else if (cp.GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }
    } 

     public void MovePlateSpawn(int matrixX, int matrixY)
    {
        //Get the board value in order to convert to xy coords
        float x = matrixX;
        float y = matrixY;

        //Adjust by variable offset
        x *= .95f;
        y *= .95f;

        //Add constants (pos 0,0)
        x += -3.33f;
        y += -3.33f;

        //Set actual unity values
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    } 

     public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        //Get the board value in order to convert to xy coords
        float x = matrixX;
        float y = matrixY;

        //Adjust by variable offset
        x *= .95f;
        y *= .95f;

        //Add constants (pos 0,0)
        x += -3.33f;
        y += -3.33f;

        //Set actual unity values
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    } 

    private void OnMouseEnter()
    {
        // First check if Game._instance exists
        if (Game._instance == null || Game._instance.isInMenu)
        {
            return;
        }

        // Only proceed if we're in an active match
        if (Game._instance.state == ScreenState.ActiveMatch)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // Perform operations with spriteRenderer
            }
        }
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

        return this.name ==item.name && this.xBoard == item.xBoard && this.yBoard==item.yBoard;
    }
}