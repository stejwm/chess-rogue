using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CI.QuickSave;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class MapManager : MonoBehaviour
{
    public List<MapNode> mapNodes;
    public MapNode currentNode;
    public GameObject nodePrefab; // Prefab for the node UI element
    public Transform mapParent; // Parent transform to hold the nodes
    public GameObject linePrefab;
    public Sprite shopSprite; 
    private Dictionary<int, MapNode> firstPathAdditionalNodes = new Dictionary<int, MapNode>();
    private Dictionary<int, MapNode> secondPathAdditionalNodes = new Dictionary<int, MapNode>();
    public List<Sprite> images;
    public Sprite wandererImage;
    public Sprite bossImage;
    public Sprite shopImage;
    private Board board;

    float startX = -4.2f * 1.5f; // Adjust this value to start from the left side of the screen
    float xOffset = 0.96f * 1.5f; // Horizontal distance between nodes
    float maxYOffset = 2.5f; // Maximum vertical offset for randomness
    float controlPointOffset = 1f; // Offset for control points to create curves
    float verticalShift = 0.0f; // Shift the nodes up
    float minVerticalDistance = 1.5f; // Minimum vertical distance between nodes from different paths


    public void Start()
    {
        //if(GameManager._instance.state != ScreenState.Map)
            gameObject.SetActive(false);
        if(!SceneLoadManager.LoadPreviousSave)
            GenerateMap();
    }

    public void OpenMap(Board board)
    {
        this.board = board;
        Debug.Log("OpeningMap");
        gameObject.SetActive(true);
        board.PauseMenuManager.SaveGame();
    }

    public void CloseMap()
    {
        //GameManager._instance.CloseMap();
        gameObject.SetActive(false);
    }

    public void NextMatch(EnemyType enemyType)
    {
        board.CreateNewMatch(enemyType);
        board.CloseMap();
    }

    public void OpenShop()
    {
        //if(GameManager._instance.shopUsed)
            return;
        //CloseMap();
        //GameManager._instance.OpenShop();
    }

    public void OpenArmyManagement()
    {
        CloseMap();
        //GameManager._instance.OpenArmyManagement();
    }

    public void SelectEnemyNode(MapNode node, EnemyType enemyType)
    {
        if (node.isCompleted || !currentNode.connectedNodes.Contains(node))
        {
            Debug.Log("Node not legal");
            return;
        }
        else{

            currentNode = node;
            NextMatch(enemyType);
            node.nodeImage.color = Color.black;

            Debug.Log("Selected enemy node: " + node.nodeName);
        }
        // Implement logic to start the match with the selected node's enemies
    }

    public void SelectShopNode(MapNode node)
    {
        if (node.isCompleted || !currentNode.connectedNodes.Contains(node))
        {
            Debug.Log("Node not legal");
            return;
        }
        else{
            Debug.Log("Selected shop node: " + node.nodeName);
            node.nodeImage.color = Color.black;
            OpenArmyManagement();
        }
    }

    public void SelectEncounterNode(MapNode node, EncounterType encounterType)
    {
        
        if (node.isCompleted || !currentNode.connectedNodes.Contains(node))
        {
            Debug.Log("Node not legal");
            return;
        }
        else{
            DialogueManager._instance.LaunchEncounterDialogue(encounterType);
            node.nodeImage.color = Color.black;
        }
    }

    private void GenerateMap()
    {
        // Create a parent GameObject for lines to ensure they are rendered behind nodes
         // Ensure linesParent is the first child of mapParent

        // Create the starting node
        Vector3 startNodePosition = new Vector3(startX - xOffset, verticalShift, 0);
        GameObject startNodeObject = Instantiate(nodePrefab, startNodePosition, Quaternion.identity, mapParent);
        MapNode startNode = startNodeObject.GetComponent<MapNode>();
        startNode.nodeName = "Start Node";
        startNode.isCompleted = false;
        mapNodes.Add(startNode);
        currentNode=startNode;
        

        // Generate the first path of nodes
        List<MapNode> firstPathNodes = new List<MapNode>();
        List<float> firstPathYOffsets = new List<float>(); // Store y-offsets for the first path
        for (int i = 0; i < 10; i++)
        {
            float yOffset = Random.Range(-maxYOffset, maxYOffset); // Random vertical offset
            firstPathYOffsets.Add(yOffset); // Store the y-offset
            Vector3 position = new Vector3(startX + i * xOffset, yOffset + verticalShift, 0); // Adjusted positions
            GameObject nodeObject = Instantiate(nodePrefab, position, Quaternion.identity, mapParent);
            MapNode mapNode = nodeObject.GetComponent<MapNode>();
            mapNode.nodeName = "Node " + i;
            mapNode.isCompleted = false;

            // Assign the node as an enemy
            mapNode.nodeType = NodeType.Enemy;
            mapNode.enemyType = GetRandomEnemyType();
            // Set the sprite based on the enemy type
            // mapNode.nodeImage.sprite = ... (assign the appropriate sprite here)

            mapNodes.Add(mapNode);
            firstPathNodes.Add(mapNode);

            // Assign the OnClick event programmatically
            Button nodeButton = nodeObject.GetComponent<Button>();
            nodeButton.onClick.AddListener(() => mapNode.OnNodeSelected());
        }

        // Generate the second path of nodes
        List<MapNode> secondPathNodes = new List<MapNode>();
        List<float> secondPathYOffsets = new List<float>(); // Store y-offsets for the second path
        for (int i = 0; i < 10; i++)
        {
            float yOffset;
            do
            {
                yOffset = Random.Range(-maxYOffset, maxYOffset); // Random vertical offset
            } while (Mathf.Abs(yOffset - firstPathYOffsets[i]) < minVerticalDistance); // Ensure minimum vertical distance

            secondPathYOffsets.Add(yOffset); // Store the y-offset
            Vector3 position = new Vector3(startX + i * xOffset, yOffset + verticalShift, 0); // Adjusted positions for the second path
            GameObject nodeObject = Instantiate(nodePrefab, position, Quaternion.identity, mapParent);
            MapNode mapNode = nodeObject.GetComponent<MapNode>();
            mapNode.nodeName = "SecondNode " + (i + 10);
            mapNode.isCompleted = false;

            // Assign the node as an enemy
            mapNode.nodeType = NodeType.Enemy;
            mapNode.enemyType = GetRandomEnemyType();
            // Set the sprite based on the enemy type
            // mapNode.nodeImage.sprite = ... (assign the appropriate sprite here)

            // Set the color of the node to black
            mapNode.nodeImage.color = new Color(69 / 255f, 69/255f, 69/255f);

            mapNodes.Add(mapNode);
            secondPathNodes.Add(mapNode);

            // Assign the OnClick event programmatically
            Button nodeButton = nodeObject.GetComponent<Button>();
            nodeButton.onClick.AddListener(() => mapNode.OnNodeSelected());
        }

        // Create additional nodes for Shop and Encounter
        NodeType[] additionalNodeTypes = { NodeType.Encounter };
        foreach (NodeType nodeType in additionalNodeTypes)
        {
            // Create one node for the first path
            int index = Random.Range(1, 10);
            while (firstPathAdditionalNodes.ContainsKey(index))
            {
                index = Random.Range(1, 10);
            }
            float yOffset = 0.0f;
            //if (Mathf.Abs(yOffset - firstPathYOffsets[index]) < minVerticalDistance || Mathf.Abs(yOffset - secondPathYOffsets[index]) < minVerticalDistance)
            //{
            float offset = firstPathYOffsets[index];
            while (!(offset <= -maxYOffset*2 || offset>=maxYOffset*2))
            {
                if(firstPathYOffsets[index]>secondPathYOffsets[index])
                    offset+=0.2f;
                else
                    offset-=0.2f;
                if (Mathf.Abs(offset - firstPathYOffsets[index]) >= minVerticalDistance && Mathf.Abs(offset - secondPathYOffsets[index]) >= minVerticalDistance)
                {
                    yOffset = offset;
                    break;
                }
            }
            //}
            Vector3 position = new Vector3(Random.Range(-.2f, 0.2f)+startX + index * xOffset, yOffset + verticalShift, 0); // Adjusted positions
            GameObject nodeObject = Instantiate(nodePrefab, position, Quaternion.identity, mapParent);
            MapNode mapNode = nodeObject.GetComponent<MapNode>();
            mapNode.nodeName = nodeType.ToString() + "First Path Node" + index;
            mapNode.isCompleted = false;
            mapNode.nodeType = nodeType;
            mapNode.encounterType = (EncounterType)Random.Range(0, System.Enum.GetValues(typeof(EncounterType)).Length);
            mapNodes.Add(mapNode);
            firstPathNodes.Add(mapNode);
            firstPathAdditionalNodes.Add(index, mapNode);
            // Assign the OnClick event programmatically
            Button nodeButton = nodeObject.GetComponent<Button>();
            nodeButton.onClick.AddListener(() => mapNode.OnNodeSelected());

            index = Random.Range(1, 10);
            while (secondPathAdditionalNodes.ContainsKey(index))
            {
                index = Random.Range(1, 10);
            }
            
            offset = secondPathYOffsets[index];
            while (!(offset <= -maxYOffset*2 || offset>=maxYOffset*2))
            {
                if(firstPathYOffsets[index]>secondPathYOffsets[index])
                    offset-=0.2f;
                else
                    offset+=0.2f;
                if (Mathf.Abs(offset - firstPathYOffsets[index]) >= minVerticalDistance && Mathf.Abs(offset - secondPathYOffsets[index]) >= minVerticalDistance)
                {
                    yOffset = offset;
                    break;
                }
            }
            
            position = new Vector3(Random.Range(-.2f, 0.2f)+startX + index * xOffset, yOffset + verticalShift, 0); // Adjusted positions
            GameObject nodeOnjectSecond = Instantiate(nodePrefab, position, Quaternion.identity, mapParent);
            MapNode mapNodeSecond = nodeOnjectSecond.GetComponent<MapNode>();
            mapNodeSecond.nodeName = nodeType.ToString() + "Second Path Node" + index;
            mapNodeSecond.isCompleted = false;
            mapNodeSecond.encounterType = (EncounterType)Random.Range(0, System.Enum.GetValues(typeof(EncounterType)).Length);
            mapNodeSecond.nodeType = nodeType;
            mapNodeSecond.nodeImage.color = new Color(69/255f, 69/255f, 69/255f);

            mapNodes.Add(mapNodeSecond);
            secondPathNodes.Add(mapNodeSecond);
            secondPathAdditionalNodes.Add(index, mapNodeSecond);

            // Assign the OnClick event programmatically
            Button NodeButtonSecond = nodeOnjectSecond.GetComponent<Button>();
            NodeButtonSecond.onClick.AddListener(() => mapNodeSecond.OnNodeSelected());
        }

        // Connect the starting node to the first nodes of each path
        startNode.connectedNodes = new MapNode[] { firstPathNodes[0], secondPathNodes[0] };

        // Create the final node
        Vector3 finalNodePosition = new Vector3(startX + 10 * xOffset, verticalShift, 0);
        GameObject finalNodeObject = Instantiate(nodePrefab, finalNodePosition, Quaternion.identity, mapParent);
        MapNode finalNode = finalNodeObject.GetComponent<MapNode>();
        finalNode.nodeName = "Final Node";
        finalNode.nodeType = NodeType.Boss;
        finalNode.enemyType = GetRandomBossType();
        finalNode.isCompleted = false;
        Button finalNodeButton = finalNodeObject.GetComponent<Button>();
        finalNodeButton.onClick.AddListener(() => finalNode.OnNodeSelected());
        mapNodes.Add(finalNode);

        // Connect the final nodes of each path to the final node
        firstPathNodes[9].connectedNodes = new MapNode[] { finalNode };
        secondPathNodes[9].connectedNodes = new MapNode[] { finalNode };

        // Connect nodes within each path and randomly between paths
        for (int i = 0; i < 10; i++)
        {
            if (i < 9)
            {
                firstPathNodes[i].connectedNodes = new MapNode[] { firstPathNodes[i + 1] };
                secondPathNodes[i].connectedNodes = new MapNode[] { secondPathNodes[i + 1] };
            }
            if(secondPathAdditionalNodes.ContainsKey(i))
            {
                List<MapNode> secondPathConnections = new List<MapNode>(secondPathNodes[i].connectedNodes);
                secondPathConnections.Add(secondPathAdditionalNodes[i]);
                secondPathNodes[i].connectedNodes = secondPathConnections.ToArray();
            }
            if(firstPathAdditionalNodes.ContainsKey(i))
            {
                List<MapNode> firstPathConnections = new List<MapNode>(firstPathNodes[i].connectedNodes);
                firstPathConnections.Add(firstPathAdditionalNodes[i]);
                firstPathNodes[i].connectedNodes = firstPathConnections.ToArray();
            }

            // Random chance to connect to the next node in the other path
            if (Random.value < 0.3f && i !=9) // 30% chance to connect to the next node in the other path
            {
                List<MapNode> firstPathConnections = new List<MapNode>(firstPathNodes[i].connectedNodes);
                firstPathConnections.Add(secondPathNodes[i+1]);
                firstPathNodes[i].connectedNodes = firstPathConnections.ToArray();
            }
            if (Random.value < 0.3f && i !=9) // 30% chance to connect to the next node in the other path
            {
                List<MapNode> secondPathConnections = new List<MapNode>(secondPathNodes[i].connectedNodes);
                secondPathConnections.Add(firstPathNodes[i+1]);
                secondPathNodes[i].connectedNodes = secondPathConnections.ToArray();
            }
        }



        InitializeNodes();
        DrawPaths();
        
    }

    public void InitializeNodes()
    {
        foreach (MapNode node in mapNodes)
        {
            node.Initialize(this);
        }
    }

    public void LoadMap(List<MapNodeData> mapNodeData)
    {
        List<MapNode> nodes = new List<MapNode>();
        List<MapNode> connectedNodes = new List<MapNode>();
        Dictionary<MapNodeData, MapNode> mappedNodes = new Dictionary<MapNodeData, MapNode>();
        foreach (var nodeData in mapNodeData)
        {
            GameObject nodeObject = Instantiate(nodePrefab, new Vector3(0, 0, 0), Quaternion.identity, mapParent);
            MapNode mapNode = nodeObject.GetComponent<MapNode>();
            mapNode.encounterType = nodeData.encounterType;
            mapNode.enemyType = nodeData.enemyType;
            mapNode.nodeType = nodeData.nodeType;
            mapNode.isCompleted = nodeData.isCompleted;
            mapNode.nodeName = nodeData.nodeName;
            mapNode.transform.localPosition = new Vector3(nodeData.localX, nodeData.localY);
            mapNode.nodeImage.color = nodeData.color;
            Button nodeButton = mapNode.GetComponent<Button>();
            nodeButton.onClick.AddListener(() => mapNode.OnNodeSelected());
            mappedNodes.Add(nodeData, mapNode);
            nodes.Add(mapNode);

            if (mapNode.isCompleted)
                mapNode.nodeImage.color = Color.black;

        }
        foreach (var nodeData in mapNodeData)
        {
            //Debug.Log("MapNodeData Count: " + mapNodeData.Count);
            connectedNodes.Clear();
            foreach (string nodeName in nodeData.connectedNodes)
            {
                var matchingNode = nodes.FirstOrDefault(n => n.nodeName == nodeName);
                if (matchingNode != null)
                    connectedNodes.Add(matchingNode);
            }

            if (nodeData.isCurrentNode)
                currentNode = mappedNodes[nodeData];

            if (nodeData.nodeName.Contains("Second"))
                mappedNodes[nodeData].nodeImage.color = new Color(69 / 255f, 69 / 255f, 69 / 255f);


            mappedNodes[nodeData].connectedNodes = connectedNodes.ToArray();
        }

        this.mapNodes = mappedNodes.Values.ToList();
        InitializeNodes();
        DrawPaths();
    }

    public void DrawPaths(){
        GameObject linesParent = new GameObject("LinesParent");
        linesParent.transform.SetParent(mapParent, false);
        linesParent.transform.SetAsFirstSibling();
        foreach (MapNode node in mapNodes)
        {
            foreach (MapNode connectedNode in node.connectedNodes)
            {
                // Draw line between nodes using UILineRenderer
                GameObject lineObject = Instantiate(linePrefab, linesParent.transform);
                RectTransform lineRectTransform = lineObject.GetComponent<RectTransform>();
                lineRectTransform.localPosition = Vector3.zero; // Ensure the line is positioned correctly

                UILineRenderer lineRenderer = lineObject.GetComponent<UILineRenderer>();

                // Set the color of the line based on the path
                /* if (firstPathNodes.Contains(node) || firstPathNodes.Contains(connectedNode))
                {
                    lineRenderer.color = Color.white;
                }
                else if (secondPathNodes.Contains(node) || secondPathNodes.Contains(connectedNode))
                {
                    lineRenderer.color = Color.grey;
                } */

                // Calculate control points for the Bezier curve
                Vector2 startPoint = node.transform.localPosition;
                Vector2 endPoint = connectedNode.transform.localPosition;
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

    public EnemyType GetRandomBossType(){
        int enemyAmount=7;
        return (EnemyType)Random.Range(enemyAmount, System.Enum.GetValues(typeof(EnemyType)).Length);
        
    }
    public EnemyType GetRandomEnemyType(){
        int bossAmount=1;
        return (EnemyType)Random.Range(0, System.Enum.GetValues(typeof(EnemyType)).Length- bossAmount);
    }
}
