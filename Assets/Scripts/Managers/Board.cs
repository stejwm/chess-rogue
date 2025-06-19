using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

public class Board : MonoBehaviour
{
    public Tile[,] tiles = new Tile[8, 8];
    public List<Tile> validTiles = new List<Tile>();
    public Tile selectedPosition;
    private ChessMatch currentMatch;
    [SerializeField] private Player hero;
    [SerializeField] private Player opponent;
    private EventHub eventHub;
    [SerializeField] private BattlePanel battlePanel;
    [SerializeField] private GameObject tilePrefab;
    private Chessman selectedPiece;
    [SerializeField] GameManager manager;
    [SerializeField] LogManager logManager;
    [SerializeField] MarketManager marketManager;
    [SerializeField] RewardManager rewardManager;
    [SerializeField] ShopManager shopManager;
    [SerializeField] MapManager mapManager;
    [SerializeField] ArmyManager armyManager;
    [SerializeField] PieceInfoManager pieceInfoManager;
    private bool tileSelect = false;
    private int reRollCost = 0;
    private BoardState previousBoardState = BoardState.None;
    private GameObject[,] positions = new GameObject[8, 8];
    private BoardState boardState;
    public Ability LastingLegacyAbility { get; set; }
    public ChessMatch CurrentMatch { get => currentMatch; set => currentMatch = value; }
    public BattlePanel BattlePanel { get => battlePanel; set => battlePanel = value; }
    public EventHub EventHub { get => eventHub; set => eventHub = value; }
    public Player Hero { get => hero; set => hero = value; }
    public bool TileSelect { get => tileSelect; set => tileSelect = value; }
    public int RerollCost { get => reRollCost; set => reRollCost = value; }
    public GameObject[,] Positions { get => positions; set => positions = value; }
    public BoardState BoardState { get => boardState; set => boardState = value; }
    public MarketManager MarketManager { get => marketManager; set => marketManager = value; }
    public RewardManager RewardManager { get => rewardManager; set => rewardManager = value; }
    public Player Opponent { get => opponent; set => opponent = value; }
    public ShopManager ShopManager { get => shopManager; set => shopManager = value; }
    public bool IsInMove { get; set; }
    public int Level { get; set; }
    public ArmyManager ArmyManager { get => armyManager; set => armyManager = value; }
    public PieceInfoManager PieceInfoManager { get => pieceInfoManager; set => pieceInfoManager = value; }

    public void Start()
    {
        EventHub = new EventHub();
        CreateTiles();
    }
    public void CreateNewMatch(Player white, Player black)
    {
        currentMatch = new ChessMatch(this, white, black, EventHub, logManager);
        opponent = black;
        currentMatch.StartMatch();
    }
    public void CreateNewMatch(EnemyType enemyType)
    {
        opponent.pieces = PieceFactory._instance.CreateOpponentPieces(this, opponent, enemyType);
        opponent.Initialize();
        opponent.LevelUp(Level, enemyType);
        currentMatch = new ChessMatch(this, hero, opponent, EventHub, logManager);
        currentMatch.StartMatch();
    }
    public void CreateTiles()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GameObject tileObject = Instantiate(tilePrefab, this.transform);

                // Get the Tile component
                Tile tile = tileObject.GetComponent<Tile>();
                if (tile == null)
                {
                    Debug.LogError("Tile prefab must have a Tile component!");
                }

