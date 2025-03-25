using System.Collections;
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

    public void LogAbilityUsage(string abilityName, string message)
    {
        StartCoroutine(ShowAbilityAndLog(abilityName, message));
        currentlyLogging=true;
    }

    private IEnumerator ShowAbilityAndLog(string abilityName, string message)
    {
        PopUpMenu.SetActive(true);
        if (currentlyLogging)
            yield return new WaitForSeconds(Game._instance.waitTime);
        var abilityPopUp = Instantiate(AbilityPopUp, PopUpMenu.transform);
        abilityPopUp.GetComponentInChildren<TMP_Text>().text = abilityName;
        feedbacks = abilityPopUp.GetComponentInChildren<MMF_Player>();
        // Play the FEEL feedbacks effect
        if (feedbacks != null)
            feedbacks.PlayFeedbacks();

        // Wait for the feedback effect duration
        yield return new WaitForSeconds(feedbacks.TotalDuration);
        AddLogMessage(abilityName + " "+ message);
        //PopUp.gameObject.SetActive(false);
        //PopUp.transform.position = startingPosition;
        PopUpMenu.SetActive(false);
        Destroy(abilityPopUp);
        currentlyLogging=false;
        yield return null;
    }

    private void AddLogMessage(string message)
    {
        LogManager._instance.WriteLog(message);
    }
}