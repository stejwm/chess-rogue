using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueEventRouter : MonoBehaviour
{
    public static DialogueEventRouter _instance; // Singleton

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);
    }

    [Serializable]
    public class NamedEvent
    {
        public string eventName;
        public UnityEvent unityEvent;
    }

    public List<NamedEvent> eventList = new List<NamedEvent>();

    public void TriggerEvent(string eventName)
    {
        if(String.IsNullOrEmpty(eventName))
        {
            return;
        }
        foreach (var namedEvent in eventList)
        {
            if (namedEvent.eventName == eventName)
            {
                namedEvent.unityEvent?.Invoke();
                return;
            }
        }

        Debug.LogWarning($"No event found for: {eventName}");
    }
}