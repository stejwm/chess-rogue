using System.Collections;
using System.Collections.Generic;
using CI.QuickSave;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager _instance;
    public AudioSource audioSource;
    void Awake()
    {
        
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }

    // Update is called once per frame
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenMenu(){
        gameObject.SetActive(true);
        Time.timeScale=0;
    }

    public void CloseMenu(){
        Time.timeScale=1;
        Game._instance.isInMenu=false;
        gameObject.SetActive(false);
    }

    public void StartGame(){
        SceneManager.LoadScene(1);
    }

    public void SaveGame(){

        List<MapNodeData> Map = new List<MapNodeData>();
        PlayerData playerData = new PlayerData
        {
            coins = Game._instance.hero.playerCoins,
            blood = Game._instance.hero.playerBlood,
            pieces = new List<PieceData>()
        };

        foreach (var pieceObj in Game._instance.hero.pieces)
        {
            Chessman piece = pieceObj.GetComponent<Chessman>();
            PieceData pieceData = new PieceData
            {
                name= piece.name,
                uniqueId = -piece.uniqueId,
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

        foreach (var nodeObj in MapManager._instance.mapNodes)
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
                isCurrentNode = MapManager._instance.currentNode == node,
                color = node.nodeImage.color
            };

            foreach (var connectedNodeObj in node.connectedNodes)
            {
                connectedNodes.Add(connectedNodeObj.GetComponent<MapNode>().nodeName);
            }
            nodeData.connectedNodes=connectedNodes;
            Map.Add(nodeData);
        }

        
        
        var writer = QuickSaveWriter.Create("Game");
            writer.Write("Player", playerData);
            writer.Write("State", Game._instance.state);
            writer.Write("Level", Game._instance.level);
            writer.Write("Shop", Game._instance.shopUsed);
            writer.Write("MapNodes", Map);
            writer.Commit();
    }

    public void QuitGame(){
        Time.timeScale=1;
        SceneManager.LoadScene(0);
    }
}
