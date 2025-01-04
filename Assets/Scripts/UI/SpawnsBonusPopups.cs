using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;


public class SpawnsBonusPopups : MonoBehaviour
{
    public static SpawnsBonusPopups Instance { get; private set; }
    
    private ObjectPool<BonusPopUp> _damageLabelPopupPool;
    
    [Header("Damage Label Popup")]
    [SerializeField] private BonusPopUp damageLabelPrefab;

    [Header("Display Setup")] 
    [Range(0.8f, 1.5f), SerializeField] public float displayLength = 1f;
    private Camera _mainCamera;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
        
        _damageLabelPopupPool = new ObjectPool<BonusPopUp>(
            () =>
            {
                BonusPopUp bonusLabel = Instantiate(damageLabelPrefab, transform);
                bonusLabel.Initialize(displayLength, this);
                return bonusLabel;
            },
            damageLabel => damageLabel.gameObject.SetActive(true),
            damageLabel => damageLabel.gameObject.SetActive(false)
        );
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _mainCamera = Camera.main;
    }
    
    public BonusPopUp BonusAdded(int damage, Vector3 position, float pitch)
    {
        // Convert world position to screen position
        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(position);

        // Convert screen position to local position in the canvas
        RectTransform canvasRect = transform as RectTransform; // Assuming this script is on the Canvas GameObject
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, 
            new Vector2(screenPosition.x, screenPosition.y), 
            _mainCamera, 
            out localPosition
        );

        // Spawn the popup at the calculated local position
        bool direction = screenPosition.x < Screen.width * 0.5f;
        return SpawnBonusPopup(damage, localPosition, direction, pitch);
    }
    
    private BonusPopUp SpawnBonusPopup(int damage, Vector3 position, bool direction, float pitch)
    {
        BonusPopUp damageLabel = _damageLabelPopupPool.Get();
        damageLabel.Display(damage, position, direction, pitch);
        return damageLabel;
    }
    
    public void ReturnDamageLabelToPool(BonusPopUp damageLabel3d)
    {
        _damageLabelPopupPool.Release(damageLabel3d);
    }
}
