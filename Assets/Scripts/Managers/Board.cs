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
using Unity.MLAgents.Integrations.Match3;
using System.Threading.Tasks;

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
    [SerializeField] AbilityLogger abilityLogger;
    [SerializeField] StockfishEngine stockFishEngine;
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
    public AbilityLogger AbilityLogger { get => abilityLogger; set => abilityLogger = value; }
    public StockfishEngine StockFishEngine { get => stockFishEngine; set => stockFishEngine = value; }

    public void Start()
    {
        EventHub = new EventHub();
        CreateTiles();
        currencyManager.Initialize(this);
        pauseMenuManager.Initialize(this);
        abilityLogger.Initialize(logManager);
        NameDatabase.LoadNames();
        if (SceneLoadManager.LoadPreviousSave)
        {
            LoadGame();
        }
        else
        {
            LetsBegin();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //hero.capturedPieces = opponent.pieces;
            //opponent.pieces.Clear();
            CurrentMatch.EndMatch();
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

        //Debug.Log($"Resuming board.BoardState{state}");
        mapManager.LoadMap(mapNodes);
        hero.playerBlood = player.blood;
        hero.playerCoins = player.coins;

        hero.pieces = PieceFactory._instance.LoadPieces(this, player.pieces, hero);

        OpenMap();

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
        if (boardState == BoardState.ShopScreen)
        {
            ShopManager.CloseShop();
            boardState = BoardState.None;
            //if(hero.orders.Where(x => x.canBeUsedFromManagement).Count>0)
            OpenManagement();
        }
    }
    public void OpenManagement()
    {
        boardState = BoardState.ManagementScreen;
        ResetPlayerPieces();
        ArmyManager.OpenManagement(this);
    }
    public void CloseManagement()
    {
        if (boardState == BoardState.ManagementScreen && ArmyManager.CloseManagement())
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
        if (boardState == BoardState.KingsOrder)
        {
            boardState = previousBoardState;
            previousBoardState = BoardState.None;
            KingsOrderManager.CloseManagement();
        }
    }
    public void ShopToManagement()
    {
        ShopManager.HideShop();
        ArmyManager.OpenFromShop(this);
        boardState = BoardState.ManagementScreen;
    }
    public void ManagementToShop()
    {
        if (boardState == BoardState.ManagementScreen)
        {
            ArmyManager.ExitToShop();
            ShopManager.OpenShopFromManagement();
            boardState = BoardState.ShopScreen;
        }
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

    public string BoardToFEN()
    {
        int size = 8; // assuming 8x8
        List<string> rows = new List<string>();

        for (int y = size - 1; y >= 0; y--) // FEN starts from rank 8 to 1 (top to bottom)
        {
            string row = "";
            int emptyCount = 0;

            for (int x = 0; x < size; x++) // file a to h (left to right)
            {
                GameObject pieceObj = positions[x, y];

                if (pieceObj == null)
                {
                    emptyCount++;
                }
                else
                {
                    if (emptyCount > 0)
                    {
                        row += emptyCount.ToString();
                        emptyCount = 0;
                    }

                    Chessman piece = pieceObj.GetComponent<Chessman>();
                    row += piece != null ? piece.GetFENChar().ToString() : "?";
                }
            }

            if (emptyCount > 0)
                row += emptyCount.ToString();

            rows.Add(row);
        }

        string piecePlacement = string.Join("/", rows);

        // You can extend this to add the full FEN fields
        string fullFEN = piecePlacement + " b - - 0 1"; // placeholder: white to move, all castling rights, no en passant, etc.

        return fullFEN;
    }

    public int GetSupportAtPosition(Chessman cm, int x, int y, bool isAttacking)
    {
        List<GameObject> possibleSupporters;
        switch (cm.color)
        {
            case PieceColor.White:
                possibleSupporters = Hero.pieces;
                break;
            case PieceColor.Black:
                possibleSupporters = Opponent.pieces;
                break;
            default:
                return 0;

        }

        Tile tile = GetTileAt(x, y);
        int supportTotal;
        if (isAttacking)
            supportTotal = cm.CalculateAttack();
        else
            supportTotal = cm.CalculateDefense();

        foreach (GameObject piece in possibleSupporters)
        {
            Chessman cmSupport = piece.GetComponent<Chessman>();
            if (cmSupport.GetValidSupportMoves().Contains(tile))
            {
                supportTotal += cmSupport.CalculateSupport();
            }
        }
        return supportTotal;

    }

    public List<string> GetValidMovesFromEngineMoves(List<string> moves)
    {
        List<string> validatedMoves = new List<string>();
        foreach (string move in moves)
        {
            BoardPosition.ParseUCIMove(move, out int fromX, out int fromY, out int toX, out int toY);
            Chessman cm = GetChessmanAtPosition(fromX, fromY);
            if (!cm)
                continue;
            if (!cm.isValidForAttack)
                continue;
            if (!cm.GetValidMoves().Contains(GetTileAt(toX, toY)))
                continue;
            if (isUselessBounce(GetChessmanAtPosition(fromX, fromY), GetChessmanAtPosition(toX, toY)))
                continue;
            else
            {
                validatedMoves.Add(move);
            }
        }
        return validatedMoves;
    }
    private bool isUselessBounce(Chessman attacker, Chessman defender)
    {
        if (defender == null)
            return false;
        if (defender.CalculateDefense() > 0)
            return false;
        if (GetSupportAtPosition(defender, defender.xBoard, defender.yBoard, false) < GetSupportAtPosition(attacker, defender.xBoard, defender.yBoard, true))
            return false;
        return true;
    }

    
     

}
