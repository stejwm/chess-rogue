using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private float _verticalMoveAmount =30f;
    [SerializeField] private float _moveTime = 0.1f;
    [Range(0f,2f), SerializeField] private float _scaleAmount = 1.1f;

    private Vector3 _startPos;
    private Vector3 _startScale;

    public void OnDeselect(BaseEventData eventData)
    {
        StartCoroutine(MoveElement(false));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.selectedObject = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.selectedObject = gameObject)
            eventData.selectedObject = null;    
    }

    public void OnSelect(BaseEventData eventData)
    {
        this.GetComponent<AudioSource>().Play();
        StartCoroutine(MoveElement(true));
    }

    private IEnumerator MoveElement(bool startingAnimation){
        
        Vector3 endPosition;
        Vector3 endScale;

        float elapsedTime =0f;
        while(elapsedTime <_moveTime){
            elapsedTime += Time.deltaTime;

            if(startingAnimation){
                endPosition = _startPos + new Vector3(0f, _verticalMoveAmount, 0f);
                endScale = _startScale * _scaleAmount;
                
            }else{
                endPosition = _startPos;
                endScale = _startScale;

                //transform.position = _startPos;
                //transform.localScale = _startScale;
            }

            Vector3 lerpedPos = Vector3.Lerp(transform.position, endPosition, elapsedTime/_moveTime);
            Vector3 lerpedScale = Vector3.Lerp(transform.localScale,  endScale, elapsedTime/_moveTime);

            transform.position = lerpedPos;
            transform.localScale = lerpedScale;

            yield return null;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _startPos = transform.position;
        _startScale= transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
