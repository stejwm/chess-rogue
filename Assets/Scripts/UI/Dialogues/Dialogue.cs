using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogues/New Dialogue")]

public class Dialogue : ScriptableObject
{
    public string description;
    public Sprite sprite;
    
    public List<String> messages;
    
}
