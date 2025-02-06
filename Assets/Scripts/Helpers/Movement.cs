using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Movement
{
    public static List<BoardPosition> ValidPawnMoves(Chessman piece, int x, int y)
    {
        var validMoves = new List<BoardPosition>();
        if (Game._instance.PositionOnBoard(x, y))
        {
            
            if (Game._instance.currentMatch.GetPieceAtPosition(x, y) == null)
            {
                validMoves.Add(new BoardPosition(x,y));
            }

            if (Game._instance.PositionOnBoard(x + 1, y) && Game._instance.currentMatch.GetPieceAtPosition(x + 1, y) != null )
            {
                validMoves.Add(new BoardPosition(x+1,y));
            }

            if (Game._instance.PositionOnBoard(x - 1, y) && Game._instance.currentMatch.GetPieceAtPosition(x - 1, y) != null)
            {
                validMoves.Add(new BoardPosition(x-1,y));
            }
        }
        return validMoves;
    }

    public static List<BoardPosition> ValidPawnSupportMoves(Chessman piece, int x, int y)
    {
        var validMoves = new List<BoardPosition>();
        if (Game._instance.PositionOnBoard(x, y))
        {
            if (Game._instance.PositionOnBoard(x + 1, y))
            {
                validMoves.Add(new BoardPosition(x+1,y));
            }

            if (Game._instance.PositionOnBoard(x - 1, y))
            {
                validMoves.Add(new BoardPosition(x-1,y));
            }
        }
        return validMoves;
    } 

    public static List<BoardPosition> ValidKnightMoves(Chessman piece, int xBoard, int yBoard)
    {
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
        IsWithinBounds(pos.x, pos.y)
        ).ToList();
        return validMoves;
    }   

    public static List<BoardPosition> ValidKingMoves(Chessman piece, int xBoard, int yBoard)
    {
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
        IsWithinBounds(pos.x, pos.y)
        ).ToList();

    return validMoves;
    } 

    private static bool IsWithinBounds(int x, int y)
    {
        return Game._instance.PositionOnBoard(x,y);
    }

    private static bool IsFriendlyPieceAtPosition(Chessman piece, int x, int y)
    {
        var otherPiece = Game._instance.currentMatch.GetPieceAtPosition(x, y); // Get the piece at the given position
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


    public static List<BoardPosition> AllOpenSquares(){
        List<BoardPosition> thisValidMoves = new List<BoardPosition>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {   
                if(Game._instance.currentMatch.GetPieceAtPosition(i,j)==null)
                thisValidMoves.Add(new BoardPosition(i,j));
            }
        }
        return thisValidMoves;
    }


    public static List<BoardPosition> LineMovePlate(Chessman piece, int xIncrement, int yIncrement, int xBoard, int yBoard)
    {
        var validMoves = new List<BoardPosition>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;
        
        while (Game._instance.PositionOnBoard(x, y) && Game._instance.currentMatch.GetPieceAtPosition(x, y) == null)
        {
            validMoves.Add(new BoardPosition(x,y));
            x += xIncrement;
            y += yIncrement;
        }
        if (Game._instance.PositionOnBoard(x, y))
        {
            validMoves.Add(new BoardPosition(x,y));
        }

        return validMoves;
        
    } 

    public static List<BoardPosition> UnhinderedSelfLineMovePlate(Chessman piece, int xIncrement, int yIncrement, int xBoard, int yBoard)
    {
        var validMoves = new List<BoardPosition>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;
        
        while (Game._instance.PositionOnBoard(x, y) && Game._instance.currentMatch.GetPieceAtPosition(x, y))
        {
            if(Game._instance.currentMatch.GetPieceAtPosition(x, y)==null)
                validMoves.Add(new BoardPosition(x,y));
            else if (Game._instance.PositionOnBoard(x, y) && Game._instance.currentMatch.GetPieceAtPosition(x, y).GetComponent<Chessman>().color != piece.color)
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
        var validMoves = new List<BoardPosition>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;
        
        while (Game._instance.PositionOnBoard(x, y) && Game._instance.currentMatch.GetPieceAtPosition(x, y) == null)
        {
            validMoves.Add(new BoardPosition(x,y));
            x += xIncrement;
            y += yIncrement;
        }

        return validMoves;
        
    } 

    public static List<BoardPosition> RemoveFriendlyPieces(List<BoardPosition> validMoves, Chessman piece){
        return validMoves.Where(pos =>         // Check if within board boundaries
        !IsFriendlyPieceAtPosition(piece, pos.x, pos.y) // Check if not occupied by a friendly piece
        ).ToList();
    }
}
