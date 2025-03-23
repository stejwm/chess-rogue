using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Rand= System.Random;



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

    public List<GameObject> CreateBlackPieces(Player owner)
    {
        return CreatePiecesForColor(owner, PieceColor.Black, Team.Enemy);
    }


    public List<GameObject> CreateKnightsOfTheRoundTable(Player owner, PieceColor color, Team team){
        int backRow = color == PieceColor.White ? 0 : 7;
        int pawnRow = color == PieceColor.White ? 1 : 6;
        List<GameObject> pieces = new List<GameObject>();

        var names = new List<string>{"Kay", "Percival", "Bedivere", "Arthur", "Lancelot", "Gawain", "Gaheris", "Gareth", "Agravain", "Mordred", "Tristan", "Palamedes"};
        for (int i=0; i<11; i++){
            if(i<6)
                if(names[i].Equals("Arthur"))
                    pieces.Add(Create(PieceType.King, $"{names[i]}", i+1, backRow, color, team, owner));
                else
                    pieces.Add(Create(PieceType.Knight, $"{names[i]}", i+1, backRow, color, team, owner));
            else{
                pieces.Add(Create(PieceType.Knight, $"{names[i]}", i-5, pawnRow, color, team, owner));
            }
        }
        return pieces;
    }

    public List<GameObject> CreateAbilityPiecesBlack(Player owner, Ability ability){
        var pieces = CreateBlackPieces(owner);
        foreach (var piece in pieces){
            piece.GetComponent<Chessman>().AddAbility(ability.Clone());
        }
        return pieces;
    }

    public List<GameObject> CreatePiecesForColor(Player owner, PieceColor color, Team team)
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

    public List<GameObject> CreateRookArmy(Player owner, PieceColor color, Team team)
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
            pieces.Add(Create(PieceType.Rook, $"{prefix}_rook", i, pawnRow, color, team, owner));
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
    public List<GameObject> CreateThievesGuild(Player owner, PieceColor color, Team team)
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

        foreach (var piece in pieces)
        {
            piece.GetComponent<Chessman>().AddAbility(Game._instance.AllAbilities[15].Clone()); //Merchant ability
        }
        // Create pawns
        for (int i = 0; i < 8; i++)
        {
            char file = (char)('a' + i);
            var pawn = Create(PieceType.Pawn, $"{prefix}_pawn", i, pawnRow, color, team, owner);
            pawn.GetComponent<Chessman>().AddAbility(Game._instance.AllAbilities[16].Clone()); //Pickpocket ability
            pieces.Add(pawn);
        }

        return pieces;
    }

    public List<GameObject> CreateDarkCult(Player owner, PieceColor color, Team team)
    {
        string prefix = color == PieceColor.White ? "white" : "black";
        int backRow = color == PieceColor.White ? 0 : 7;
        int pawnRow = color == PieceColor.White ? 1 : 6;
        owner.playerCoins= UnityEngine.Random.Range(10,30);
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

        foreach (var piece in pieces)
        {
            piece.GetComponent<Chessman>().AddAbility(Game._instance.AllAbilities[17].Clone()); //Blood offering ability
        }
        // Create pawns
        for (int i = 0; i < 8; i++)
        {
            char file = (char)('a' + i);
            var pawn = Create(PieceType.Pawn, $"{prefix}_pawn", i, pawnRow, color, team, owner);
            pawn.GetComponent<Chessman>().AddAbility(Game._instance.AllAbilities[20].Clone()); //Vampire ability
            pieces.Add(pawn);
        }

        return pieces;
    }

    public List<GameObject> CreateAngryMob(Player owner, PieceColor color, Team team)
    {
        string prefix = color == PieceColor.White ? "white" : "black";
        int backRow = color == PieceColor.White ? 0 : 7;
        int pawnRow = color == PieceColor.White ? 1 : 6;
        int frontRow = color == PieceColor.White ? 2 : 5;
        owner.playerCoins= UnityEngine.Random.Range(10,30);
        // Create back row
        List<GameObject> pieces = new List<GameObject> {
            Create(PieceType.King, $"{prefix}_king", 4, backRow, color, team, owner)
        };

        foreach (var piece in pieces)
        {
            piece.GetComponent<Chessman>().AddAbility(Game._instance.AllAbilities[17].Clone()); //Blood offering ability
        }
        // Create pawns
        for (int i = 0; i < 8; i++)
        {
            var pawn = Create(PieceType.Pawn, $"{prefix}_pawn", i, pawnRow, color, team, owner);
            StartCoroutine(WaitForPieceToApplyAbility(pawn.GetComponent<Chessman>(), Game._instance.AllAbilities[0].Clone()));
            RandomMobAbility(pawn); 
            pieces.Add(pawn);
            pawn = Create(PieceType.Pawn, $"{prefix}_pawn", i, frontRow, color, team, owner);
            StartCoroutine(WaitForPieceToApplyAbility(pawn.GetComponent<Chessman>(), Game._instance.AllAbilities[0].Clone()));
            RandomMobAbility(pawn); 
            pieces.Add(pawn);
            if(i!=4){
                pawn = Create(PieceType.Pawn, $"{prefix}_pawn", i, backRow, color, team, owner);
                StartCoroutine(WaitForPieceToApplyAbility(pawn.GetComponent<Chessman>(), Game._instance.AllAbilities[0].Clone()));
                RandomMobAbility(pawn); 
                pieces.Add(pawn);
            }
            
        }

        return pieces;
    }

    public List<GameObject> CreateRoyalFamily(Player owner, PieceColor color, Team team)
    {
        string prefix = color == PieceColor.White ? "white" : "black";
        int backRow = color == PieceColor.White ? 0 : 7;
        int pawnRow = color == PieceColor.White ? 1 : 6;
        owner.playerCoins= UnityEngine.Random.Range(10,30);
        // Create back row
        List<GameObject> pieces = new List<GameObject> {
            Create(PieceType.Queen, $"{prefix}_queen", 3, backRow, color, team, owner),
            Create(PieceType.King, $"{prefix}_king", 4, backRow, color, team, owner),
            Create(PieceType.Pawn, $"{prefix}_pawn", 3, pawnRow, color, team, owner),
            Create(PieceType.Pawn, $"{prefix}_pawn", 4, pawnRow, color, team, owner),
        };

        foreach (var piece in pieces)
        {
            piece.GetComponent<Chessman>().AddAbility(Game._instance.AllAbilities[17].Clone()); //Blood offering ability
        }
        

        return pieces;
    }

    public IEnumerator WaitForPieceToApplyAbility(Chessman piece, Ability ability){
        yield return new WaitUntil(() => piece.moveProfile!=null);
        yield return null;
        piece.AddAbility(ability);
    }
    public void RandomMobAbility(GameObject pieceObj){
        Chessman piece = pieceObj.GetComponent<Chessman>();
        var rand = UnityEngine.Random.Range(0,10);
        if(rand>3){
            rand = UnityEngine.Random.Range(0,4);
            switch(rand){
                case 0:
                    StartCoroutine(WaitForPieceToApplyAbility(piece, Game._instance.AllAbilities[14].Clone())); //Blood thirst
                    break;
                case 1:
                    StartCoroutine(WaitForPieceToApplyAbility(piece, Game._instance.AllAbilities[8].Clone())); //Blood thirst
                    break;
                case 2:
                    StartCoroutine(WaitForPieceToApplyAbility(piece, Game._instance.AllAbilities[10].Clone())); //Blood thirst
                    break;
                case 3:
                    StartCoroutine(WaitForPieceToApplyAbility(piece, Game._instance.AllAbilities[23].Clone())); //Blood thirst
                    break;
                case 4:
                    StartCoroutine(WaitForPieceToApplyAbility(piece, Game._instance.AllAbilities[19].Clone())); //Blood thirst
                    break;
            }
        }
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
        cm.name = NameDatabase.GetRandomName();
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.startingPosition = new BoardPosition(x,y);
        
        return obj;
    }

    public GameObject CreateAbilityPiece(PieceType type, string name, int x, int y, PieceColor color, Team team, Player owner, Ability ability){
        var piece = Create(type, name, x, y, color, team, owner);
        StartCoroutine(WaitForPieceToApplyAbility(piece.GetComponent<Chessman>(), ability));
        return piece;
    }

    public List<GameObject> CreateOpponentPieces(Player opponent, EnemyType enemyType)
    {
        switch(enemyType)
        {
            case EnemyType.Knights:
                return CreateKnightsOfTheRoundTable(opponent, opponent.color, Team.Enemy);
            case EnemyType.Fortress:
                return CreateRookArmy(opponent, opponent.color, Team.Enemy);
            case EnemyType.Assassins:
                return CreateAbilityPiecesBlack(opponent, Game._instance.AllAbilities[2].Clone()); //Assassin ability
            case EnemyType.Thieves:
                return CreateThievesGuild(opponent, opponent.color, Team.Enemy);
            case EnemyType.Cult:
                return CreateDarkCult(opponent, opponent.color, Team.Enemy);
            case EnemyType.Mob:
                return CreateAngryMob(opponent, opponent.color, Team.Enemy);
            case EnemyType.RoyalFamily:
                return CreateRoyalFamily(opponent, opponent.color, Team.Enemy);
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

    public GameObject CreateRandomPiece(){
        Array values = Enum.GetValues(typeof(PieceType));
        System.Random random = new System.Random();
        if(random.Next(100)<16){
            return Create(PieceType.Jester,"pieceName",-1,-1,Game._instance.heroColor,Team.Hero,null);
        }
        PieceType randPieceType = (PieceType)values.GetValue(random.Next(values.Length-3));
        string pieceName = (Game._instance.heroColor+"_"+randPieceType).ToLower();
        return Create(randPieceType,pieceName,-1,-1,Game._instance.heroColor,Team.Hero,null);
    }

    public IEnumerator DelayedDestroy(Chessman piece){
        yield return null;
        if(piece !=null)
            Destroy(piece.gameObject);
    }
}