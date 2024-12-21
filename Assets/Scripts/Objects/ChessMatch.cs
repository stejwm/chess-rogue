using System;
using System.Collections;
using System.Collections.Generic;
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
    public bool turnOverride = false;


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
        foreach (GameObject piece in white.pieces)
        {
            Chessman cm = piece.GetComponent<Chessman>();
            positions[cm.xBoard,cm.yBoard]=piece; 
        }
        foreach (GameObject piece in black.pieces)
        {
            Chessman cm = piece.GetComponent<Chessman>();
            positions[cm.xBoard,cm.yBoard]=piece; 
        }
        
    }

    public void ResetBoard(){
        foreach (GameObject piece in white.pieces)
        {
            piece.SetActive(true);
            piece.GetComponent<Chessman>().ResetBonuses();
            Chessman cm = piece.GetComponent<Chessman>();
            MovePiece(cm, cm.startingPosition.x,cm.startingPosition.y);
        }
        foreach (GameObject piece in black.pieces)
        {
            piece.SetActive(true);
            piece.GetComponent<Chessman>().ResetBonuses();
            Chessman cm = piece.GetComponent<Chessman>();
            MovePiece(cm, cm.startingPosition.x,cm.startingPosition.y);        
        }
        foreach (GameObject piece in white.capturedPieces)
        {
            piece.SetActive(true);
            piece.GetComponent<Chessman>().ResetBonuses();
            Chessman cm = piece.GetComponent<Chessman>();
            MovePiece(cm, cm.startingPosition.x,cm.startingPosition.y);        
        }
        foreach (GameObject piece in black.capturedPieces)
        {
            piece.SetActive(true);
            piece.GetComponent<Chessman>().ResetBonuses();
            Chessman cm = piece.GetComponent<Chessman>();
            MovePiece(cm, cm.startingPosition.x,cm.startingPosition.y);        
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
        Debug.Log("IsTurnOverride? "+turnOverride);
        if(turnOverride)
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

    public void MovePiece(Chessman piece, int x, int y){
        piece.xBoard = x;
        piece.yBoard = y;
        positions[x,y] = piece.gameObject;
        piece.UpdateUIPosition();
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