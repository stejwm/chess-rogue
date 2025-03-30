using System.Collections;
using System.Collections.Generic;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData
{
    public int coins;
    public int blood;
    public List<PieceData> pieces;
}

[System.Serializable]
public class PieceData
{
    public string name;
    public int uniqueId;
    public PieceType pieceType;  // E.g., "Knight", "Bishop"
    public int attack;
    public int defense;
    public int support;
    public PieceColor color;
    public int posX, posY;    
    public List<AbilityData> abilities;
}

[System.Serializable]
public class AbilityData
{
    public string abilityName;
    public string abilityDescription;
}

[System.Serializable]
public class MapNodeData
{
    public string nodeName;
    public bool isCompleted;
    public List<string> connectedNodes;
    public NodeType nodeType; // Add this field
    public EnemyType enemyType; // Add this field for enemy nodes
    public EncounterType encounterType;
    public float localX;
    public float localY;
}