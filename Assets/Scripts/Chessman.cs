using System;
using System.Collections;
using System.Collections.Generic;
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
}

public enum PieceType
{
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King,


}
public abstract class Chessman : MonoBehaviour
{
    //References to objects in our Unity Scene
    public GameObject controller;
    public GameObject movePlate;

    //Position for this Chesspiece on the Board
    //The correct position will be set later
    public int xBoard = -1;
    public int yBoard = -1;
    public MovementProfile moveProfile;
    protected Sprite sprite;

    public int attack = 1;
    public int defense = 1;
    public int support = 1;
    public string info = "";
    public PieceColor color;
    public Team team;
    public PieceType type;
    public List<Ability> abilities;
    //public AbilityManager abilityManager; 


    List<BoardPosition> validMoves = new List<BoardPosition>();

    //Variable for keeping track of the player it belongs to "black" or "white"
    private string player;

    //References to all the possible Sprites that this Chesspiece could be
    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

    public abstract List<BoardPosition> GetValidMoves();
    public abstract List<BoardPosition> GetValidSupportMoves();
    public int CalculateSupport(){
        return support;
    }
    public int CalculateAttack(){
        return attack;
    }
    public int CalculateDefense(){
        return defense;
    }
    public void Activate()
    {
        //Get the game controller
        controller = GameObject.FindGameObjectWithTag("GameController");

        //Take the instantiated location and adjust transform
        SetCoords();

        //Choose correct sprite based on piece's name
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

    public void SetCoords()
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
        if(controller.GetComponent<Game>().isInInventory)
            controller.GetComponent<Game>().PieceSelected(this);
        else{    
            Debug.Log(this.name+ " piece clicked");
            if (!controller.GetComponent<Game>().IsGameOver() && controller.GetComponent<Game>().GetCurrentPlayer() == color)
            {
                //Remove all moveplates relating to previously selected piece
                DestroyMovePlates();
                validMoves.Clear();

                //Create new MovePlates
                validMoves=GetValidMoves();
                DisplayValidMoves();
            }
        }
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

    public void DisplayValidMoves(){
        Game sc = controller.GetComponent<Game>();
        

        foreach (var coordinate in validMoves)
        {
            if (sc.PositionOnBoard(coordinate.x, coordinate.y))
            {
                GameObject cp = sc.GetPosition(coordinate.x, coordinate.y);
                if (cp == null)
                {
                    MovePlateSpawn(coordinate.x, coordinate.y);
                }
                else if (cp.GetComponent<Chessman>().player != player)
                {
                    MovePlateAttackSpawn(coordinate.x, coordinate.y);
                }
            }
        }
    }

     public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

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

    private void OnMouseEnter(){
        var sprite = this.GetComponent<SpriteRenderer>().sprite;
        if(team==Team.Hero)
            StatBoxManager._instance.SetAndShowStats(attack.ToString(),defense.ToString(),support.ToString(),info,name, sprite);
        else if(team == Team.Enemy)
            EnemyStatBoxManager._instance.SetAndShowStats(attack.ToString(),defense.ToString(),support.ToString(),info,name, sprite);
    }

    private void OnMouseExit(){
        //StatBoxManager._instance.HideStats();
    }

}