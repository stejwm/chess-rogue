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
    public bool SwiftOverride = false;
    
    int turnReward = 30;
    public int reward;

    public bool AvengerActive = false;

    public bool isSetUpPhase = true;
    public int turns = 0;


    public ChessMatch(Player white, Player black)
    {
        //ResetPieces();
        this.white=white;
        this.black=black;
        
    }

    public ChessMatch(Player white)
    {
        this.white=white;
        isSetUpPhase=false;
        UpdateBoard();
    }

    public void TutorialMatch(){
        Game._instance.tutorial=true;
        CoroutineRunner.instance.StartCoroutine(Tutorial());

    }

    public IEnumerator Tutorial(){
        MovePiece(positions[1,0].GetComponent<Chessman>(), 0, 2); // White knight
        MovePiece(positions[3,1].GetComponent<Chessman>(), 3, 1); // White D pawn
        MovePiece(positions[4,1].GetComponent<Chessman>(), 4, 3); // White E pawn
        MovePiece(positions[1,7].GetComponent<Chessman>(), 2, 5); // Black night 
        MovePiece(positions[3,7].GetComponent<Chessman>(), 5, 5); //Black queen
        MovePiece(positions[5,7].GetComponent<Chessman>(), 7, 3); //Black bishop
        MovePiece(positions[6,7].GetComponent<Chessman>(), 6, 3); //Black night
        MovePiece(positions[3,6].GetComponent<Chessman>(), 3, 5); //Black D pawn
        MovePiece(positions[4,6].GetComponent<Chessman>(), 4, 4); //Black E pawn

        yield return null;
    }

    public void StartMatch(){
        reward= 4;
        Debug.Log("Match Starting");
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
            isSetUpPhase=true;
            KingsOrderManager._instance.Hide();
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
        if(black!=null)
        foreach (GameObject piece in black.pieces)
        {
            Chessman cm = piece.GetComponent<Chessman>();
            positions[cm.xBoard,cm.yBoard]=piece; 
        }
        
    }

    public float CalculateBoardState(Player player)
    {
        int boardState = 0;
        int mod = 1;
        foreach (GameObject piece in positions)
        {
            if (piece != null)
            {
                Chessman cm = piece.GetComponent<Chessman>();
                if (cm != null)
                {
                    if (cm.color == player.color)
                    {
                        mod = 1;
                    }
                    else
                    {
                        mod = -1;
                    }
                    boardState += mod * cm.CalculateAttack();
                    boardState += mod * cm.CalculateDefense();
                    boardState += mod * cm.CalculateSupport();
                    boardState += mod * cm.abilities.Count;

                    boardState += mod * cm.GetValidMoves().Count;

                    var supportMoves = cm.GetValidSupportMoves();
                    boardState += mod * supportMoves.Count;
                    foreach (var move in supportMoves)
                    {
                        var connectedPiece = GetPieceAtPosition(move.x, move.y);
                        if (connectedPiece != null && connectedPiece.GetComponent<Chessman>().color == cm.color)
                        {
                            boardState += mod * cm.CalculateSupport(); // Count support moves that support own pieces
                        }
                        if (connectedPiece != null && connectedPiece.GetComponent<Chessman>().color != cm.color && connectedPiece.GetComponent<Chessman>().type == PieceType.King)
                        {
                            boardState += mod * Math.Max(cm.CalculateAttack(), cm.CalculateSupport());
                        }

                    }
                }
            }
        }
        return boardState / 100f; // Normalize the score to a range of -1 to 1
    }

    public void ResetPieces()
    {
        foreach (GameObject piece in white.pieces)
        {
            piece.SetActive(true);
            piece.GetComponent<Chessman>().ResetBonuses();
            Chessman cm = piece.GetComponent<Chessman>();
            MovePiece(cm, cm.startingPosition.x, cm.startingPosition.y);
        }
        foreach (GameObject piece in black.pieces)
        {
            piece.SetActive(true);
            piece.GetComponent<Chessman>().ResetBonuses();
            Chessman cm = piece.GetComponent<Chessman>();
            MovePiece(cm, cm.startingPosition.x, cm.startingPosition.y);
        }
        foreach (GameObject piece in white.capturedPieces)
        {
            piece.SetActive(true);
            piece.GetComponent<Chessman>().ResetBonuses();
            Chessman cm = piece.GetComponent<Chessman>();
            MovePiece(cm, cm.startingPosition.x, cm.startingPosition.y);
        }
        foreach (GameObject piece in black.capturedPieces)
        {
            piece.SetActive(true);
            piece.GetComponent<Chessman>().ResetBonuses();
            Chessman cm = piece.GetComponent<Chessman>();
            MovePiece(cm, cm.startingPosition.x, cm.startingPosition.y);
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
        if(BloodThirstOverride || AdamantAssaultOverride || AvengingStrikeOverride || Game._instance.pauseOverride || SwiftOverride || Game._instance.tutorial)
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
        DestroyTiles();
        Game._instance.toggleAllPieceColliders(true);
        BoardManager._instance.toggleTileColliders(false);
        white.playerCoins+=reward;
        white.playerCoins+=(turnReward/turns);
        Game._instance.EndMatch();
    }
    public GameObject[,] GetPositions()
    {
        return positions;
    }
    
}