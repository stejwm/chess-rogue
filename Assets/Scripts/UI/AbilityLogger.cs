using System.Collections;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

public class AbilityLogger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI popUpText; // Text for the pop-up
    [SerializeField] private MMF_Player feedbacks;
    [SerializeField] private GameObject PopUpMenu;
    [SerializeField] private GameObject PopUp;
    public static AbilityLogger _instance;
    private Vector3 startingPosition;

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
        startingPosition=PopUp.transform.position;
        //gameObject.SetActive(false);
    }

    public void LogAbilityUsage(string abilityName)
    {
        
        Debug.Log("Ability pop up started");
        StartCoroutine(ShowAbilityAndLog(abilityName));
    }

    private IEnumerator ShowAbilityAndLog(string abilityName)
    {
        PopUpMenu.SetActive(true);
        PopUp.gameObject.SetActive(true);
        popUpText.text = abilityName;

        // Play the FEEL feedbacks effect
        if (feedbacks != null)
            feedbacks.PlayFeedbacks();

        // Wait for the feedback effect duration
        yield return new WaitForSeconds(feedbacks.TotalDuration);
        AddLogMessage(abilityName);
        PopUp.gameObject.SetActive(false);
        PopUp.transform.position = startingPosition;
        PopUpMenu.SetActive(false);
    }

    private void AddLogMessage(string message)
    {
        LogManager._instance.WriteLog(message);
    }
}