using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    //Reference from Unity IDE
    public GameObject chesspiece;

    //Matrices needed, positions of each of the GameObjects
    //Also separate arrays for the players in order to easily keep track of them all
    //Keep in mind that the same objects are going to be in "positions" and "playerBlack"/"playerWhite"
    private GameObject[,] positions = new GameObject[8, 8];
    public ArrayList playerBlack;
    public ArrayList playerWhite;
    public PieceColor heroColor;

    //current turn
    private string currentPlayer = "white";

    //Game Ending
    private bool gameOver = false;

    //Unity calls this right when the game starts, there are a few built in functions
    //that Unity can call for you
    public void Start()
    {
        playerWhite = new ArrayList(new GameObject[] { Create("white_rook", 0, 0,PieceColor.White, Type.Hero), Create("white_knight", 1, 0,PieceColor.White, Type.Hero),
            Create("white_bishop", 2, 0,PieceColor.White, Type.Hero), Create("white_queen", 3, 0,PieceColor.White, Type.Hero), Create("white_king", 4, 0,PieceColor.White, Type.Hero),
            Create("white_bishop", 5, 0,PieceColor.White, Type.Hero), Create("white_knight", 6, 0,PieceColor.White, Type.Hero), Create("white_rook", 7, 0,PieceColor.White, Type.Hero),
            Create("white_pawn", 0, 1,PieceColor.White, Type.Hero), Create("white_pawn", 1, 1,PieceColor.White, Type.Hero), Create("white_pawn", 2, 1,PieceColor.White, Type.Hero),
            Create("white_pawn", 3, 1,PieceColor.White, Type.Hero), Create("white_pawn", 4, 1,PieceColor.White, Type.Hero), Create("white_pawn", 5, 1,PieceColor.White, Type.Hero),
            Create("white_pawn", 6, 1,PieceColor.White, Type.Hero), Create("white_pawn", 7, 1,PieceColor.White, Type.Hero) });
        playerBlack = new ArrayList(new GameObject[] { Create("black_rook", 0, 7,PieceColor.Black, Type.Enemy), Create("black_knight",1,7,PieceColor.Black, Type.Enemy),
            Create("black_bishop",2,7,PieceColor.Black, Type.Enemy), Create("black_queen",3,7,PieceColor.Black, Type.Enemy), Create("black_king",4,7,PieceColor.Black, Type.Enemy),
            Create("black_bishop",5,7,PieceColor.Black, Type.Enemy), Create("black_knight",6,7,PieceColor.Black, Type.Enemy), Create("black_rook",7,7,PieceColor.Black, Type.Enemy),
            Create("black_pawn", 0, 6,PieceColor.Black, Type.Enemy), Create("black_pawn", 1, 6,PieceColor.Black, Type.Enemy), Create("black_pawn", 2, 6,PieceColor.Black, Type.Enemy),
            Create("black_pawn", 3, 6,PieceColor.Black, Type.Enemy), Create("black_pawn", 4, 6,PieceColor.Black, Type.Enemy), Create("black_pawn", 5, 6,PieceColor.Black, Type.Enemy),
            Create("black_pawn", 6, 6,PieceColor.Black, Type.Enemy), Create("black_pawn", 7, 6,PieceColor.Black, Type.Enemy) });

            heroColor=PieceColor.White;
        //Set all piece positions on the positions board
        for (int i = 0; i < playerBlack.Count; i++)
        {
            SetPosition((GameObject)playerBlack[i]);
            SetPosition((GameObject)playerWhite[i]);
        }
    }

    public GameObject Create(string name, int x, int y, PieceColor color, Type type)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>(); //We have access to the GameObject, we need the script
        cm.color=color;
        cm.type=type;
        cm.name = name; //This is a built in variable that Unity has, so we did not have to declare it before
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate(); //It has everything set up so it can now Activate()
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();

        //Overwrites either empty space or whatever was there
        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void NextTurn()
    {
        if (currentPlayer == "white")
        {
            currentPlayer = "black";
        }
        else
        {
            currentPlayer = "white";
        }
    }

    public void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;

            //Using UnityEngine.SceneManagement is needed here
            SceneManager.LoadScene("Game"); //Restarts the game by loading the scene over again
        }
    }
    
    public void Winner(string playerWinner)
    {
        gameOver = true;

        //Using UnityEngine.UI is needed here
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = playerWinner + " is the winner";

        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
    }
}
