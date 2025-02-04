using UnityEngine;
using UnityEngine.UI;

public enum NodeType
{
    Enemy,
    Shop,
    Encounter
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
                MapManager._instance.SelectEncounterNode(this);
                break;
        }
    }
}