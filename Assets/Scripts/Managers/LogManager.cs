using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LogManager : MonoBehaviour
{
    public static LogManager _instance;
    public TMP_Text LogText;
    [SerializeField] private ScrollRect scrollRect;
    public GameObject abilityTarget;

    public void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f; // Scrolls to the bottom
        Canvas.ForceUpdateCanvases(); // Ensure the UI updates immediately
    }

    void Awake()
    {
        
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }
    public void WriteLog(string message)
    {
        ScrollToBottom();
        LogText.text += message + "\n";
        ScrollToBottom();
        LogText.text +="<color=#FFFFFF>--------------------------</color>\n";
        if(abilityTarget.transform.localPosition.y>-380)
            abilityTarget.transform.localPosition = new Vector2 (abilityTarget.transform.localPosition.x, abilityTarget.transform.localPosition.y-70);
    }
    public void ClearLogs()
    {
        LogText.text ="";
    }
    public Vector3 GetTextBottomPosition()
    {
        RectTransform rectTransform = LogText.rectTransform;

        // Get the position of the text object's bottom in world space
        Vector3 worldBottom = rectTransform.position - new Vector3(0, rectTransform.rect.height / 2, 0);

        // Optionally, convert world space position to local position if you need it relative to a parent object
        Vector3 localBottom = rectTransform.InverseTransformPoint(worldBottom);

        return localBottom;
    }

}
