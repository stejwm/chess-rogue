using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Rand= System.Random;
using System.Linq;

public enum EnemyType
{
    // Enemies
    Knights,
    Fortress,
    Assassins,
    Thieves,
    Cult,
    Mob,
    RoyalFamily,
    //Bosses
    SoulKing
}

public class PieceFactory : MonoBehaviour
{
     private static Rand rng = new Rand();
    public GameObject pawn;
    public GameObject knight;
    public GameObject bishop;
    public GameObject rook;
    public GameObject queen;
    public GameObject king;
    public GameObject jester;

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

    public List<GameObject> CreateBlackPieces(Board board, Player owner)
    {
        return CreatePiecesForColor(board, owner, PieceColor.Black);
    }


    public List<GameObject> CreateKnightsOfTheRoundTable(Board board, Player owner, PieceColor color){
        int backRow = color == PieceColor.White ? 0 : 7;
        int pawnRow = color == PieceColor.White ? 1 : 6;
        List<GameObject> pieces = new List<GameObject>();

        var names = new List<string>{"Kay", "Percival", "Bedivere", "Arthur", "Lancelot", "Gawain", "Gaheris", "Gareth", "Agravain", "Mordred", "Tristan", "Palamedes"};
        for (int i=0; i<11; i++){
            if(i<6)
                if(names[i].Equals("Arthur"))
                    pieces.Add(Create(board, PieceType.King,  i+1, backRow, color, owner, $"{names[i]}"));
                else
                    pieces.Add(Create(board, PieceType.Knight,  i+1, backRow, color, owner, $"{names[i]}"));
            else{
                pieces.Add(Create(board, PieceType.Knight,  i-5, pawnRow, color, owner, $"{names[i]}"));
            }
        }
        return pieces;
    }

    public List<GameObject> CreateSoulKing(Board board, Player owner, PieceColor color){
        int backRow = color == PieceColor.White ? 0 : 7;
        int pawnRow = color == PieceColor.White ? 1 : 6;
        int frontRow = color == PieceColor.White ? 2 : 5;
        List<GameObject> pieces = new List<GameObject>();

        for (int i=0; i<8; i++){
            if(i==3)
                pieces.Add(Create(board, PieceType.Queen, i, backRow, color, owner));
            else if(i==4)
                pieces.Add(Create(board, PieceType.King, i, backRow, color, owner));

            else
                pieces.Add(Create(board, PieceType.Pawn, i, backRow, color, owner));
        }
        for (int i=2; i<6; i++){
            pieces.Add(Create(board, PieceType.Pawn, i, pawnRow, color, owner));
        }
        
        foreach (var piece in pieces){
            StartCoroutine(WaitForPieceToApplyAbility(board, piece.GetComponent<Chessman>(), AbilityDatabase._instance.GetAbilityByName("SoulBond")));
        }
        return pieces;
    }

    public List<GameObject> CreateAbilityPiecesBlack(Board board, Player owner, Ability ability){
        var pieces = CreateBlackPieces(board, owner);
        foreach (var piece in pieces){
            piece.GetComponent<Chessman>().AddAbility(board, ability);
        }
        return pieces;
    }

    public List<GameObject> CreatePiecesForColor(Board board, Player owner, PieceColor color )
    {
        string prefix = color == PieceColor.White ? "white" : "black";
        int backRow = color == PieceColor.White ? 0 : 7;
        int pawnRow = color == PieceColor.White ? 1 : 6;

        // Create back row
        List<GameObject> pieces = new List<GameObject> {
            Create(board, PieceType.Rook,  0, backRow, color, owner),
            Create(board, PieceType.Knight,  1, backRow, color, owner),
            Create(board, PieceType.Bishop,  2, backRow, color, owner),
            Create(board, PieceType.Queen,  3, backRow, color, owner),
            Create(board, PieceType.King, 4, backRow, color, owner),
            Create(board, PieceType.Bishop,  5, backRow, color, owner),
            Create(board, PieceType.Knight,  6, backRow, color, owner),
            Create(board, PieceType.Rook,  7, backRow, color, owner)
        };

        // Create pawns
        for (int i = 0; i < 8; i++)
        {
            char file = (char)('a' + i);
            pieces.Add(Create(board, PieceType.Pawn, i, pawnRow, color, owner));
        }

        return pieces;
    }

