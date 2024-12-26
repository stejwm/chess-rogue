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
            // Check if the pawn can move one space forward
            if (Game._instance.currentMatch.GetPieceAtPosition(x, y) == null)
            {
                validMoves.Add(new BoardPosition(x, y));

                // Check if the pawn can move two spaces forward (only if it hasn't moved yet)
                if (piece is Pawn pawn && !pawn.HasMovedBefore())
                {
                    int twoStepY = piece.color == PieceColor.White ? y + 1 : y - 1;
                    if (sc.PositionOnBoard(x, twoStepY) && Game._instance.currentMatch.GetPieceAtPosition(x, twoStepY) == null)
                    {
                        validMoves.Add(new BoardPosition(x, twoStepY));
                    }
                }
            }

            // Check for diagonal captures
            if (sc.PositionOnBoard(x + 1, y) && Game._instance.currentMatch.GetPieceAtPosition(x + 1, y) != null && Game._instance.currentMatch.GetPieceAtPosition(x + 1, y).GetComponent<Chessman>().color != piece.color)
            {
                validMoves.Add(new BoardPosition(x + 1, y));
            }

            if (sc.PositionOnBoard(x - 1, y) && Game._instance.currentMatch.GetPieceAtPosition(x - 1, y) != null && Game._instance.currentMatch.GetPieceAtPosition(x - 1, y).GetComponent<Chessman>().color != piece.color)
            {
                validMoves.Add(new BoardPosition(x - 1, y));
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

    private static bool CanCastle(Chessman piece, bool isKingside)
    {
        int direction = isKingside ? 1 : -1;
        int squares = isKingside ? 2 : 3;

        // Check if squares between king and rook are empty
        for (int i = 1; i <= squares; i++)
        {
            int x = piece.xBoard + (i * direction);
            if (Game._instance.currentMatch.GetPieceAtPosition(x, piece.yBoard) != null)
            {
                return false;
            }
        }
        return true;
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
        
        // Add castling moves
        if (!piece.hasMoved)
        {
            // Kingside castling
            var kingsideRook = Game._instance.currentMatch.GetPieceAtPosition(xBoard + 3, yBoard)?.GetComponent<Chessman>();
            if (kingsideRook != null && kingsideRook is Rook && !kingsideRook.hasMoved)
            {
                if (CanCastle(piece, true))
                {
                    validMoves.Add(new BoardPosition(xBoard + 2, yBoard));
                }
            }

            // Queenside castling
            var queensideRook = Game._instance.currentMatch.GetPieceAtPosition(xBoard - 4, yBoard)?.GetComponent<Chessman>();
            if (queensideRook != null && queensideRook is Rook && !queensideRook.hasMoved)
            {
                if (CanCastle(piece, false))
                {
                    validMoves.Add(new BoardPosition(xBoard - 2, yBoard));
                }
            }
        }

        validMoves = validMoves.Where(pos =>
            IsWithinBounds(sc, pos.x, pos.y) &&          
            !IsFriendlyPieceAtPosition(sc, piece, pos.x, pos.y)
        ).ToList();

        return validMoves;
    } 

    private static bool IsWithinBounds(Game sc, int x, int y)
    {
        return sc.PositionOnBoard(x,y);
    }

    private static bool IsFriendlyPieceAtPosition(Game sc, Chessman piece, int x, int y)
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
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game sc = controller.GetComponent<Game>();
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
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game sc = controller.GetComponent<Game>();
        var validMoves = new List<BoardPosition>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;
        
        while (sc.PositionOnBoard(x, y))
        {
            var pieceAtPosition = Game._instance.currentMatch.GetPieceAtPosition(x, y);
            if (pieceAtPosition == null)
            {
                validMoves.Add(new BoardPosition(x, y));
            }
            else if (pieceAtPosition.GetComponent<Chessman>().color != piece.color)
            {
                validMoves.Add(new BoardPosition(x, y));
                break;  // Stop at enemy piece
            }
            else
            {
                break;  // Stop at friendly piece
            }
            x += xIncrement;
            y += yIncrement;
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
        
        while (sc.PositionOnBoard(x, y))
        {
            var pieceAtPosition = Game._instance.currentMatch.GetPieceAtPosition(x, y);
            if (pieceAtPosition == null)
            {
                validMoves.Add(new BoardPosition(x, y));
            }
            else if (pieceAtPosition.GetComponent<Chessman>().color != piece.color)
            {
                validMoves.Add(new BoardPosition(x, y));
                break;
            }
            else
            {
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
        
        while (sc.PositionOnBoard(x, y) && Game._instance.currentMatch.GetPieceAtPosition(x, y) == null)
        {
            validMoves.Add(new BoardPosition(x,y));
            x += xIncrement;
            y += yIncrement;
        }

        return validMoves;
        
    } 
}

