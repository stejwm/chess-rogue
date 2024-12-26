using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ChessMatch
{
    //public List<GameObject> playerWhite;
    //public List<GameObject> playerBlack;
    public PieceColor currentPlayer;
    public Player white;
    public Player black;
    private GameObject[,] positions = new GameObject[8, 8];
    public bool AdamantAssaultOverride = false;
    public bool BloodThirstOverride = false;


    public ChessMatch(Player white, Player black)
    {
        this.white=white;
        this.black=black;
        //playerWhite = white.pieces;
        //playerBlack = black.pieces;
        UpdateBoard();
        SetWhiteTurn();
    }

    public ChessMatch(Player white)
    {
        this.white=white;
        //playerWhite = white.pieces;
    }

    public void ExecuteTurn(Chessman piece, int x, int y){
        Game._instance.isInMenu=true;
        MoveManager._instance.Set(this, piece,x,y);
        MoveManager._instance.HandleMove(piece,x,y);
    }
    public void UpdateBoard(){
        // First clear all positions
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                positions[x, y] = null;
            }
        }

        // Then update with current piece positions
        foreach (GameObject piece in white.pieces)
        {
            if (piece.activeSelf)
            {
                Chessman cm = piece.GetComponent<Chessman>();
                positions[cm.xBoard, cm.yBoard] = piece;
            }
        }
        foreach (GameObject piece in black.pieces)
        {
            if (piece.activeSelf)
            {
                Chessman cm = piece.GetComponent<Chessman>();
                positions[cm.xBoard, cm.yBoard] = piece;
            }
        }
    }

    public void ResetBoard()
    {
        // Clear the board first
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                positions[x, y] = null;
            }
        }

        // Reset and move white pieces
        foreach (GameObject piece in white.pieces.ToList())
        {
            piece.SetActive(true);
            var cm = piece.GetComponent<Chessman>();
            cm.ResetBonuses();
            cm.xBoard = cm.startingPosition.x;
            cm.yBoard = cm.startingPosition.y;
            positions[cm.xBoard, cm.yBoard] = piece;
            cm.UpdateUIPosition();
        }

        // Reset and move black pieces
        foreach (GameObject piece in black.pieces.ToList())
        {
            piece.SetActive(true);
            var cm = piece.GetComponent<Chessman>();
            cm.ResetBonuses();
            cm.xBoard = cm.startingPosition.x;
            cm.yBoard = cm.startingPosition.y;
            positions[cm.xBoard, cm.yBoard] = piece;
            cm.UpdateUIPosition();
        }

        // Handle captured pieces
        foreach (GameObject piece in white.capturedPieces.ToList())
        {
            piece.SetActive(true);
            var cm = piece.GetComponent<Chessman>();
            cm.ResetBonuses();
            cm.xBoard = cm.startingPosition.x;
            cm.yBoard = cm.startingPosition.y;
            positions[cm.xBoard, cm.yBoard] = piece;
            cm.UpdateUIPosition();
            white.pieces.Add(piece);
        }
        white.capturedPieces.Clear();

        foreach (GameObject piece in black.capturedPieces.ToList())
        {
            piece.SetActive(true);
            var cm = piece.GetComponent<Chessman>();
            cm.ResetBonuses();
            cm.xBoard = cm.startingPosition.x;
            cm.yBoard = cm.startingPosition.y;
            positions[cm.xBoard, cm.yBoard] = piece;
            cm.UpdateUIPosition();
            black.pieces.Add(piece);
        }
        black.capturedPieces.Clear();

        // Make sure all pieces are properly initialized
        foreach (GameObject piece in white.pieces.Concat(black.pieces))
        {
            var cm = piece.GetComponent<Chessman>();
            cm.isValidForAttack = true;
        }
    }

    public void SetWhiteTurn(){
        foreach (GameObject item in white.pieces)
            {
                item.GetComponent<Chessman>().isValidForAttack=true;
            }
        foreach (GameObject item in black.pieces)
            {
                item.GetComponent<Chessman>().isValidForAttack=false;
            }
            
        white.MakeMove(this);
    }
    public void SetBlackTurn(){
        foreach (GameObject item in black.pieces)
        {
            item.GetComponent<Chessman>().isValidForAttack=true;
        }
        foreach (GameObject item in white.pieces)
        {
            item.GetComponent<Chessman>().isValidForAttack=false;
        }
        black.MakeMove(this);
    }

    public void NextTurn()
    {
        //Debug.Log("IsTurnOverride? "+turnOverride);
        if(BloodThirstOverride || AdamantAssaultOverride)
            return;
        if (currentPlayer == PieceColor.White)
        {
            currentPlayer = PieceColor.Black;
            SetBlackTurn();
        }
        else
        {
            currentPlayer = PieceColor.White;
            SetWhiteTurn();
        }
    }

    public GameObject GetPieceAtPosition(int x, int y)
    {
        return positions[x, y];
    }

    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public void MovePiece(Chessman piece, int x, int y)
    {
        // Clear the piece's old position first
        if (piece.xBoard >= 0 && piece.yBoard >= 0)
        {
            SetPositionEmpty(piece.xBoard, piece.yBoard);
        }

        // Check for castling
        if (piece is King && !piece.hasMoved && Math.Abs(x - piece.xBoard) == 2)
        {
            bool isKingside = x > piece.xBoard;
            int rookStartX = isKingside ? piece.xBoard + 3 : piece.xBoard - 4;
            int rookEndX = isKingside ? x - 1 : x + 1;

            var rookObj = GetPieceAtPosition(rookStartX, piece.yBoard);
            if (rookObj != null)
            {
                var rook = rookObj.GetComponent<Chessman>();
                SetPositionEmpty(rookStartX, piece.yBoard);
                rook.xBoard = rookEndX;
                rook.yBoard = piece.yBoard;
                positions[rookEndX, piece.yBoard] = rook.gameObject;
                rook.UpdateUIPosition();
                rook.hasMoved = true;
            }
        }

        // Move the piece
        piece.xBoard = x;
        piece.yBoard = y;
        positions[x,y] = piece.gameObject;
        piece.UpdateUIPosition();

        // Handle pawn movement
        if (piece is Pawn pawn)
        {
            pawn.OnMove();
        }
    } 

    public void PlayerTurn(){
        currentPlayer=Game._instance.heroColor;
    }
    public void EndGame(){
        BattlePanel._instance.HideResults();   
        BattlePanel._instance.HideStats();
        ResetBoard();
        Game._instance.EndMatch();
    }
    public GameObject[,] GetPositions()
    {
        return positions;
    }
}