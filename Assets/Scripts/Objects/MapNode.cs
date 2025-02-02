using UnityEngine;

public class MapNode : MonoBehaviour
{
    public string nodeName;
    public bool isCompleted;
    public MapNode[] connectedNodes;

    // Method to be called when the node button is clicked
    public void OnNodeSelected()
    {
        // Handle node selection logic
        MapManager._instance.SelectNode(this);
    }
}