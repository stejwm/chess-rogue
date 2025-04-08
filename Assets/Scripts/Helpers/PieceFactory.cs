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
                    pieces.Add(Create(PieceType.King,  i+1, backRow, color, team, owner, $"{names[i]}"));
                else
                    pieces.Add(Create(PieceType.Knight,  i+1, backRow, color, team, owner, $"{names[i]}"));
            else{
                pieces.Add(Create(PieceType.Knight,  i-5, pawnRow, color, team, owner, $"{names[i]}"));
            }
        }
        return pieces;
    }

    public List<GameObject> CreateSoulKing(Player owner, PieceColor color, Team team){
        int backRow = color == PieceColor.White ? 0 : 7;
        int pawnRow = color == PieceColor.White ? 1 : 6;
        int frontRow = color == PieceColor.White ? 2 : 5;
        List<GameObject> pieces = new List<GameObject>();

        for (int i=0; i<8; i++){
            if(i==3)
                pieces.Add(Create(PieceType.Queen, i, backRow, color, team, owner));
            else if(i==4)
                pieces.Add(Create(PieceType.King, i, backRow, color, team, owner));

            else
                pieces.Add(Create(PieceType.Pawn, i, backRow, color, team, owner));
        }
        for (int i=2; i<6; i++){
            pieces.Add(Create(PieceType.Pawn, i, pawnRow, color, team, owner));
        }
        
        foreach (var piece in pieces){
            StartCoroutine(WaitForPieceToApplyAbility(piece.GetComponent<Chessman>(), Game._instance.AllAbilities[27].Clone()));
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
            Create(PieceType.Rook,  0, backRow, color, team, owner),
            Create(PieceType.Knight,  1, backRow, color, team, owner),
            Create(PieceType.Bishop,  2, backRow, color, team, owner),
            Create(PieceType.Queen,  3, backRow, color, team, owner),
            Create(PieceType.King, 4, backRow, color, team, owner),
            Create(PieceType.Bishop,  5, backRow, color, team, owner),
            Create(PieceType.Knight,  6, backRow, color, team, owner),
            Create(PieceType.Rook,  7, backRow, color, team, owner)
        };

        // Create pawns
        for (int i = 0; i < 8; i++)
        {
            char file = (char)('a' + i);
            pieces.Add(Create(PieceType.Pawn, i, pawnRow, color, team, owner));
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
            Create(PieceType.Rook,  0, backRow, color, team, owner),
            Create(PieceType.Knight,  1, backRow, color, team, owner),
            Create(PieceType.Bishop,  2, backRow, color, team, owner),
            Create(PieceType.Queen,  3, backRow, color, team, owner),
            Create(PieceType.King, 4, backRow, color, team, owner),
            Create(PieceType.Bishop,  5, backRow, color, team, owner),
            Create(PieceType.Knight,  6, backRow, color, team, owner),
            Create(PieceType.Rook,  7, backRow, color, team, owner)
        };

        // Create pawns
        for (int i = 0; i < 8; i++)
        {
            char file = (char)('a' + i);
            pieces.Add(Create(PieceType.Rook,  i, pawnRow, color, team, owner));
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
            Create(PieceType.Rook,  0, backRow, color, team, owner),
            Create(PieceType.Knight,  1, backRow, color, team, owner),
            Create(PieceType.Bishop,  2, backRow, color, team, owner),
            Create(PieceType.Queen,  3, backRow, color, team, owner),
            Create(PieceType.King, 4, backRow, color, team, owner),
            Create(PieceType.Bishop,  5, backRow, color, team, owner),
            Create(PieceType.Knight,  6, backRow, color, team, owner),
            Create(PieceType.Rook,  7, backRow, color, team, owner)
        };

        foreach (var piece in pieces)
        {
            piece.GetComponent<Chessman>().AddAbility(Game._instance.AllAbilities[15].Clone()); //Merchant ability
        }
        // Create pawns
        for (int i = 0; i < 8; i++)
        {
            char file = (char)('a' + i);
            var pawn = Create(PieceType.Pawn, i, pawnRow, color, team, owner);
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
            Create(PieceType.Rook,  0, backRow, color, team, owner),
            Create(PieceType.Knight,  1, backRow, color, team, owner),
            Create(PieceType.Bishop,  2, backRow, color, team, owner),
            Create(PieceType.Queen,  3, backRow, color, team, owner),
            Create(PieceType.King, 4, backRow, color, team, owner),
            Create(PieceType.Bishop,  5, backRow, color, team, owner),
            Create(PieceType.Knight,  6, backRow, color, team, owner),
            Create(PieceType.Rook,  7, backRow, color, team, owner)
        };

        foreach (var piece in pieces)
        {
            piece.GetComponent<Chessman>().AddAbility(Game._instance.AllAbilities[17].Clone()); //Blood offering ability
        }
        // Create pawns
        for (int i = 0; i < 8; i++)
        {
            char file = (char)('a' + i);
            var pawn = Create(PieceType.Pawn, i, pawnRow, color, team, owner);
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
            Create(PieceType.King, 4, backRow, color, team, owner)
        };

        foreach (var piece in pieces)
        {
            piece.GetComponent<Chessman>().AddAbility(Game._instance.AllAbilities[17].Clone()); //Blood offering ability
        }
        // Create pawns
        for (int i = 0; i < 8; i++)
        {
            var pawn = Create(PieceType.Pawn, i, pawnRow, color, team, owner);
            StartCoroutine(WaitForPieceToApplyAbility(pawn.GetComponent<Chessman>(), Game._instance.AllAbilities[0].Clone()));
            RandomMobAbility(pawn); 
            pieces.Add(pawn);
            pawn = Create(PieceType.Pawn, i, frontRow, color, team, owner);
            StartCoroutine(WaitForPieceToApplyAbility(pawn.GetComponent<Chessman>(), Game._instance.AllAbilities[0].Clone()));
            RandomMobAbility(pawn); 
            pieces.Add(pawn);
            if(i!=4){
                pawn = Create(PieceType.Pawn,  i, backRow, color, team, owner);
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
            Create(PieceType.Queen,  3, backRow, color, team, owner),
            Create(PieceType.King,  4, backRow, color, team, owner),
            Create(PieceType.Pawn,  3, pawnRow, color, team, owner),
            Create(PieceType.Pawn,  4, pawnRow, color, team, owner),
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
    public GameObject Create(PieceType type, int x, int y, PieceColor color, Team team, Player owner, string name="")
    {
        GameObject prefab = GetPrefab(type);
        if (prefab == null) return null;

        GameObject obj = Instantiate(prefab, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();
        cm.owner = owner;
        cm.color = color;
        cm.team = team;
        if(string.IsNullOrEmpty(name))
            cm.name = NameDatabase.GetRandomName();
        else
            cm.name=name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.startingPosition = new BoardPosition(x,y);
        
        return obj;
    }

    public GameObject CreateAbilityPiece(PieceType type, string name, int x, int y, PieceColor color, Team team, Player owner, Ability ability){
        var piece = Create(type, x, y, color, team, owner);
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
            case EnemyType.SoulKing:
                return CreateSoulKing(opponent, opponent.color, Team.Enemy);
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
            return Create(PieceType.Jester,-1,-1,Game._instance.heroColor,Team.Hero,null);
        }
        PieceType randPieceType = (PieceType)values.GetValue(random.Next(values.Length-3));
        return Create(randPieceType,-1,-1,Game._instance.heroColor,Team.Hero,null);
    }

    public IEnumerator DelayedDestroy(Chessman piece){
        yield return null;
        if(piece !=null)
            piece.DestroyPiece();
    }

    public List<GameObject> LoadPieces(List<PieceData> pieces){

        var chessmen = new List<GameObject>();
        foreach (var pieceData in pieces)
        {
            var pieceObj = Create(pieceData.pieceType, pieceData.posX, pieceData.posY, pieceData.color, Team.Hero, Game._instance.hero, pieceData.name);
            var piece = pieceObj.GetComponent<Chessman>();
            piece.uniqueId = pieceData.uniqueId;

            foreach (AbilityData abilityData in pieceData.abilities){
                Ability ability = Game._instance.AllAbilities.FirstOrDefault(a => a.abilityName==abilityData.abilityName);
                piece.AddAbility(ability);
            }

            chessmen.Add(pieceObj);
        }

        return chessmen;
    }
}