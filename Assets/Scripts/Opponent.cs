using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    public PieceColor color;
    public ArrayList pieces;
    private GameObject controller;
    private List<Chessman> piecesThatCanMove = new List<Chessman>();
    public Game game;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Initialize(){
        controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();
    }
    public void Move(){
        piecesThatCanMove.Clear();
        if(color == PieceColor.White)
            pieces=game.playerWhite;
        else
            pieces=game.playerBlack;

        foreach (GameObject item in pieces)
        {
            Chessman piece = item.GetComponent<Chessman>();
            var pieceMoves = piece.GetValidMoves();
            piece.validMoves = pieceMoves;
            if (piece.DisplayValidMoves().Count>0)
                piecesThatCanMove.Add(piece);
        }
        int randomPieceIndex = Random.Range(0, piecesThatCanMove.Count);
        Chessman movingPiece = piecesThatCanMove[randomPieceIndex];
        var validMoves=movingPiece.DisplayValidMoves();
        int randomMoveIndex = Random.Range(0,validMoves.Count);
        BoardPosition move = validMoves[randomMoveIndex];
        Debug.Log("Moving piece "+ movingPiece.name + " to "+move.x+","+move.y);
        game.ExecuteTurn(movingPiece,move.x, move.y);
    }
}
