using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public bool AvengingStrikeOverride = false;

    public bool isSetUpPhase = true;
    //[SerializeField] private GameObject tilePrefab;


    public ChessMatch(Player white, Player black)
    {
        this.white=white;
        this.black=black;
        CreateTiles();
        CheckInventory();
        
    }

    public void StartMatch(){
        isSetUpPhase=false;
        Game._instance.toggleAllPieceColliders(false);
        BoardManager._instance.toggleTileColliders(true);
        UpdateBoard();
        SetWhiteTurn();
    }
    private void CreateTiles()
    {
        BoardManager._instance.CreateBoard();
    }
    private void DestroyTiles()
    {
        BoardManager._instance.DestroyBoard();
    }

    public void CheckInventory(){
        UpdateBoard();
        ResetPieces();
        if (Game._instance.hero.inventoryPieces.Count>0){
            int i = 0;
            foreach (var obj in Game._instance.hero.inventoryPieces)
            {
                Chessman piece = obj.GetComponent<Chessman>();
                obj.SetActive(true);
                piece.xBoard=-4;
                piece.yBoard=4-i; 
                i++;
                piece.UpdateUIPosition();
            }
            Game._instance.toggleAllPieceColliders(false);
            Game._instance.togglePieceColliders(Game._instance.hero.inventoryPieces, true);
        }
        else{
            StartMatch();
        }
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

    public void ResetPieces(){
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
        //Debug.Log("IsTurnOverride? "+turnOverride);
        if(BloodThirstOverride || AdamantAssaultOverride || AvengingStrikeOverride)
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
        Debug.Log(piece.name + " moved to "+ BoardPosition.ConvertToChessNotation(x,y));
    } 

    public void MyTurn(PieceColor player){
        currentPlayer=player;
    }
    public void EndGame(){
        BattlePanel._instance.HideResults();   
        BattlePanel._instance.HideStats();
        LogManager._instance.ClearLogs();
        ResetPieces();
        DestroyTiles();
        Game._instance.toggleAllPieceColliders(true);
        BoardManager._instance.toggleTileColliders(false);
        Game._instance.EndMatch();
    }
    public GameObject[,] GetPositions()
    {
        return positions;
    }
    
}