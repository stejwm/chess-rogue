using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CI.QuickSave;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public AudioSource audioSource;
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

    public void OpenMenu(){
        gameObject.SetActive(true);
        Time.timeScale=0;
    }

    public void CloseMenu(){
        Time.timeScale=1;
        gameObject.SetActive(false);
    }

    public void StartGame(){
        SceneManager.LoadScene(1);
    }

    public void SaveGame(){

        List<MapNodeData> Map = new List<MapNodeData>();
        PlayerData playerData = new PlayerData
        {
            coins = board.Hero.playerCoins,
            blood = board.Hero.playerBlood,
            pieces = new List<PieceData>()
        };

        foreach (var pieceObj in board.Hero.pieces)
        {
            Chessman piece = pieceObj.GetComponent<Chessman>();
            PieceData pieceData = new PieceData
            {
                name= piece.name,
                pieceType = piece.type,
                attack = piece.attack,
                defense = piece.defense,
                support = piece.support,
                posX = piece.xBoard,
                posY = piece.yBoard,
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
            nodeData.connectedNodes=connectedNodes;
            Map.Add(nodeData);
        }

        string savePath = "C:\\Users\\steve\\chess-rogue\\chess-rogue\\Saves";
        QuickSaveGlobalSettings.StorageLocation = savePath;
        int total = Directory.GetFiles(savePath+"\\QuickSave").Length;
        
        var writer = QuickSaveWriter.Create("Game"+total);
            writer.Write("Player", playerData);
            writer.Write("State", board.BoardState);
            writer.Write("Level", board.Level);
            writer.Write("MapNodes", Map);
            writer.Commit();
    }

    public void QuitGame(){
        Time.timeScale=1;
        SceneManager.LoadScene(0);
    }
}
