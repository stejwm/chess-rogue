using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityLogger : MonoBehaviour
{
    private MMF_Player feedbacks;
    [SerializeField] private GameObject PopUpMenu;
    [SerializeField] private GameObject AbilityPopUp;
    private LogManager logManager;
    bool currentlyLogging=false;
    Queue<Tuple<string, string>> queue = new Queue<Tuple<string, string>>();

    public void Initialize(LogManager logManager){
        this.logManager = logManager;
    }

    public IEnumerator HandleQueue(){
        if(currentlyLogging || queue.Count==0){
            yield break;
        }
        currentlyLogging=true;
        if (queue.Peek().Item1 != "null")
        {
            StartCoroutine(ShowAbilityAndLog(queue.Peek().Item1, queue.Peek().Item2));
            yield return new WaitForSeconds(Settings.Instance.WaitTime);
        }
        else
        {
            AddLogMessage(queue.Peek().Item2);
        }
        queue.Dequeue();
        currentlyLogging=false;
        if(queue.Count>0){
            StartCoroutine(HandleQueue());
        }
    }
    public void AddAbilityLogToQueue(string abilityName, string message)
    {
    
        queue.Enqueue(new Tuple<string, string>(abilityName, message));
        StartCoroutine(HandleQueue());
    }
    public void AddLogToQueue(string message)
    {
    
        queue.Enqueue(new Tuple<string, string>("null", message));
        StartCoroutine(HandleQueue());
    }

    private IEnumerator ShowAbilityAndLog(string abilityName, string message)
    {
        AddLogMessage(abilityName + " " + message);
        PopUpMenu.SetActive(true);
        var abilityPopUp = Instantiate(AbilityPopUp, PopUpMenu.transform);
        abilityPopUp.GetComponentInChildren<TMP_Text>().text = abilityName;
        feedbacks = abilityPopUp.GetComponentInChildren<MMF_Player>();
        // Play the FEEL feedbacks effect
        if (feedbacks != null)
            feedbacks.PlayFeedbacks();

        // Wait for the feedback effect duration
        yield return new WaitForSeconds(feedbacks.TotalDuration);
        PopUpMenu.SetActive(false);
        Destroy(abilityPopUp);
        yield return null;
    }

    private void AddLogMessage(string message)
    {
        logManager.WriteLog(message);
    }
}