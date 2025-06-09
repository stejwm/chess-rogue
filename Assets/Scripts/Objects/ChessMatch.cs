using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public class ChessMatch : IGameInputReceiver
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
    public bool SwiftOverride = false;
    int turnReward = 30;
    public int reward;
    public bool AvengerActive = false;
    public bool isSetUpPhase = true;
    public int turns = 0;
    
    public UnityEvent<Chessman, Chessman> OnPieceCaptured = new UnityEvent<Chessman, Chessman>();
    public UnityEvent<Chessman, int, bool, BoardPosition> OnAttack = new UnityEvent<Chessman, int, bool, BoardPosition>();
    public UnityEvent<Chessman, Chessman> OnAttackStart = new UnityEvent<Chessman, Chessman>();
    public UnityEvent<Chessman, Chessman, int, int> OnAttackEnd = new UnityEvent<Chessman, Chessman, int, int>();
    public UnityEvent<Chessman, BoardPosition> OnMove = new UnityEvent<Chessman, BoardPosition>();
    public UnityEvent<Chessman, BoardPosition> OnRawMoveEnd = new UnityEvent<Chessman, BoardPosition>();
    public UnityEvent<Chessman, Chessman, bool> OnPieceBounced = new UnityEvent<Chessman,Chessman, bool>();
    public UnityEvent<Chessman, Chessman, Chessman> OnSupportAdded = new UnityEvent<Chessman, Chessman, Chessman>();
    public UnityEvent OnChessMatchStart = new UnityEvent();


    public ChessMatch(Player white, Player black)
    {
        //ResetPieces();
        this.white = white;
        this.black = black;

    }

    public void HandleClick(GameObject clicked)
    {
        Tile tile = clicked.GetComponent<Tile>();
        if (tile != null)
        {
            HandleTileClick(tile);
        }
    }
    
    private void HandleTileClick(Tile tile)
    {
        // Piece selection, move handling, etc.
    }


    public void StartMatch(){
        reward= 4;
        Debug.Log("Match Starting");
        KingsOrderManager._instance.Setup();
        white.CreateMoveCommandDictionary();
        black.CreateMoveCommandDictionary();
        isSetUpPhase=false;
        //GameManager._instance.toggleAllPieceColliders(false);
        Board._instance.toggleTileColliders(true);
        UpdateBoard();
        SetWhiteTurn();
        OnChessMatchStart.Invoke();
    }

    public void CheckInventory(){
        UpdateBoard();
        if (white.inventoryPieces.Count>0){
            KingsOrderManager._instance.Hide();
            int i = 0;
            foreach (var obj in white.inventoryPieces)
            {
                Chessman piece = obj.GetComponent<Chessman>();
                obj.SetActive(true);
                piece.xBoard=-4;
                piece.yBoard=3-i; 
                i++;
                piece.UpdateUIPosition();
            }
            //GameManager._instance.toggleAllPieceColliders(false);
            //GameManager._instance.togglePieceColliders(GameManager._instance.hero.inventoryPieces, true);
        }
        else{
            StartMatch();
        }
    }

    public void ExecuteTurn(Chessman piece, int x, int y){
        //GameManager._instance.isInMenu=true;
        MoveManager._instance.Set(this, piece,x,y);
        MoveManager._instance.HandleMove(piece,x,y);
    }
    public void UpdateBoard(){
        foreach (GameObject piece in white.pieces)
        {
            Chessman cm = piece.GetComponent<Chessman>();
            positions[cm.xBoard,cm.yBoard]=piece; 
        }
        if(black!=null)
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
                CheckHex(item.GetComponent<Chessman>());
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
            CheckHex(item.GetComponent<Chessman>());
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
        if(BloodThirstOverride || AdamantAssaultOverride || AvengingStrikeOverride ||  SwiftOverride)
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
            turns++;
        }
    }

    public void CheckHex(Chessman piece){
        if(piece.hexed){
            foreach(var ability in piece.abilities){
                ability.Remove(piece);
            }
            piece.hexed=false;
            piece.wasHexed=true;
        }
        else if(piece.wasHexed){
            piece.wasHexed=false;
            List<Ability> abilitiesCopy = new List<Ability>(piece.abilities);
            piece.abilities.Clear();
            foreach(var ability in abilitiesCopy){
                ability.Apply(piece);
            }
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
        StatBoxManager._instance.SetAndShowStats(piece);
    } 

    public void MyTurn(PieceColor player){
        currentPlayerColor=player;
    }
    public void EndGame(){
        BattlePanel._instance.HideResults();   
        BattlePanel._instance.HideStats();
        LogManager._instance.ClearLogs();
        ResetPieces();
        //DestroyTiles();
        //GameManager._instance.toggleAllPieceColliders(true);
        Board._instance.toggleTileColliders(false);
        white.playerCoins+=reward;
        white.playerCoins+=(turnReward/turns);
        //GameManager._instance.EndMatch();
    }
    public GameObject[,] GetPositions()
    {
        return positions;
    }
    
}