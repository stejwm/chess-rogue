using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class BonusPopUp : MonoBehaviour
{
    [Header("Damage Label")] 
        [SerializeField] private TMP_Text bonusText;
        [SerializeField] private float fontSize = 50;
        [SerializeField] private Color fontColor = Color.green;
        
        [SerializeField] private float startColorFadeAtPercent = 0.8f;

        [Header("Animation easing")] 
        [SerializeField] private AnimationCurve easeCurve;
        private float _displayDuration;

        [Header("Bezier curve settings")] 
        [SerializeField] private Vector2 highPointOffset = new Vector2(-350, 300); 
        [SerializeField] private Vector2 lowPointOffset = new Vector2(-100, -500);
        [SerializeField] private float heightVariationMax = 150;
        [SerializeField] private float heightVariationMin = 50;
        
        private Vector3 _highPointOffsetBasedOnDirection = Vector3.zero;
        private Vector3 _dropPointOffsetBasedOnDirection = Vector3.zero;
        private bool _direction = true;
        
        [Header("Visualize")] 
        [SerializeField] private bool displayGizmos;
        [SerializeField, Range(1, 30)] private int gizmoResolution = 20;
        private Vector3 _startingPositionForVisualization = Vector3.zero;
        public AudioSource audioSource;
        
        private SpawnsBonusPopups _poolManager;

        private Coroutine _moveCoroutine;

    private void OnDrawGizmos(){
        if(!displayGizmos)
            return;
        OrientCurveBasedOnDirection();
        Vector3 start = transform.position;

        if (Application.isPlaying)
            start = _startingPositionForVisualization;

        var heighVariation = heightVariationMax-heightVariationMin;

        Vector3 highPoint = start +_highPointOffsetBasedOnDirection+ new Vector3(0,heighVariation,0);
        Vector3 dropPoint = highPoint + _dropPointOffsetBasedOnDirection;
        int colorChangeIndex = (int) (startColorFadeAtPercent*gizmoResolution);

        Gizmos.color=Color.red;
        Vector3 prevPoint=start;

        for (int i =1; i<=gizmoResolution; i++){
            float time = i/(float)gizmoResolution;
            Vector3 nextPoint = CalculateBezierPoint(time,start,highPoint, dropPoint);

            if(i>= colorChangeIndex)
                Gizmos.color = Color.yellow;

            Gizmos.DrawLine(prevPoint,nextPoint);
            prevPoint=nextPoint;
        }
    }

    private void OrientCurveBasedOnDirection()
        {
            // Reset to default values
            _highPointOffsetBasedOnDirection = highPointOffset;
            _dropPointOffsetBasedOnDirection = lowPointOffset;

            if (_direction) 
                return;
            
            _highPointOffsetBasedOnDirection.x = -_highPointOffsetBasedOnDirection.x;
            _dropPointOffsetBasedOnDirection.x = -_dropPointOffsetBasedOnDirection.x;
        }

    private Vector3 CalculateBezierPoint(float progress, Vector3 start, Vector3 control, Vector3 end){
        float remainingPath = 1-progress;
        Vector3 currentLocation = remainingPath*remainingPath*start;
        currentLocation += 2*remainingPath*progress*control;
        currentLocation +=progress*progress*end;
        return currentLocation;
    }

    public void Initialize(float displayDuration, SpawnsBonusPopups poolManager){
        _poolManager = poolManager;
        _displayDuration = displayDuration;
        //OrientCurveBasedOnDirection();
    }

    public void Display(int bonus, Vector3 objPosition, bool direction, float pitch){
        transform.position = objPosition;
        _startingPositionForVisualization=objPosition;
        _direction=direction;
        bonusText.SetText("+"+bonus);
        bonusText.color= fontColor;
        bonusText.fontSize = fontSize;
        audioSource.pitch=pitch;
        SoundManager.Instance.PlaySoundFXClip(SoundManager.Instance.pieceBonus, pitch, Settings.Instance.SfxVolume);

        if(_moveCoroutine !=null)
            StopCoroutine(_moveCoroutine);
        
        _moveCoroutine=StartCoroutine(Move());
        StartCoroutine(ReturnBonusLabelToPool(_displayDuration));

    }

    private IEnumerator Move()
        {
            
            float time = 0;
            float fadeStartTime = startColorFadeAtPercent * _displayDuration;
            //audioSource.Play();
            //OrientCurveBasedOnDirection();
            
            Vector3 start = transform.position;
            
            var heightVariation = Random.Range(heightVariationMin, heightVariationMax);
            Vector3 variation = new Vector3(0, heightVariation, 0);
            
            //Vector3 highPoint = (start + _highPointOffsetBasedOnDirection + variation);
            //Vector3 dropPoint = highPoint + _dropPointOffsetBasedOnDirection;
        
            while (time < _displayDuration)
            {
                time += Time.deltaTime;
            
                float progess = time / _displayDuration;
                float easedTime = easeCurve.Evaluate(progess);
            
                if (time > fadeStartTime)
                {
                    Color color = bonusText.color;
                    float newAlpha = Mathf.Lerp(1, 0, (time - fadeStartTime) / (_displayDuration - fadeStartTime));                    
                    color.a = newAlpha;
                    bonusText.color = color;
                }

                //Debug.Log($"Start Position: {start}");
                //Debug.Log($"High Point: {highPoint}");
                //Debug.Log($"Drop Point: {dropPoint}");
                //transform.position = CalculateBezierPoint(easedTime, start, highPoint, dropPoint);
                //transform.position = Vector3.Lerp(start, dropPoint, time / _displayDuration);
            
                yield return null;
            }
        }
        private IEnumerator ReturnBonusLabelToPool(float displayLength)
        {
            yield return new WaitForSeconds(displayLength);
            _poolManager.ReturnDamageLabelToPool(this);
        }

}
