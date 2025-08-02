using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CI.QuickSave;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuOptions;
    [SerializeField] private GameObject tutorialOptions;
    [SerializeField] private Toggle tutorialToggle;
    [SerializeField] private GameObject settingsOptions;
    [SerializeField] private TMP_Text enemiesCapturedText;
    [SerializeField] private TMP_Text enemiesBouncedText;
    [SerializeField] private TMP_Text enemiesDecimatedText;
    [SerializeField] private TMP_Text enemiesKilledText;
    [SerializeField] private TMP_Text enemiesReleasedText;
    [SerializeField] private TMP_Text enemiesAbandonedText;
    [SerializeField] private TMP_Text myPieceCapturedText;
    [SerializeField] private TMP_Text myPieceBouncedText;
    [SerializeField] private TMP_Text myPieceDecimatedText;
    [SerializeField] private TMP_Text myPieceKilledText;
    [SerializeField] private TMP_Text myPieceReleasedText;
    [SerializeField] private TMP_Text myPieceAbandonedText;

    private Board board;

    void Start()
    {
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    public void Initialize(Board board)
    {
        this.board = board;
    }

    public void OpenMenu()
    {
        menuOptions.SetActive(true);
        tutorialOptions.SetActive(false);
        settingsOptions.SetActive(false);
        gameObject.SetActive(true);
        enemiesCapturedText.text = board.Hero.enemiesCaptured.ToString();
        enemiesBouncedText.text = board.Hero.enemiesBounced.ToString();
        enemiesDecimatedText.text = board.Hero.enemiesDecimated.ToString();
        enemiesKilledText.text = board.Hero.enemiesKilled.ToString();
        enemiesReleasedText.text = board.Hero.enemiesReleased.ToString();
        enemiesAbandonedText.text = board.Hero.enemiesAbandoned.ToString();
        myPieceCapturedText.text = board.Hero.myPieceCaptured.ToString();
        myPieceBouncedText.text = board.Hero.myPieceBounced.ToString();
        myPieceDecimatedText.text = board.Hero.myPieceDecimated.ToString();
        myPieceKilledText.text = board.Hero.myPieceKilled.ToString();
        myPieceReleasedText.text = board.Hero.myPieceReleased.ToString();
        myPieceAbandonedText.text = board.Hero.myPieceAbandoned.ToString();
        Time.timeScale = 0;
    }

    public void CloseMenu()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        SceneLoadManager.LoadPreviousSave = false;
        SceneManager.LoadScene(1);
    }

    public void SaveGame()
    {

        List<MapNodeData> Map = new List<MapNodeData>();
        List<OrderData> Orders = new List<OrderData>();
        PlayerData playerData = new PlayerData
        {
            coins = board.Hero.playerCoins,
            blood = board.Hero.playerBlood,
            piecesCaptured = board.Hero.myPieceCaptured,
            piecesBounced = board.Hero.myPieceBounced,
            piecesDecimated = board.Hero.myPieceDecimated,
            piecesKilled = board.Hero.myPieceKilled,
            piecesReleased = board.Hero.myPieceReleased,
            piecesAbandoned = board.Hero.myPieceAbandoned,
            enemiesCaptured = board.Hero.enemiesCaptured,
            enemiesBounced = board.Hero.enemiesBounced,
            enemiesDecimated = board.Hero.enemiesDecimated,
            enemiesKilled = board.Hero.enemiesKilled,
            enemiesReleased = board.Hero.enemiesReleased,
            enemiesAbandoned = board.Hero.enemiesAbandoned,
            pieces = new List<PieceData>()
        };

        foreach (var pieceObj in board.Hero.pieces)
        {
            Chessman piece = pieceObj.GetComponent<Chessman>();
            PieceData pieceData = new PieceData
            {
                name = piece.name,
                pieceType = piece.type,
                attack = piece.attack,
                defense = piece.defense,
                support = piece.support,
                posX = piece.xBoard,
                posY = piece.yBoard,
                uniqueId = piece.uniqueId,
                abilities = new List<AbilityData>()
            };

            foreach (Ability ability in piece.abilities)  // Assuming `abilities` is a List<Ability>
            {
                pieceData.abilities.Add(new AbilityData
                {
                    abilityName = ability.abilityName,  // Use a unique identifier for each ability
                    abilityDescription = ability.description    // Store any relevant data
                });
            }
            playerData.pieces.Add(pieceData);
        }

        foreach (var nodeObj in board.MapManager.mapNodes)
        {
            var connectedNodes = new List<string>();
            MapNode node = nodeObj.GetComponent<MapNode>();
            MapNodeData nodeData = new MapNodeData
            {
                nodeName = node.nodeName,
                isCompleted = node.isCompleted,
                nodeType = node.nodeType, // Add this field
                enemyType = node.enemyType, // Add this field for enemy nodes
                encounterType = node.encounterType,
                localX = node.transform.localPosition.x,
                localY = node.transform.localPosition.y,
                isCurrentNode = board.MapManager.currentNode == node,
                color = node.nodeImage.color
            };

            foreach (var connectedNodeObj in node.connectedNodes)
            {
                connectedNodes.Add(connectedNodeObj.GetComponent<MapNode>().nodeName);
            }
            nodeData.connectedNodes = connectedNodes;
            Map.Add(nodeData);
        }

        foreach (var order in board.Hero.orders)
        {
            OrderData orderData = new OrderData
            {
                orderName = order.Name
            };
            Orders.Add(orderData);
        }
        //string savePath = "C:\\Users\\steve\\chess-rogue\\chess-rogue\\Saves";
        //QuickSaveGlobalSettings.StorageLocation = savePath;
        //int total = Directory.GetFiles(QuickSave).Length;

        var writer = QuickSaveWriter.Create("Game");
        writer.Write("Player", playerData);
        writer.Write("State", board.BoardState);
        writer.Write("Level", board.Level);
        writer.Write("MapNodes", Map);
        writer.Write("Orders", Orders);
        writer.Write("CommonRarity", board.Hero.RarityWeights[Rarity.Common]);
        writer.Write("UncommonRarity", board.Hero.RarityWeights[Rarity.Uncommon]);
        writer.Write("RareRarity", board.Hero.RarityWeights[Rarity.Rare]);
        writer.Commit();
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        Application.Quit();
    }

    public void ToggleTutorials(bool toggle)
    {
        Settings.Instance.Tutorial = toggle;
        board.SavePrefs();
    }
    public void OpenSettings()
    {
        menuOptions.SetActive(false);
        tutorialOptions.SetActive(false);
        settingsOptions.SetActive(true);
        tutorialToggle.isOn = Settings.Instance.Tutorial;
    }
    public void OpenTutorials()
    {
        menuOptions.SetActive(false);
        tutorialOptions.SetActive(true);
        settingsOptions.SetActive(false);
    }
    public void SetSfxVolume(float volume)
    {
        Settings.Instance.SfxVolume = volume;
        board.SavePrefs();
    }

    public void SetStickSpeed(float speed)
    {
        Settings.Instance.JoystickSpeed = speed;
        board.SavePrefs();
    }
}
