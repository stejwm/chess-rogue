using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class MapManager : MonoBehaviour
{
    public static MapManager _instance;

    public List<MapNode> mapNodes;
    public MapNode currentNode;
    public GameObject nodePrefab; // Prefab for the node UI element
    public Transform mapParent; // Parent transform to hold the nodes
    public GameObject linePrefab; // Prefab for the UILineRenderer

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void Start()
    {
        gameObject.SetActive(false);
        GenerateMap();
    }

    public void OpenMap()
    {
        gameObject.SetActive(true);
        Game._instance.isInMenu = true;
    }

    public void CloseMap()
    {
        Game._instance.isInMenu = false;
        Game._instance.CloseMap();
        gameObject.SetActive(false);
    }

    public void NextMatch()
    {
        CloseMap();
        Game._instance.NextMatch();
    }

    public void OpenShop()
    {
        CloseMap();
        Game._instance.OpenShop();
    }

    public void SelectNode(MapNode node)
    {
        if (node.isCompleted)
        {
            Debug.Log("Node already completed.");
            return;
        }

        currentNode = node;
        Debug.Log("Selected node: " + node.nodeName);
        // Implement logic to start the match with the selected node's enemies
    }

    private void GenerateMap()
    {
        float startX = -4.5f; // Adjust this value to start from the left side of the screen
        float xOffset = 0.96f; // Horizontal distance between nodes
        float maxYOffset = 0.5f; // Maximum vertical offset for randomness
        float controlPointOffset = 0.5f; // Offset for control points to create curves

        // Example of generating nodes procedurally at different positions
        for (int i = 0; i < 10; i++)
        {
            float yOffset = Random.Range(-maxYOffset, maxYOffset); // Random vertical offset
            Vector3 position = new Vector3(startX + i * xOffset, yOffset, 0); // Adjusted positions
            GameObject nodeObject = Instantiate(nodePrefab, position, Quaternion.identity, mapParent);
            MapNode mapNode = nodeObject.GetComponent<MapNode>();
            mapNode.nodeName = "Node " + i;
            mapNode.isCompleted = false;
            mapNodes.Add(mapNode);

            // Assign the OnClick event programmatically
            Button nodeButton = nodeObject.GetComponent<Button>();
            nodeButton.onClick.AddListener(() => mapNode.OnNodeSelected());
        }

        // Example of connecting nodes and drawing lines
        for (int i = 0; i < mapNodes.Count - 1; i++)
        {
            MapNode currentNode = mapNodes[i];
            MapNode nextNode = mapNodes[i + 1];
            currentNode.connectedNodes = new MapNode[] { nextNode };

            // Draw line between nodes using UILineRenderer
            GameObject lineObject = Instantiate(linePrefab, mapParent);
            RectTransform lineRectTransform = lineObject.GetComponent<RectTransform>();
            lineRectTransform.localPosition = Vector3.zero; // Ensure the line is positioned correctly

            UILineRenderer lineRenderer = lineObject.GetComponent<UILineRenderer>();

            // Calculate control points for the Bezier curve
            Vector2 startPoint = currentNode.transform.localPosition;
            Vector2 endPoint = nextNode.transform.localPosition;
            Vector2 controlPoint1 = startPoint + new Vector2(Random.Range(0.3f, 0.7f) * (endPoint.x - startPoint.x), Random.Range(-controlPointOffset, controlPointOffset));
            Vector2 controlPoint2 = endPoint + new Vector2(Random.Range(-0.7f, -0.3f) * (endPoint.x - startPoint.x), Random.Range(-controlPointOffset, controlPointOffset));

            // Generate points along the Bezier curve
            List<Vector2> bezierPoints = new List<Vector2>();
            int segmentCount = 20; // Number of segments for the curve
            for (int j = 0; j <= segmentCount; j++)
            {
                float t = j / (float)segmentCount;
                Vector2 point = Mathf.Pow(1 - t, 3) * startPoint +
                                3 * Mathf.Pow(1 - t, 2) * t * controlPoint1 +
                                3 * (1 - t) * Mathf.Pow(t, 2) * controlPoint2 +
                                Mathf.Pow(t, 3) * endPoint;
                bezierPoints.Add(point);
            }

            lineRenderer.Points = bezierPoints.ToArray();
        }
    }
}
