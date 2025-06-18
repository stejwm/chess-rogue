using System.Collections.Generic;
using Unity.VisualScripting;
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
    public EncounterType encounterType;
    public Image nodeImage; // Reference to the Image component for changing the sprite
    private MapManager mapManager;


    public void Initialize(MapManager mapManager)
    {
        this.mapManager = mapManager;
        switch (nodeType)
            {
                case NodeType.Enemy:
                    nodeImage.sprite = mapManager.images[Random.Range(0, mapManager.images.Count)];
                    break;
                case NodeType.Shop:
                    nodeImage.sprite = mapManager.shopImage;
                    break;
                case NodeType.Encounter:
                    nodeImage.sprite = mapManager.wandererImage;
                    break;
                case NodeType.Boss:
                    nodeImage.sprite = mapManager.bossImage;
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
            case NodeType.Boss:
                mapManager.SelectEnemyNode(this, enemyType);
                break;
            case NodeType.Shop:
                mapManager.SelectShopNode(this);
                break;
            case NodeType.Encounter:
                mapManager.SelectEncounterNode(this, encounterType);
                break;
        }

        
        
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        MapNode other = (MapNode)obj;
        return nodeName == other.nodeName && 
               nodeType == other.nodeType &&
               enemyType == other.enemyType &&
               encounterType == other.encounterType;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (nodeName != null ? nodeName.GetHashCode() : 0);
            hash = hash * 23 + nodeType.GetHashCode();
            hash = hash * 23 + enemyType.GetHashCode();
            hash = hash * 23 + encounterType.GetHashCode();
            return hash;
        }
    }
}