    public List<GameObject> CreateRookArmy(Board board, Player owner, PieceColor color)
    {
        string prefix = color == PieceColor.White ? "white" : "black";
        int backRow = color == PieceColor.White ? 0 : 7;
        int pawnRow = color == PieceColor.White ? 1 : 6;

        // Create back row
        List<GameObject> pieces = new List<GameObject> {
            Create(board, PieceType.Rook,  0, backRow, color,  owner),
            Create(board, PieceType.Knight,  1, backRow, color, owner),
            Create(board, PieceType.Bishop,  2, backRow, color, owner),
            Create(board, PieceType.Queen,  3, backRow, color, owner),
            Create(board, PieceType.King, 4, backRow, color, owner),
            Create(board, PieceType.Bishop,  5, backRow, color, owner),
            Create(board, PieceType.Knight,  6, backRow, color, owner),
            Create(board, PieceType.Rook,  7, backRow, color, owner)
        };

        // Create pawns
        for (int i = 0; i < 8; i++)
        {
            char file = (char)('a' + i);
            pieces.Add(Create(board, PieceType.Rook,  i, pawnRow, color, owner));
        }

        return pieces;
    }
    public List<GameObject> CreateThievesGuild(Board board, Player owner, PieceColor color)
    {
        string prefix = color == PieceColor.White ? "white" : "black";
        int backRow = color == PieceColor.White ? 0 : 7;
        int pawnRow = color == PieceColor.White ? 1 : 6;
        
        // Create back row
        List<GameObject> pieces = new List<GameObject> {
            Create(board, PieceType.Rook,  0, backRow, color, owner),
            Create(board, PieceType.Knight,  1, backRow, color, owner),
            Create(board, PieceType.Bishop,  2, backRow, color, owner),
            Create(board, PieceType.Queen,  3, backRow, color, owner),
            Create(board, PieceType.King, 4, backRow, color, owner),
            Create(board, PieceType.Bishop,  5, backRow, color, owner),
            Create(board, PieceType.Knight,  6, backRow, color, owner),
            Create(board, PieceType.Rook,  7, backRow, color, owner)
        };

        foreach (var piece in pieces)
        {
            piece.GetComponent<Chessman>().AddAbility(board, AbilityDatabase._instance.GetAbilityByName("Merchant")); //Merchant ability
        }
        // Create pawns
        for (int i = 0; i < 8; i++)
        {
            char file = (char)('a' + i);
            var pawn = Create(board, PieceType.Pawn, i, pawnRow, color, owner);
            pawn.GetComponent<Chessman>().AddAbility(board, AbilityDatabase._instance.GetAbilityByName("Pickpocket")); //Pickpocket ability
            pieces.Add(pawn);
        }

        return pieces;
    }

    public List<GameObject> CreateDarkCult(Board board, Player owner, PieceColor color)
    {
        string prefix = color == PieceColor.White ? "white" : "black";
        int backRow = color == PieceColor.White ? 0 : 7;
        int pawnRow = color == PieceColor.White ? 1 : 6;
        owner.playerCoins= UnityEngine.Random.Range(10,30);
        // Create back row
        List<GameObject> pieces = new List<GameObject> {
            Create(board, PieceType.Rook,  0, backRow, color, owner),
            Create(board, PieceType.Knight,  1, backRow, color, owner),
            Create(board, PieceType.Bishop,  2, backRow, color, owner),
            Create(board, PieceType.Queen,  3, backRow, color, owner),
            Create(board, PieceType.King, 4, backRow, color, owner),
            Create(board, PieceType.Bishop,  5, backRow, color, owner),
            Create(board, PieceType.Knight,  6, backRow, color, owner),
            Create(board, PieceType.Rook,  7, backRow, color, owner)
        };

        foreach (var piece in pieces)
        {
            piece.GetComponent<Chessman>().AddAbility(board, AbilityDatabase._instance.GetAbilityByName("BloodOffering")); //Blood offering ability
        }
        // Create pawns
        for (int i = 0; i < 8; i++)
        {
            char file = (char)('a' + i);
            var pawn = Create(board, PieceType.Pawn, i, pawnRow, color, owner);
            pawn.GetComponent<Chessman>().AddAbility(board, AbilityDatabase._instance.GetAbilityByName("Vampire")); //Vampire ability
            pieces.Add(pawn);
        }

        return pieces;
    }

