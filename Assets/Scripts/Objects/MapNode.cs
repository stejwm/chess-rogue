using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NodeType
{
    Enemy,
    Shop,
    Encounter,
    Boss
}
public class MapNode : MonoBehaviour
{
    public string nodeName;
    public bool isCompleted;
    public MapNode[] connectedNodes;
    public NodeType nodeType; // Add this field
    public EnemyType enemyType; // Add this field for enemy nodes
    public Image nodeImage; // Reference to the Image component for changing the sprite
    


    void Start(){
        switch(nodeType){
            case NodeType.Enemy:
                nodeImage.sprite= MapManager._instance.images[Random.Range(0,MapManager._instance.images.Count)];                
                break;
            case NodeType.Shop:
                nodeImage.sprite= MapManager._instance.shopImage;
                break;
            case NodeType.Encounter:
                nodeImage.sprite= MapManager._instance.wandererImage;
                break;
            case NodeType.Boss:
                nodeImage.sprite= MapManager._instance.bossImage;
                break;
        }
        
    }


    // Method to be called when the node button is clicked
    public void OnNodeSelected()
    {
        // Handle node selection logic based on node type
        switch (nodeType)
        {
            case NodeType.Enemy:
                MapManager._instance.SelectEnemyNode(this, enemyType);
                break;
            case NodeType.Shop:
                MapManager._instance.SelectShopNode(this);
                break;
            case NodeType.Encounter:
                MapManager._instance.SelectShopNode(this);
                break;
        }
    }
}