using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Movement
{
    public static List<BoardPosition> ValidPawnMoves(Board board, Chessman piece, int x, int y)
    {
        var validMoves = new List<BoardPosition>();
        if (BoardPosition.IsPositionOnBoard(x, y))
        {
            
            if (board.CurrentMatch.GetPieceAtPosition(x, y) == null)
            {
                validMoves.Add(new BoardPosition(x,y));
            }

            if (BoardPosition.IsPositionOnBoard(x + 1, y) && board.CurrentMatch.GetPieceAtPosition(x + 1, y) != null )
            {
                validMoves.Add(new BoardPosition(x+1,y));
            }

            if (BoardPosition.IsPositionOnBoard(x - 1, y) && board.CurrentMatch.GetPieceAtPosition(x - 1, y) != null)
            {
                validMoves.Add(new BoardPosition(x-1,y));
            }
        }
        return validMoves;
    }

    public static List<BoardPosition> ValidPawnSupportMoves(Board board, Chessman piece, int x, int y)
    {
        var validMoves = new List<BoardPosition>();
        if (BoardPosition.IsPositionOnBoard(x, y))
        {
            if (BoardPosition.IsPositionOnBoard(x + 1, y))
            {
                validMoves.Add(new BoardPosition(x+1,y));
            }

            if (BoardPosition.IsPositionOnBoard(x - 1, y))
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

    public static List<BoardPosition> ValidJesterMoves(Chessman piece, int xBoard, int yBoard)
    {
        var validMoves = new List<BoardPosition>
        {
            new BoardPosition(xBoard + 1, yBoard),
            new BoardPosition(xBoard - 1, yBoard),
            new BoardPosition(xBoard, yBoard + 1),
            new BoardPosition(xBoard, yBoard - 1),
            new BoardPosition(xBoard + 3, yBoard),
            new BoardPosition(xBoard - 3, yBoard),
            new BoardPosition(xBoard, yBoard + 3),
            new BoardPosition(xBoard, yBoard - 3),
            new BoardPosition(xBoard-2, yBoard +3),
            new BoardPosition(xBoard-2, yBoard - 3),
            new BoardPosition(xBoard-3, yBoard - 2),
            new BoardPosition(xBoard-3, yBoard +2),
            new BoardPosition(xBoard+2, yBoard +3),
            new BoardPosition(xBoard+2, yBoard - 3),
            new BoardPosition(xBoard+3, yBoard - 2),
            new BoardPosition(xBoard+3, yBoard +2),
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
        return BoardPosition.IsPositionOnBoard(x,y);
    }

    private static bool IsFriendlyPieceAtPosition(Board board, Chessman piece, int x, int y)
    {
        var otherPiece = board.CurrentMatch.GetPieceAtPosition(x, y); // Get the piece at the given position
        return otherPiece != null && otherPiece.GetComponent<Chessman>().color == piece.color;
    }

    public static List<BoardPosition> ValidRookMoves(Board board, Chessman piece, int xBoard, int yBoard){
        List<BoardPosition> thisValidMoves = new List<BoardPosition>();
        thisValidMoves.AddRange(LineMovePlate(board, piece, 1, 0, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,0, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,-1, 0, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,0, -1, xBoard, yBoard));
        return thisValidMoves;
    }

    public static List<BoardPosition> ValidBishopMoves(Board board, Chessman piece, int xBoard, int yBoard){

        List<BoardPosition> thisValidMoves = new List<BoardPosition>();
        thisValidMoves.AddRange(LineMovePlate(board, piece, 1, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,1, -1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,-1, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,-1, -1, xBoard, yBoard));
        return thisValidMoves;
    }

    public static List<BoardPosition> ValidQueenMoves(Board board, Chessman piece, int xBoard, int yBoard){
        List<BoardPosition> thisValidMoves = new List<BoardPosition>();
        thisValidMoves.AddRange(LineMovePlate(board, piece,1, 0, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,0, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,1, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,-1, 0, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,0, -1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,-1, -1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,-1, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,1, -1, xBoard, yBoard));
        return thisValidMoves;
    }

    public static List<BoardPosition> ValidScoutMoves(Board board, Chessman piece, int xBoard, int yBoard){
        List<BoardPosition> thisValidMoves = new List<BoardPosition>();
        thisValidMoves.AddRange(LineMovePlateNoCapture(board, piece,1, 0, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlateNoCapture(board, piece,0, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlateNoCapture(board, piece,1, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlateNoCapture(board, piece,-1, 0, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlateNoCapture(board, piece,0, -1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlateNoCapture(board, piece,-1, -1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlateNoCapture(board, piece,-1, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlateNoCapture(board, piece,1, -1, xBoard, yBoard));
        return thisValidMoves;
    }


    public static List<BoardPosition> AllOpenSquares(Board board){
        List<BoardPosition> thisValidMoves = new List<BoardPosition>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {   
                if(board.CurrentMatch.GetPieceAtPosition(i,j)==null)
                thisValidMoves.Add(new BoardPosition(i,j));
            }
        }
        return thisValidMoves;
    }


    public static List<BoardPosition> LineMovePlate(Board board, Chessman piece, int xIncrement, int yIncrement, int xBoard, int yBoard)
    {
        var validMoves = new List<BoardPosition>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;
        
        while (BoardPosition.IsPositionOnBoard(x, y) && board.CurrentMatch.GetPieceAtPosition(x, y) == null)
        {
            validMoves.Add(new BoardPosition(x,y));
            x += xIncrement;
            y += yIncrement;
        }
        if (BoardPosition.IsPositionOnBoard(x, y))
        {
            validMoves.Add(new BoardPosition(x,y));
        }

        return validMoves;
        
    } 

    public static List<BoardPosition> UnhinderedSelfLineMovePlate(Board board, Chessman piece, int xIncrement, int yIncrement, int xBoard, int yBoard)
    {
        var validMoves = new List<BoardPosition>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;
        
        while (BoardPosition.IsPositionOnBoard(x, y) && board.CurrentMatch.GetPieceAtPosition(x, y))
        {
            if(board.CurrentMatch.GetPieceAtPosition(x, y)==null)
                validMoves.Add(new BoardPosition(x,y));
            else if (BoardPosition.IsPositionOnBoard(x, y) && board.CurrentMatch.GetPieceAtPosition(x, y).GetComponent<Chessman>().color != piece.color)
            {
                validMoves.Add(new BoardPosition(x,y));
                break;
            }
            x += xIncrement;
            y += yIncrement;
        }
        

        return validMoves;
        
    } 
    public static List<BoardPosition> LineMovePlateNoCapture(Board board, Chessman piece, int xIncrement, int yIncrement, int xBoard, int yBoard)
    {
        var validMoves = new List<BoardPosition>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;
        
        while (BoardPosition.IsPositionOnBoard(x, y) && board.CurrentMatch.GetPieceAtPosition(x, y) == null)
        {
            validMoves.Add(new BoardPosition(x,y));
            x += xIncrement;
            y += yIncrement;
        }

        return validMoves;
        
    } 

    public static List<BoardPosition> RemoveFriendlyPieces(Board board, List<BoardPosition> validMoves, Chessman piece){
        return validMoves.Where(pos =>         // Check if within board boundaries
        !IsFriendlyPieceAtPosition(board, piece, pos.x, pos.y) // Check if not occupied by a friendly piece
        ).ToList();
    }
}