                // Initialize the tile
                tile.Initialize(i, j);
                tiles[i, j] = tile;
            }
        }
    }
    public void SetActiveTile(Chessman piece, Tile tile)
    {
        validTiles.Add(tile);
        tile.SetReference(piece);
        tile.SetValidMove();
    }
    public Tile GetTileAt(int x, int y)
    {
        if (x < 0 || x >= 8 || y < 0 || y >= 8)
        {
            return null;
        }
        return tiles[x, y];
    }
    public void ClearTiles()
    {
        foreach (var tile in validTiles)
        {
            tile.Clear();
        }
        validTiles.Clear();
    }
    public void ResetPlayerPieces()
    {
        foreach (GameObject piece in hero.pieces)
        {
            piece.SetActive(true);
            Chessman cm = piece.GetComponent<Chessman>();
            piece.GetComponent<Chessman>().ResetBonuses();
            PlacePiece(cm, cm.startingPosition);
        }
    }
    public void PlacePiece(Chessman piece, Tile tile)
    {
        piece.xBoard = tile.X;
        piece.yBoard = tile.Y;
        piece.UpdateUIPosition();
        positions[tile.X, tile.Y] = piece.gameObject;

    }
    public void AddPiece(Chessman piece, Tile tile)
    {
        PlacePiece(piece, tile);
        Hero.inventoryPieces.Remove(piece.gameObject);
        Hero.openPositions.Remove(tile);
        Hero.pieces.Add(piece.gameObject);
        eventHub.RaisePieceAdded(piece);
    }
    public void PiecesToStartingPositions()
    {
        Array.Clear(positions, 0, positions.Length);
        foreach (GameObject piece in hero.pieces)
        {
            piece.SetActive(true);
            piece.GetComponent<Chessman>().ResetBonuses();
            Chessman cm = piece.GetComponent<Chessman>();
            PlacePiece(cm, cm.startingPosition);
        }
        foreach (GameObject piece in opponent.pieces)
        {
            piece.SetActive(true);
            piece.GetComponent<Chessman>().ResetBonuses();
            Chessman cm = piece.GetComponent<Chessman>();
            PlacePiece(cm, cm.startingPosition);       
        }
        foreach (GameObject piece in hero.capturedPieces)
        {
            piece.SetActive(true);
            piece.GetComponent<Chessman>().ResetBonuses();
            Chessman cm = piece.GetComponent<Chessman>();
            PlacePiece(cm, cm.startingPosition);      
        }
        foreach (GameObject piece in opponent.capturedPieces)
        {
            piece.SetActive(true);
            piece.GetComponent<Chessman>().ResetBonuses();
            Chessman cm = piece.GetComponent<Chessman>();
            PlacePiece(cm, cm.startingPosition);        
        }
        
    }
    public void ResetBoard()
    {
        foreach (Tile tile in tiles)
        {
            tile.ClearBloodTile();
        }
        PiecesToStartingPositions();
    }
    public void EndMatch()
    {
        Level++;
        currentMatch = null;
        boardState = BoardState.None;
        ResetBoard();
        OpenMarket();
    }
    public void OpenMarket()
    {
        boardState= BoardState.PrisonersMarket;
        marketManager.OpenMarket(this);
    }
    public void CloseMarket()
    {
        marketManager.CloseMarket(this);
        boardState = BoardState.None;
        opponent.DestroyPieces();
        OpenReward();
    }
    public void OpenReward()
    {
        boardState = BoardState.RewardScreen;
        ResetPlayerPieces();
        StatBoxManager._instance.HideEnemyStatBox();
        RewardManager.OpenReward(this);
    }
    public void CloseReward()
    {
        rewardManager.CloseReward(this);
        boardState = BoardState.None;
        OpenShop();
    }
    public void OpenShop()
    {
        boardState = BoardState.ShopScreen;
        ShopManager.OpenShop(this);
    }
    public void CloseShop()
    {
        ShopManager.CloseShop();
        boardState = BoardState.None;
        OpenManagement();
    }
    public void OpenManagement()
    {
        boardState = BoardState.ManagementScreen;
        ArmyManager.OpenManagement(this);
    }
    public void CloseManagement()
    {
        ArmyManager.CloseManagement();
        boardState = BoardState.None;
        OpenMap();
    }
    public void OpenMap()
    {
        mapManager.OpenMap(this);
    }
    public void CloseMap()
    {
        mapManager.CloseMap();
        this.boardState = BoardState.ActiveMatch;
    }
    public void OpenPieceInfo(Chessman piece)
    {
        if (boardState != BoardState.InfoScreen)
        {
            previousBoardState = boardState;
            Debug.Log(previousBoardState);
        }
        boardState = BoardState.InfoScreen;
        pieceInfoManager.OpenPieceInfo(piece);
    }
    public void ClosePieceInfo()
    {
        Debug.Log(previousBoardState);
        boardState = previousBoardState;
        previousBoardState = BoardState.None;
        pieceInfoManager.ClosePieceInfo();
    }
    public GameObject GetPieceAtPosition(int x, int y)
    {
        if (positions[x, y])
            return positions[x, y];
        else
            return null;
    }
    public GameObject GetPieceAtPosition(Tile tile)
    {
        if (positions[tile.X, tile.Y])
            return positions[tile.X, tile.Y];
        else
            return null;
    }
    public Chessman GetChessmanAtPosition(Tile tile)
    {
        if (positions[tile.X, tile.Y])
            return positions[tile.X, tile.Y].GetComponent<Chessman>();
        else
            return null;
    }


}
