using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PieceFactory : MonoBehaviour
{
    public GameObject pawn;
    public GameObject knight;
    public GameObject bishop;
    public GameObject rook;
    public GameObject queen;
    public GameObject king;

    public static PieceFactory _instance;
    //private BoardManager boardManager;

    private void Awake()
    {
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }

    public List<GameObject> CreateWhitePieces(Player owner)
    {
        return CreatePiecesForColor(PieceColor.White, Team.Hero, owner);
    }

    public List<GameObject> CreateBlackPieces(Player owner)
    {
        return CreatePiecesForColor(PieceColor.Black, Team.Enemy, owner);
    }

    private List<GameObject> CreatePiecesForColor(PieceColor color, Team team, Player owner)
    {
        string prefix = color == PieceColor.White ? "white" : "black";
        int backRow = color == PieceColor.White ? 0 : 7;
        int pawnRow = color == PieceColor.White ? 1 : 6;

        // Create back row
        List<GameObject> pieces = new List<GameObject> {
            Create(PieceType.Rook, $"{prefix}_rook", 0, backRow, color, team, owner),
            Create(PieceType.Knight, $"{prefix}_knight", 1, backRow, color, team, owner),
            Create(PieceType.Bishop, $"{prefix}_bishop", 2, backRow, color, team, owner),
            Create(PieceType.Queen, $"{prefix}_queen", 3, backRow, color, team, owner),
            Create(PieceType.King, $"{prefix}_king", 4, backRow, color, team, owner),
            Create(PieceType.Bishop, $"{prefix}_bishop", 5, backRow, color, team, owner),
            Create(PieceType.Knight, $"{prefix}_knight", 6, backRow, color, team, owner),
            Create(PieceType.Rook, $"{prefix}_rook", 7, backRow, color, team, owner)
        };

        // Create pawns
        for (int i = 0; i < 8; i++)
        {
            char file = (char)('a' + i);
            pieces.Add(Create(PieceType.Pawn, $"{prefix}_pawn_{file}", i, pawnRow, color, team, owner));
        }

        // Add pieces to appropriate player list
        /* if (color == PieceColor.White)
            boardManager.playerWhite = pieces;
        else
            boardManager.playerBlack = pieces; */

        return pieces;
        // Add starting positions
        /* foreach (GameObject piece in pieces)
        {
            boardManager.AddStartingPosition(piece);
        } */
    }

    public GameObject Create(PieceType type, string name, int x, int y, PieceColor color, Team team, Player owner)
    {
        GameObject prefab = GetPrefab(type);
        if (prefab == null) return null;

        GameObject obj = Instantiate(prefab, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();
        cm.owner = owner;
        cm.color = color;
        cm.team = team;
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.startingPosition = new BoardPosition(x,y);
        
        return obj;
    }

    private GameObject GetPrefab(PieceType type)
    {
        switch (type)
        {
            case PieceType.Queen: return queen;
            case PieceType.Knight: return knight;
            case PieceType.Bishop: return bishop;
            case PieceType.King: return king;
            case PieceType.Rook: return rook;
            case PieceType.Pawn: return pawn;
            default: return null;
        }
    }
}