using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public interface IBoardInputReceiver
{
    void HandleTileClick(Tile tile);
    void HandleObjectClick(GameObject obj); // for other types of input
}
public class Board : MonoBehaviour
{
    public Tile[,] tiles = new Tile[8, 8];
    public List<Tile> validTiles = new List<Tile>();
    public BoardPosition selectedPosition;
    private ChessMatch currentMatch;
    private Player hero;
    private EventHub eventHub;
    private BattlePanel battlePanel;
    [SerializeField] private GameObject tilePrefab;

    public ChessMatch CurrentMatch { get => currentMatch; set => currentMatch = value; }
    public BattlePanel BattlePanel { get => battlePanel; set => battlePanel = value; }
    public EventHub EventHub { get => eventHub; set => eventHub = value; }
    public Player Hero { get => hero; set => hero = value; }

    public void Start()
    {
        EventHub = new EventHub();
        CreateTiles();
    }

    public void CreateNewMatch(Player white, Player black)
    {
        currentMatch = new ChessMatch(this, white, black, EventHub);
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

    }
    public void AddPiece(Chessman piece, Tile tile)
    {
        PlacePiece(piece, tile);
        Hero.inventoryPieces.Remove(piece.gameObject);
        Hero.openPositions.Remove(tile);
        Hero.pieces.Add(piece.gameObject);
        eventHub.RaisePieceAdded(piece);
    }


}
