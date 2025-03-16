using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    
    public string eventName; 
    public Dialogue nextDialogue;
}