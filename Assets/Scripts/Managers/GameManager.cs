/* using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System.Linq;
using Rand= System.Random;
using MoreMountains.Feedbacks;
using System.Text.RegularExpressions;
using CI.QuickSave;




public class GameManager : MonoBehaviour
{
    public Player hero;
    //public AIPlayer white;
    public Player opponent;
    [SerializeField] private Board board;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private KingsOrderManager kingsOrderManager;

    public GameObject card;
    public PieceColor heroColor;
    public AudioSource audioSource;
    public AudioClip capture;
    public AudioClip bounce;
    public AudioClip move;
    public AudioClip ability;

    public int level = 0;
    private static Rand rng = new Rand();
    private List<GameObject> cards = new List<GameObject>();
    public bool shopUsed = false;
    

    public void Start()
    {
        

    }
    public void Update()
    {
         if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenuManager._instance.OpenMenu();
        } 
    }
    public void LetsBegin()
    {
        heroColor = PieceColor.White;
        opponent.pieces = PieceFactory._instance.CreateKnightsOfTheRoundTable(board, opponent, opponent.color);
        hero.pieces = PieceFactory._instance.CreatePiecesForColor(board, hero, hero.color);
        hero.Initialize();
        opponent.Initialize();
        board.CreateNewMatch(hero, opponent);
        board.BoardState= BoardState.ActiveMatch; 
    }
    
    

    public void OpenReward()
    {
        board.ResetPlayerPieces();
        board.BoardState= BoardState.RewardScreen;
        InventoryManager._instance.OpenInventory();
    }
    public void CloseReward()
    {
        board.BoardState= BoardState.MainGameboard;
        OpenShop();
    } 
    public void OpenShop()
    {
        board.BoardState= BoardState.ShopScreen;
        //ShopManager._instance.OpenShop();
        shopUsed = true;
    }
    public void CloseShop()
    {
        board.BoardState= BoardState.ShopScreen;
        OpenMap();
    }
    public void OpenMap()
    {
        //KingsOrderManager._instance.Hide();
        //MapManager._instance.OpenMap();
        this.board.BoardState= BoardState.Map;
    }
    public void CloseMap()
    {
        board.BoardState = BoardState.None;
    }
    public void NextMatch(EnemyType enemyType)
    {
        level++;
        //state=BoardState.ActiveMatch;
        shopUsed = false;
        opponent.DestroyPieces();
        opponent.pieces = PieceFactory._instance.CreateOpponentPieces(board, opponent, enemyType);
        opponent.Initialize();
        opponent.LevelUp(level, enemyType);
        board.CreateNewMatch(hero, opponent);
    }

    public void OpenArmyManagement()
    {
        board.BoardState= BoardState.ManagementScreen;
        //ArmyManager._instance.OpenShop();
    }

    public void CloseArmyManagement()
    {
        //ResetPlayerPieces();
        board.BoardState= BoardState.ShopScreen; ;
        //ShopManager._instance.UnHideShop();
    }
    

}
 */