    public List<GameObject> CreateAngryMob(Board board, Player owner, PieceColor color)
    {
        string prefix = color == PieceColor.White ? "white" : "black";
        int backRow = color == PieceColor.White ? 0 : 7;
        int pawnRow = color == PieceColor.White ? 1 : 6;
        int frontRow = color == PieceColor.White ? 2 : 5;
        owner.playerCoins= UnityEngine.Random.Range(10,30);
        // Create back row
        List<GameObject> pieces = new List<GameObject> {
            Create(board, PieceType.King, 4, backRow, color, owner)
        };

        foreach (var piece in pieces)
        {
            piece.GetComponent<Chessman>().AddAbility(board, AbilityDatabase._instance.GetAbilityByName("BloodOffering")); //Blood offering ability
        }
        // Create pawns
        for (int i = 0; i < 8; i++)
        {
            var pawn = Create(board, PieceType.Pawn, i, pawnRow, color, owner);
            StartCoroutine(WaitForPieceToApplyAbility(board, pawn.GetComponent<Chessman>(), AbilityDatabase._instance.GetAbilityByName("BloodThirst")));
            RandomMobAbility(board, pawn); 
            pieces.Add(pawn);
            pawn = Create(board, PieceType.Pawn, i, frontRow, color, owner);
            StartCoroutine(WaitForPieceToApplyAbility(board, pawn.GetComponent<Chessman>(), AbilityDatabase._instance.GetAbilityByName("BloodThirst")));
            RandomMobAbility(board, pawn); 
            pieces.Add(pawn);
            if(i!=4){
                pawn = Create(board, PieceType.Pawn,  i, backRow, color, owner);
                StartCoroutine(WaitForPieceToApplyAbility(board, pawn.GetComponent<Chessman>(), AbilityDatabase._instance.GetAbilityByName("BloodThirst")));
                RandomMobAbility(board, pawn); 
                pieces.Add(pawn);
            }
            
        }

        return pieces;
    }

    public List<GameObject> CreateRoyalFamily(Board board, Player owner, PieceColor color)
    {
        string prefix = color == PieceColor.White ? "white" : "black";
        int backRow = color == PieceColor.White ? 0 : 7;
        int pawnRow = color == PieceColor.White ? 1 : 6;
        owner.playerCoins= UnityEngine.Random.Range(10,30);
        // Create back row
        List<GameObject> pieces = new List<GameObject> {
            Create(board, PieceType.Queen,  3, backRow, color, owner),
            Create(board, PieceType.King,  4, backRow, color, owner),
            Create(board, PieceType.Pawn,  3, pawnRow, color, owner),
            Create(board, PieceType.Pawn,  4, pawnRow, color, owner),
        };

        foreach (var piece in pieces)
        {
            piece.GetComponent<Chessman>().AddAbility(board, AbilityDatabase._instance.GetAbilityByName("BloodOffering")); //Blood offering ability
        }
        

        return pieces;
    }

