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
    public static AbilityLogger _instance;
    bool currentlyLogging=false;
    Queue<Tuple<string, string>> queue = new Queue<Tuple<string, string>>();

    void Awake()
    {
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }

    void Start(){
        //startingPosition=PopUp.transform.position;
        //gameObject.SetActive(false);
    }

    public IEnumerator HandleQueue(){
        if(currentlyLogging || queue.Count==0){
            yield break;
        }
        currentlyLogging=true;
        StartCoroutine(ShowAbilityAndLog(queue.Peek().Item1, queue.Peek().Item2));
        yield return new WaitForSeconds(Settings.Instance.WaitTime);
        queue.Dequeue();
        currentlyLogging=false;
        if(queue.Count>0){
            StartCoroutine(HandleQueue());
        }
    }
    public void AddLogToQueue(string abilityName, string message)
    {
    
        queue.Enqueue(new Tuple<string, string>(abilityName, message));
        StartCoroutine(HandleQueue());
    }

    private IEnumerator ShowAbilityAndLog(string abilityName, string message)
    {
        PopUpMenu.SetActive(true);
        var abilityPopUp = Instantiate(AbilityPopUp, PopUpMenu.transform);
        abilityPopUp.GetComponentInChildren<TMP_Text>().text = abilityName;
        feedbacks = abilityPopUp.GetComponentInChildren<MMF_Player>();
        // Play the FEEL feedbacks effect
        if (feedbacks != null)
            feedbacks.PlayFeedbacks();

        // Wait for the feedback effect duration
        yield return new WaitForSeconds(feedbacks.TotalDuration);
        AddLogMessage(abilityName + " "+ message);
        PopUpMenu.SetActive(false);
        Destroy(abilityPopUp);
        yield return null;
    }

    private void AddLogMessage(string message)
    {
        //LogManager._instance.WriteLog(message);
    }
}