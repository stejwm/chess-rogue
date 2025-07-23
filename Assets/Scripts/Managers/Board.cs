using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using System.Linq;
using CI.QuickSave;
using System.IO;

public enum BoardState
{
    RewardScreen,
    PrisonersMarket,
    ActiveMatch,
    Map,
    ShopScreen,
    ManagementScreen,
    InfoScreen,
    KingsOrder,
    KingsOrderActive,
    None
}
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
    [SerializeField] private GameObject gameOverPanel;
    private Chessman selectedPiece;
    [SerializeField] LogManager logManager;
    [SerializeField] MarketManager marketManager;
    [SerializeField] RewardManager rewardManager;
    [SerializeField] ShopManager shopManager;
    [SerializeField] MapManager mapManager;
    [SerializeField] ArmyManager armyManager;
    [SerializeField] PieceInfoManager pieceInfoManager;
    [SerializeField] KingsOrderManager kingsOrderManager;
    [SerializeField] CurrencyManager currencyManager;
    [SerializeField] PauseMenuManager pauseMenuManager;
    public int Width = 8;
    public int Height = 8;
    private int reRollCost = 2;
    private int rerollCostIncrease = 1;
    private int level = 0;
    public BoardState previousBoardState = BoardState.None;
    private GameObject[,] positions = new GameObject[8, 8];
    [SerializeField] private BoardState boardState;
    public Ability LastingLegacyAbility { get; set; }
    public ChessMatch CurrentMatch { get => currentMatch; set => currentMatch = value; }
    public BattlePanel BattlePanel { get => battlePanel; set => battlePanel = value; }
    public EventHub EventHub { get => eventHub; set => eventHub = value; }
    public Player Hero { get => hero; set => hero = value; }
    public int RerollCost { get => reRollCost; set => reRollCost = value; }
    public GameObject[,] Positions { get => positions; set => positions = value; }
    public BoardState BoardState { get => boardState; set => boardState = value; }
    public MarketManager MarketManager { get => marketManager; set => marketManager = value; }
    public RewardManager RewardManager { get => rewardManager; set => rewardManager = value; }
    public Player Opponent { get => opponent; set => opponent = value; }
    public ShopManager ShopManager { get => shopManager; set => shopManager = value; }
    public bool IsInMove { get; set; }
    public int Level { get => level; set => level = value; }
    public ArmyManager ArmyManager { get => armyManager; set => armyManager = value; }
    public PieceInfoManager PieceInfoManager { get => pieceInfoManager; set => pieceInfoManager = value; }
    public KingsOrderManager KingsOrderManager { get => kingsOrderManager; set => kingsOrderManager = value; }
    public PauseMenuManager PauseMenuManager { get => pauseMenuManager; set => pauseMenuManager = value; }
    public MapManager MapManager { get => mapManager; set => mapManager = value; }
    public int RerollCostIncrease { get => rerollCostIncrease; set => rerollCostIncrease = value; }
    public bool headless = false; // Set to true for headless mode, false for normal gameplay

    public void Start()
    {
        EventHub = new EventHub();
        CreateTiles();
        if (!headless)
        {
            currencyManager.Initialize(this);
            pauseMenuManager.Initialize(this);
        }
        NameDatabase.LoadNames();
        AbilityDatabase.Instance.LoadAbilities();
        if (SceneLoadManager.LoadPreviousSave)
        {
            LoadGame();
        }
        else
        {
            //LetsBegin();
            LoadRandomGame();
        }
    }
    public void LetsBegin()
    {
        opponent.pieces = PieceFactory._instance.CreateKnightsOfTheRoundTable(this, opponent, opponent.color);
        hero.pieces = PieceFactory._instance.CreatePiecesForColor(this, hero, hero.color);
        hero.Initialize(this);
        opponent.Initialize(this);
        CreateNewMatch(hero, opponent);
        BoardState = BoardState.ActiveMatch;
    }
    public void LoadGame()
    {
        var quickSaveReader = QuickSaveReader.Create("Game");
        PlayerData player;
        List<MapNodeData> mapNodes;
        BoardState state;
        quickSaveReader.TryRead<PlayerData>("Player", out player);
        quickSaveReader.TryRead<BoardState>("State", out state);
        quickSaveReader.TryRead<int>("Level", out level);
        quickSaveReader.TryRead<List<MapNodeData>>("MapNodes", out mapNodes);
        BoardState = state;

        Debug.Log($"Resuming board.BoardState{state}");
        mapManager.LoadMap(mapNodes);
        hero.playerBlood = player.blood;
        hero.playerCoins = player.coins;

        hero.pieces = PieceFactory._instance.LoadPieces(this, player.pieces, hero);

        OpenMap();

    }
    public void LoadRandomGame()
    {
        opponent.DestroyPieces();
        hero.DestroyPieces();
        Array.Clear(positions, 0, positions.Length);
        foreach (Tile tile in tiles)
        {
            tile.ClearBloodTile();
        }
        EnemyType enemyType = (EnemyType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(EnemyType)).Length);
        opponent.pieces = PieceFactory._instance.CreateOpponentPieces(this, opponent, enemyType);

        
        hero.pieces = CreateRandomHeroPiecesFromSave();
        hero.Initialize(this);
        opponent.Initialize(this);
        opponent.LevelUp(level, enemyType);
        
        CreateNewMatch(hero, opponent);    
        
    }
    public List<GameObject> CreateRandomHeroPiecesFromSave(){
        // Get all save files matching pattern
        string savePath = "C:\\Users\\steve\\chess-rogue\\chess-rogue\\Saves";
        QuickSaveGlobalSettings.StorageLocation = savePath;
        var saveFiles = Directory.GetFiles(savePath + "\\QuickSave")
            .ToList();

        Debug.Log($"Found {saveFiles.Count} save files.");
        if (saveFiles.Count == 0)
            return PieceFactory._instance.CreatePiecesForColor(this, hero, hero.color);

        // Select random save file
        string selectedSave = saveFiles[UnityEngine.Random.Range(0, saveFiles.Count)];
        Debug.Log($"Selected save file: {Path.GetFileName(selectedSave)}");
        
        try
        {
            var quickSaveReader = QuickSaveReader.Create(Path.GetFileName(selectedSave).Replace(".json", ""));
            PlayerData player;
            quickSaveReader.TryRead<PlayerData>("Player", out player);
            quickSaveReader.TryRead<int>("Level", out level);
            
            hero.playerBlood=player.blood;
            hero.playerCoins=player.coins;
            return PieceFactory._instance.LoadPieces(this, player.pieces, hero);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load save file: {e.Message}");
            return PieceFactory._instance.CreatePiecesForColor(this, hero, hero.color);
        }
        
        
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
        opponent.Initialize(this);
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
            if (piece)
            {
                piece.SetActive(true);
                Chessman cm = piece.GetComponent<Chessman>();
                cm.ResetBonuses();
                cm.flames.Stop();
                cm.highlightedParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                PlacePiece(cm, cm.startingPosition);
            }
        }
    }
    public void PlacePiece(Chessman piece, Tile tile)
    {
        piece.xBoard = tile.X;
        piece.yBoard = tile.Y;
        piece.UpdateUIPosition();
        positions[tile.X, tile.Y] = piece.gameObject;

    }
    public void ClearPosition(int x, int y)
    {
        positions[x, y] = null;
    }
    public void AddPiece(Chessman piece, Tile tile)
    {
        piece.startingPosition = tile;
        PlacePiece(piece, tile);
        Hero.inventoryPieces.Remove(piece.gameObject);
        Hero.openPositions.Remove(tile);
        Hero.pieces.Add(piece.gameObject);
        SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
        piece.flames.GetComponent<Renderer>().sortingOrder = 2;
        rend.sortingOrder = 1;
        eventHub.RaisePieceAdded(piece);
    }
    public void PiecesToStartingPositions()
    {
        Array.Clear(positions, 0, positions.Length);
        foreach (GameObject piece in hero.pieces)
        {
            if (piece)
            {
                piece.SetActive(true);
                piece.GetComponent<Chessman>().ResetBonuses();
                Chessman cm = piece.GetComponent<Chessman>();
                PlacePiece(cm, cm.startingPosition);
            }
            else
            {
                Debug.Log($"Lost piece in hero pieces..");
            }
        }
        foreach (GameObject piece in opponent.pieces)
        {
            if (piece)
            {
                piece.SetActive(true);
                piece.GetComponent<Chessman>().ResetBonuses();
                Chessman cm = piece.GetComponent<Chessman>();
                PlacePiece(cm, cm.startingPosition);
            }
            else
            {
                Debug.Log($"Lost piece in opponent pieces..");
            }
        }
        foreach (GameObject piece in hero.capturedPieces)
        {
            if (piece)
            {
                piece.SetActive(true);
                piece.GetComponent<Chessman>().ResetBonuses();
                Chessman cm = piece.GetComponent<Chessman>();
                PlacePiece(cm, cm.startingPosition);
            }
            else
            {
                Debug.Log($"Lost piece in hero captured pieces.. ");
            }
        }
        foreach (GameObject piece in opponent.capturedPieces)
        {
            if (piece)
            {
                piece.SetActive(true);
                piece.GetComponent<Chessman>().ResetBonuses();
                Chessman cm = piece.GetComponent<Chessman>();
                PlacePiece(cm, cm.startingPosition);
            }
            else
            {
                Debug.Log($"Lost piece in opponent captured pieces.. removing from list");
            }
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
        boardState = BoardState.PrisonersMarket;
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
        ResetPlayerPieces();
        ShopManager.OpenShop(this);
    }
    public void CloseShop()
    {
        ShopManager.CloseShop();
        boardState = BoardState.None;
        //if(hero.orders.Where(x => x.canBeUsedFromManagement).Count>0)
        OpenManagement();
    }
    public void OpenManagement()
    {
        boardState = BoardState.ManagementScreen;
        ResetPlayerPieces();
        ArmyManager.OpenManagement(this);
    }
    public void CloseManagement()
    {
        if (ArmyManager.CloseManagement())
        {
            boardState = BoardState.None;
            OpenMap();
        }
    }
    public void OpenMap()
    {
        ResetPlayerPieces();
        boardState = BoardState.Map;
        MapManager.OpenMap(this);
    }
    public void CloseMap()
    {
        MapManager.CloseMap();
        this.boardState = BoardState.ActiveMatch;
    }
    public void OpenKingsOrders()
    {
        if (boardState != BoardState.KingsOrder)
        {
            previousBoardState = boardState;
        }
        if (boardState == BoardState.ActiveMatch || boardState == BoardState.ManagementScreen)
        {
            KingsOrderManager.OpenManagement(this);
            boardState = BoardState.KingsOrder;
        }
        else if (boardState == BoardState.ShopScreen)
        {
            ShopToManagement();
            previousBoardState = BoardState.ManagementScreen;
            KingsOrderManager.OpenManagement(this);
            boardState = BoardState.KingsOrder;
        }

    }
    public void CloseKingsOrders()
    {
        boardState = previousBoardState;
        previousBoardState = BoardState.None;
        KingsOrderManager.CloseManagement();
    }
    public void ShopToManagement()
    {
        ShopManager.HideShop();
        ArmyManager.OpenFromShop(this);
        boardState = BoardState.ManagementScreen;
    }
    public void ManagementToShop()
    {
        ArmyManager.ExitToShop();
        ShopManager.OpenShopFromManagement();
        boardState = BoardState.ShopScreen;
    }
    public void OpenPieceInfo(Chessman piece)
    {
        if (boardState != BoardState.InfoScreen)
        {
            previousBoardState = boardState;
        }
        boardState = BoardState.InfoScreen;
        pieceInfoManager.OpenPieceInfo(this, piece);
    }
    public void ClosePieceInfo()
    {
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
    public Chessman GetChessmanAtPosition(int x, int y)
    {
        if (positions[x, y])
            return positions[x, y].GetComponent<Chessman>();
        else
            return null;
    }
    public void SetSelectedPosition(Tile tile)
    {
        if (selectedPosition == null)
        {
            selectedPosition = tile;
            Debug.Log("position set");
        }
    }
    public void ClearSelectedPosition()
    {
        selectedPosition = null;
    }
    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

}
