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
    public PieceColor currentPlayerColor;
    public Player currentPlayer;
    public Player white;
    public Player black;
    private GameObject[,] positions = new GameObject[8, 8];
    public bool AdamantAssaultOverride = false;
    public bool BloodThirstOverride = false;
    public bool AvengingStrikeOverride = false;
    
    public int reward = 40;

    public bool AvengerActive = false;

    public bool isSetUpPhase = true;
    //[SerializeField] private GameObject tilePrefab;


    public ChessMatch(Player white, Player black)
    {
        //ResetPieces();
        this.white=white;
        this.black=black;
        CheckInventory();
        
    }

    public void StartMatch(){
        KingsOrderManager._instance.Setup();
        white.CreateMoveCommandDictionary();
        black.CreateMoveCommandDictionary();
        isSetUpPhase=false;
        Game._instance.toggleAllPieceColliders(false);
        BoardManager._instance.toggleTileColliders(true);
        UpdateBoard();
        SetWhiteTurn();
        Game._instance.OnChessMatchStart.Invoke();
    }
    private void DestroyTiles()
    {
        BoardManager._instance.DestroyBoard();
    }

    public void CheckInventory(){
        UpdateBoard();
        if (Game._instance.hero.inventoryPieces.Count>0){
            int i = 0;
            foreach (var obj in Game._instance.hero.inventoryPieces)
            {
                Chessman piece = obj.GetComponent<Chessman>();
                obj.SetActive(true);
                piece.xBoard=-4;
                piece.yBoard=3-i; 
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
    public ChessMatch(AIPlayer white)
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
        currentPlayer=white;
        foreach (GameObject item in white.pieces)
            {
                if(item.GetComponent<Chessman>().paralyzed){
                    item.GetComponent<Chessman>().isValidForAttack=false;
                    item.GetComponent<Chessman>().paralyzed=false;
                }
                else{
                    item.GetComponent<Chessman>().isValidForAttack=true;
                }
            }
        foreach (GameObject item in black.pieces)
            {
                item.GetComponent<Chessman>().isValidForAttack=false;
            }
            
        white.MakeMove(this);
    }

    public void SetPiecesValidForAttack(Player player){
        foreach (GameObject item in player.pieces)
        {
            item.GetComponent<Chessman>().isValidForAttack=true;
        }

    }
    public void SetBlackTurn(){
        currentPlayer=black;
        foreach (GameObject item in black.pieces)
        {
            if(item.GetComponent<Chessman>().paralyzed){
                item.GetComponent<Chessman>().isValidForAttack=false;
                item.GetComponent<Chessman>().paralyzed=false;
            }
            else{
                item.GetComponent<Chessman>().isValidForAttack=true;
            }
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
        if(BloodThirstOverride || AdamantAssaultOverride || AvengingStrikeOverride || Game._instance.pauseOverride)
            return;
        if (currentPlayerColor == PieceColor.White)
        {
            currentPlayerColor = PieceColor.Black;
            //currentPlayer=black;
            SetBlackTurn();
        }
        else
        {
            currentPlayerColor = PieceColor.White;
            //currentPlayer=white;
            SetWhiteTurn();
            if(reward>0)
                reward-=2;
        }
    }

    public GameObject GetPieceAtPosition(int x, int y)
    {
        if(positions[x, y])
            return positions[x, y];
        else
            return null;
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

    public void MyTurn(PieceColor player){
        currentPlayerColor=player;
    }
    public void EndGame(){
        BattlePanel._instance.HideResults();   
        BattlePanel._instance.HideStats();
        LogManager._instance.ClearLogs();
        ResetPieces();
        DestroyTiles();
        Game._instance.toggleAllPieceColliders(true);
        BoardManager._instance.toggleTileColliders(false);
        white.playerCoins+=reward;
        Game._instance.EndMatch();
    }
    public GameObject[,] GetPositions()
    {
        return positions;
    }
    
}