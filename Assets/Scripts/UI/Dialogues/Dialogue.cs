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
    
    [System.Serializable]
    public class DialogueMessage
    {
        public string message;
        public List<DialogueOption> options;
    }
    
    public List<DialogueMessage> messages;
}