    public IEnumerator WaitForPieceToApplyAbility(Board board, Chessman piece, Ability ability){
        yield return new WaitUntil(() => piece.moveProfile!=null);
        yield return null;
        piece.AddAbility(board, ability);
    }
    public void RandomMobAbility(Board board, GameObject pieceObj){
        Chessman piece = pieceObj.GetComponent<Chessman>();
        var rand = UnityEngine.Random.Range(0,10);
        if(rand>3){
            rand = UnityEngine.Random.Range(0,4);
            switch(rand){
                case 0:
                    StartCoroutine(WaitForPieceToApplyAbility(board, piece, AbilityDatabase._instance.GetAbilityByName("AvengingStrike"))); //Avenging Strike
                    break;
                case 1:
                    StartCoroutine(WaitForPieceToApplyAbility(board, piece, AbilityDatabase._instance.GetAbilityByName("ParalyzingBlow"))); //ParalyzingBlow
                    break;
                case 2:
                    StartCoroutine(WaitForPieceToApplyAbility(board,piece, AbilityDatabase._instance.GetAbilityByName("AdamantAssault"))); //Blood thirst
                    break;
                case 3:
                    StartCoroutine(WaitForPieceToApplyAbility(board, piece, AbilityDatabase._instance.GetAbilityByName("CounterMarch"))); //Blood thirst
                    break;
                case 4:
                    //StartCoroutine(WaitForPieceToApplyAbility(piece, GameManager._instance.AllAbilities[19].Clone())); //Blood thirst
                    break;
            }
        }
    }
    public GameObject Create(Board board, PieceType type, int x, int y, PieceColor color,  Player owner, string name="")
    {
        Debug.Log("Creating piece of type: " + type + " at position: " + x + ", " + y + " for owner: " + owner.name);
        GameObject prefab = GetPrefab(type);
        if (prefab == null) return null;

        GameObject obj = Instantiate(prefab, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();
        cm.owner = owner;
        cm.color = color;
        if(string.IsNullOrEmpty(name))
            cm.name = NameDatabase.GetRandomName();
        else
            cm.name=name;
        cm.xBoard=x;
        cm.yBoard=y;
        cm.startingPosition = board.GetTileAt(x, y);
        cm.Initialize(board);
        return obj;
    }

    public GameObject CreateAbilityPiece(Board board, PieceType type, string name, int x, int y, PieceColor color, Player owner, Ability ability){
        var piece = Create(board, type, x, y, color, owner, name);
        StartCoroutine(WaitForPieceToApplyAbility(board, piece.GetComponent<Chessman>(), ability));
        return piece;
    }

    public List<GameObject> CreateOpponentPieces(Board board, Player opponent, EnemyType enemyType)
    {
        switch(enemyType)
        {
            case EnemyType.Knights:
                return CreateKnightsOfTheRoundTable(board, opponent, opponent.color);
            case EnemyType.Fortress:
                return CreateRookArmy(board, opponent, opponent.color);
            case EnemyType.Assassins:
                return CreateAbilityPiecesBlack(board, opponent, AbilityDatabase._instance.GetAbilityByName("Assassin")); //Assassin ability
            case EnemyType.Thieves:
                return CreateThievesGuild(board, opponent, opponent.color);
            case EnemyType.Cult:
                return CreateDarkCult(board, opponent, opponent.color);
            case EnemyType.Mob:
                return CreateAngryMob(board, opponent, opponent.color);
            case EnemyType.RoyalFamily:
                return CreateRoyalFamily(board, opponent, opponent.color);
            case EnemyType.SoulKing:
                return CreateSoulKing(board, opponent, opponent.color);
        }
        return null;
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
            case PieceType.Jester: return jester;
            default: return null;
        }
    }

    public GameObject CreateRandomPiece(Board board){
        Array values = Enum.GetValues(typeof(PieceType));
        System.Random random = new System.Random();
        if(random.Next(100)<16){
            return Create(board, PieceType.Jester,-1,-1,PieceColor.White,null);
        }
        PieceType randPieceType = (PieceType)values.GetValue(random.Next(values.Length-3));
        return Create(board, randPieceType,-1,-1,PieceColor.White,null);
    }

    public IEnumerator DelayedDestroy(Chessman piece){
        yield return null;
        if(piece !=null)
            piece.DestroyPiece();
    }

    public List<GameObject> LoadPieces(Board board, List<PieceData> pieces, Player player){

        var chessmen = new List<GameObject>();
        foreach (var pieceData in pieces)
        {
            var pieceObj = Create(board, pieceData.pieceType, pieceData.posX, pieceData.posY, pieceData.color, player, pieceData.name);
            var piece = pieceObj.GetComponent<Chessman>();
            piece.uniqueId = pieceData.uniqueId;

            foreach (AbilityData abilityData in pieceData.abilities){
                Ability ability = AbilityDatabase._instance.GetAbilityByName(abilityData.abilityName);
                piece.AddAbility(board, ability);
            }
            piece.attack = pieceData.attack;
            piece.defense = pieceData.defense;
            piece.support = pieceData.support;

            chessmen.Add(pieceObj);
        }

        return chessmen;
    }
}