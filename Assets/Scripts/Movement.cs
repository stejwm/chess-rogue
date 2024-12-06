using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Movement
{
    public static GameObject controller;
    public static List<BoardPosition> ValidPawnMoves(Chessman piece, int x, int y)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        var validMoves = new List<BoardPosition>();
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            
            if (sc.GetPosition(x, y) == null)
            {
                validMoves.Add(new BoardPosition(x,y));
            }

            if (sc.PositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null && sc.GetPosition(x + 1, y).GetComponent<Chessman>().color != piece.color)
            {
                validMoves.Add(new BoardPosition(x+1,y));
            }

            if (sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null && sc.GetPosition(x - 1, y).GetComponent<Chessman>().color != piece.color)
            {
                validMoves.Add(new BoardPosition(x-1,y));
            }
        }
        return validMoves;
    }

    public static List<BoardPosition> ValidPawnSupportMoves(Chessman piece, int x, int y)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        var validMoves = new List<BoardPosition>();
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            if (sc.PositionOnBoard(x + 1, y))
            {
                validMoves.Add(new BoardPosition(x+1,y));
            }

            if (sc.PositionOnBoard(x - 1, y))
            {
                validMoves.Add(new BoardPosition(x-1,y));
            }
        }
        return validMoves;
    } 

    public static List<BoardPosition> ValidKnightMoves(Chessman piece, int xBoard, int yBoard)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game sc = controller.GetComponent<Game>();
        var validMoves = new List<BoardPosition>
        {
            new BoardPosition(xBoard + 1, yBoard + 2),
            new BoardPosition(xBoard - 1, yBoard + 2),
            new BoardPosition(xBoard + 2, yBoard + 1),
            new BoardPosition(xBoard + 2, yBoard - 1),
            new BoardPosition(xBoard + 1, yBoard - 2),
            new BoardPosition(xBoard - 1, yBoard - 2),
            new BoardPosition(xBoard - 2, yBoard + 1),
            new BoardPosition(xBoard - 2, yBoard - 1)
        };
        validMoves = validMoves.Where(pos =>
        IsWithinBounds(sc, pos.x, pos.y) &&          // Check if within board boundaries
        !IsFriendlyPieceAtPosition(sc, piece, pos.x, pos.y) // Check if not occupied by a friendly piece
        ).ToList();
        return validMoves;
    }   

    public static List<BoardPosition> ValidKingMoves(Chessman piece, int xBoard, int yBoard)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game sc = controller.GetComponent<Game>();
        var validMoves = new List<BoardPosition>
        {
            new BoardPosition(xBoard+0, yBoard + 1),
            new BoardPosition(xBoard+0, yBoard - 1),
            new BoardPosition(xBoard - 1, yBoard + 0),
            new BoardPosition(xBoard - 1, yBoard - 1),
            new BoardPosition(xBoard - 1, yBoard + 1),
            new BoardPosition(xBoard + 1, yBoard + 0),
            new BoardPosition(xBoard + 1, yBoard - 1),
            new BoardPosition(xBoard + 1, yBoard + 1)
        };
        
        validMoves = validMoves.Where(pos =>
        IsWithinBounds(sc, pos.x, pos.y) &&          // Check if within board boundaries
        !IsFriendlyPieceAtPosition(sc, piece, pos.x, pos.y) // Check if not occupied by a friendly piece
        ).ToList();

    return validMoves;
    } 

    private static bool IsWithinBounds(Game sc, int x, int y)
    {
        return sc.PositionOnBoard(x,y);
    }

    private static bool IsFriendlyPieceAtPosition(Game sc, Chessman piece, int x, int y)
    {
        var otherPiece = sc.GetPosition(x, y); // Get the piece at the given position
        return otherPiece != null && otherPiece.GetComponent<Chessman>().color == piece.color;
    }

    public static List<BoardPosition> ValidRookMoves(Chessman piece, int xBoard, int yBoard){
        List<BoardPosition> thisValidMoves = new List<BoardPosition>();
        thisValidMoves.AddRange(LineMovePlate(piece, 1, 0, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(piece,0, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(piece,-1, 0, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(piece,0, -1, xBoard, yBoard));
        return thisValidMoves;
    }

    public static List<BoardPosition> ValidBishopMoves(Chessman piece, int xBoard, int yBoard){

        List<BoardPosition> thisValidMoves = new List<BoardPosition>();
        thisValidMoves.AddRange(LineMovePlate(piece, 1, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(piece,1, -1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(piece,-1, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(piece,-1, -1, xBoard, yBoard));
        return thisValidMoves;
    }

    public static List<BoardPosition> ValidQueenMoves(Chessman piece, int xBoard, int yBoard){
        List<BoardPosition> thisValidMoves = new List<BoardPosition>();
        thisValidMoves.AddRange(LineMovePlate(piece,1, 0, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(piece,0, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(piece,1, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(piece,-1, 0, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(piece,0, -1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(piece,-1, -1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(piece,-1, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(piece,1, -1, xBoard, yBoard));
        return thisValidMoves;
    }

    public static List<BoardPosition> ValidScoutMoves(Chessman piece, int xBoard, int yBoard){
        List<BoardPosition> thisValidMoves = new List<BoardPosition>();
        thisValidMoves.AddRange(LineMovePlateNoCapture(piece,1, 0, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlateNoCapture(piece,0, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlateNoCapture(piece,1, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlateNoCapture(piece,-1, 0, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlateNoCapture(piece,0, -1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlateNoCapture(piece,-1, -1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlateNoCapture(piece,-1, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlateNoCapture(piece,1, -1, xBoard, yBoard));
        return thisValidMoves;
    }


    public static List<BoardPosition> LineMovePlate(Chessman piece, int xIncrement, int yIncrement, int xBoard, int yBoard)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game sc = controller.GetComponent<Game>();
        var validMoves = new List<BoardPosition>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;
        
        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            validMoves.Add(new BoardPosition(x,y));
            x += xIncrement;
            y += yIncrement;
        }
        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Chessman>().color != piece.color)
        {
            validMoves.Add(new BoardPosition(x,y));
        }

        return validMoves;
        
    } 

    public static List<BoardPosition> UnhinderedSelfLineMovePlate(Chessman piece, int xIncrement, int yIncrement, int xBoard, int yBoard)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game sc = controller.GetComponent<Game>();
        var validMoves = new List<BoardPosition>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;
        
        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y))
        {
            if(sc.GetPosition(x, y)==null)
                validMoves.Add(new BoardPosition(x,y));
            else if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Chessman>().color != piece.color)
            {
                validMoves.Add(new BoardPosition(x,y));
                break;
            }
            x += xIncrement;
            y += yIncrement;
        }
        

        return validMoves;
        
    } 
    public static List<BoardPosition> LineMovePlateNoCapture(Chessman piece, int xIncrement, int yIncrement, int xBoard, int yBoard)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game sc = controller.GetComponent<Game>();
        var validMoves = new List<BoardPosition>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;
        
        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            validMoves.Add(new BoardPosition(x,y));
            x += xIncrement;
            y += yIncrement;
        }

        return validMoves;
        
    } 
}
