using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Movement
{
    public static List<Tile> ValidPawnMoves(Board board, Chessman piece, int x, int y)
    {
        var validMoves = new List<Tile>();
        if (BoardPosition.IsPositionOnBoard(x, y))
        {
            
            if (board.GetPieceAtPosition(x, y) == null)
            {
                validMoves.Add(board.GetTileAt(x,y));
            }

            if (BoardPosition.IsPositionOnBoard(x + 1, y) && board.GetPieceAtPosition(x + 1, y) != null )
            {
                validMoves.Add(board.GetTileAt(x+1,y));
            }

            if (BoardPosition.IsPositionOnBoard(x - 1, y) && board.GetPieceAtPosition(x - 1, y) != null)
            {
                validMoves.Add(board.GetTileAt(x-1,y));
            }
        }
        return validMoves;
    }

    public static List<Tile> ValidPawnSupportMoves(Board board, Chessman piece, int x, int y)
    {
        var validMoves = new List<Tile>();
        if (BoardPosition.IsPositionOnBoard(x, y))
        {
            if (BoardPosition.IsPositionOnBoard(x + 1, y))
            {
                validMoves.Add(board.GetTileAt(x+1,y));
            }

            if (BoardPosition.IsPositionOnBoard(x - 1, y))
            {
                validMoves.Add(board.GetTileAt(x-1,y));
            }
        }
        return validMoves;
    } 

    public static List<Tile> ValidKnightMoves(Board board, Chessman piece, int xBoard, int yBoard)
    {
        var validMoves = new List<Tile>
        {
            board.GetTileAt(xBoard + 1, yBoard + 2),
            board.GetTileAt(xBoard - 1, yBoard + 2),
            board.GetTileAt(xBoard + 2, yBoard + 1),
            board.GetTileAt(xBoard + 2, yBoard - 1),
            board.GetTileAt(xBoard + 1, yBoard - 2),
            board.GetTileAt(xBoard - 1, yBoard - 2),
            board.GetTileAt(xBoard - 2, yBoard + 1),
            board.GetTileAt(xBoard - 2, yBoard - 1)
        };
        validMoves = validMoves.Where(pos =>
        pos != null
        ).ToList();
        return validMoves;
    }   

    public static List<Tile> ValidJesterMoves(Board board, Chessman piece, int xBoard, int yBoard)
    {
        var validMoves = new List<Tile>
        {
            board.GetTileAt(xBoard + 1, yBoard),
            board.GetTileAt(xBoard - 1, yBoard),
            board.GetTileAt(xBoard, yBoard + 1),
            board.GetTileAt(xBoard, yBoard - 1),
            board.GetTileAt(xBoard + 3, yBoard),
            board.GetTileAt(xBoard - 3, yBoard),
            board.GetTileAt(xBoard, yBoard + 3),
            board.GetTileAt(xBoard, yBoard - 3),
            board.GetTileAt(xBoard-2, yBoard +3),
            board.GetTileAt(xBoard-2, yBoard - 3),
            board.GetTileAt(xBoard-3, yBoard - 2),
            board.GetTileAt(xBoard-3, yBoard +2),
            board.GetTileAt(xBoard+2, yBoard +3),
            board.GetTileAt(xBoard+2, yBoard - 3),
            board.GetTileAt(xBoard+3, yBoard - 2),
            board.GetTileAt(xBoard+3, yBoard +2),
        };
        validMoves = validMoves.Where(pos =>
        pos != null
        ).ToList();
        return validMoves;
    }   

    public static List<Tile> ValidKingMoves(Board board, Chessman piece, int xBoard, int yBoard)
    {
        var validMoves = new List<Tile>
        {
            board.GetTileAt(xBoard+0, yBoard + 1),
            board.GetTileAt(xBoard+0, yBoard - 1),
            board.GetTileAt(xBoard - 1, yBoard + 0),
            board.GetTileAt(xBoard - 1, yBoard - 1),
            board.GetTileAt(xBoard - 1, yBoard + 1),
            board.GetTileAt(xBoard + 1, yBoard + 0),
            board.GetTileAt(xBoard + 1, yBoard - 1),
            board.GetTileAt(xBoard + 1, yBoard + 1)
        };
        
        validMoves = validMoves.Where(pos =>
        pos!=null
        ).ToList();

    return validMoves;
    }

    private static bool IsFriendlyPieceAtPosition(Board board, Chessman piece, int x, int y)
    {
        var otherPiece = board.GetPieceAtPosition(x, y); // Get the piece at the given position
        return otherPiece != null && otherPiece.GetComponent<Chessman>().color == piece.color;
    }

    public static List<Tile> ValidRookMoves(Board board, Chessman piece, int xBoard, int yBoard){
        List<Tile> thisValidMoves = new List<Tile>();
        thisValidMoves.AddRange(LineMovePlate(board, piece, 1, 0, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,0, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,-1, 0, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,0, -1, xBoard, yBoard));
        return thisValidMoves;
    }

    public static List<Tile> ValidBishopMoves(Board board, Chessman piece, int xBoard, int yBoard){

        List<Tile> thisValidMoves = new List<Tile>();
        thisValidMoves.AddRange(LineMovePlate(board, piece, 1, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,1, -1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,-1, 1, xBoard, yBoard));
        thisValidMoves.AddRange(LineMovePlate(board, piece,-1, -1, xBoard, yBoard));
        return thisValidMoves;
    }

    public static List<Tile> ValidQueenMoves(Board board, Chessman piece, int xBoard, int yBoard){
        List<Tile> thisValidMoves = new List<Tile>();
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

    public static List<Tile> ValidScoutMoves(Board board, Chessman piece, int xBoard, int yBoard){
        List<Tile> thisValidMoves = new List<Tile>();
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


    public static List<Tile> AllOpenSquares(Board board){
        List<Tile> thisValidMoves = new List<Tile>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {   
                if(board.GetPieceAtPosition(i,j)==null)
                thisValidMoves.Add(board.GetTileAt(i, j));
            }
        }
        return thisValidMoves;
    }


    public static List<Tile> LineMovePlate(Board board, Chessman piece, int xIncrement, int yIncrement, int xBoard, int yBoard)
    {
        var validMoves = new List<Tile>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;
        
        while (BoardPosition.IsPositionOnBoard(x, y) && board.GetPieceAtPosition(x, y) == null)
        {
            validMoves.Add(board.GetTileAt(x, y));
            x += xIncrement;
            y += yIncrement;
        }
        if (BoardPosition.IsPositionOnBoard(x, y))
        {
            validMoves.Add(board.GetTileAt(x, y));
        }

        return validMoves;
        
    } 

    public static List<Tile> UnhinderedSelfLineMovePlate(Board board, Chessman piece, int xIncrement, int yIncrement, int xBoard, int yBoard)
    {
        var validMoves = new List<Tile>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;
        
        while (BoardPosition.IsPositionOnBoard(x, y) && board.GetPieceAtPosition(x, y))
        {
            if(board.GetPieceAtPosition(x, y)==null)
                validMoves.Add(board.GetTileAt(x, y));
            else if (BoardPosition.IsPositionOnBoard(x, y) && board.GetPieceAtPosition(x, y).GetComponent<Chessman>().color != piece.color)
            {
                validMoves.Add(board.GetTileAt(x, y));
                break;
            }
            x += xIncrement;
            y += yIncrement;
        }
        

        return validMoves;
        
    } 
    public static List<Tile> LineMovePlateNoCapture(Board board, Chessman piece, int xIncrement, int yIncrement, int xBoard, int yBoard)
    {
        var validMoves = new List<Tile>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;
        
        while (BoardPosition.IsPositionOnBoard(x, y) && board.GetPieceAtPosition(x, y) == null)
        {
            validMoves.Add(board.GetTileAt(x, y));
            x += xIncrement;
            y += yIncrement;
        }

        return validMoves;
        
    } 

    public static List<Tile> RemoveFriendlyPieces(Board board, List<Tile> validMoves, Chessman piece){
        return validMoves.Where(pos =>         // Check if within board boundaries
        !IsFriendlyPieceAtPosition(board, piece, pos.X, pos.Y) // Check if not occupied by a friendly piece
        ).ToList();
    }
}
