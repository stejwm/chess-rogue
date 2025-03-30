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
    public string pieceType;  // E.g., "Knight", "Bishop"
    public int attack;
    public int defense;
    public int support;
    public int posX, posY;    
    public List<AbilityData> abilities;
}

[System.Serializable]
public class AbilityData
{
    public string abilityName;
    public string abilityDescription;
}