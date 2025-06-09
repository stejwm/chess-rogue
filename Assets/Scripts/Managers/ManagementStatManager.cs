using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
//using UnityEngine.UIElements;

public class ManagementStatManager : MonoBehaviour
{
    
    public static ManagementStatManager _instance;
    public TMP_Text attack;
    public TMP_Text defense;
    public TMP_Text support;
    public TMP_Text diplomacy;

    public TMP_Text captures;
    public TMP_Text captured;
    public TMP_Text bounces;
    public TMP_Text bouncings;
    public TMP_Text supportAttacks;
    public TMP_Text supportDefends;

    public TMP_Text value;
    public Text pieceName;
    public Image image;
    public GameObject abilityUI;
    public Chessman piece;
    public GameObject infoBox;
    public GameObject purchase;
    public GameObject GameStats;
    [SerializeField] GameObject PopUpCanvas;

    public int diplomacyCost=10;


    // Start is called before the first frame update
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
        Cursor.visible=true;
        
        gameObject.SetActive(false);
        //PopUpCanvas.SetActive(false);
        
    }

    public void Purchase(){
        if(GameManager._instance.hero.playerCoins>=piece.releaseCost && GameManager._instance.hero.openPositions.Count>0){
            GameManager._instance.hero.playerCoins-=piece.releaseCost;
            GameManager._instance.hero.inventoryPieces.Add(piece.gameObject);
            piece.owner=GameManager._instance.hero;
            ShopManager._instance.pieces.Remove(piece.gameObject);
            piece.gameObject.SetActive(false);
            ShopManager._instance.UpdateCurrency();
            HideStats();
        }
    }
    public void SetAndShowStats(Chessman piece){
        ShopManager._instance.toggleCardColliders();
        PopUpCanvas.SetActive(true);
        GameManager._instance.isInMenu=true;
        foreach(Transform child in infoBox.transform)
        {
            Destroy(child.gameObject);
        }
        gameObject.SetActive(true);
        this.piece=piece;
        if (piece.owner == null){
            purchase.SetActive(true);
            GameStats.SetActive(false);
        }else{
            purchase.SetActive(false);
            GameStats.SetActive(true);
        }
        updateStats();
        StartCoroutine(SetAbilities(piece));
    }
    private void updateStats(){
        this.attack.text = "<sprite name=\"sword\">: " + piece.CalculateAttack();
        this.defense.text = "<sprite name=\"shield\">: " + piece.CalculateDefense();
        this.support.text = "<sprite name=\"cross\">: " + piece.CalculateSupport();
        this.diplomacy.text = piece.diplomacy.ToString();

        this.captures.text = piece.captures.ToString();
        this.captured.text = piece.captured.ToString();
        this.bounces.text = piece.bounced.ToString();
        this.bouncings.text = piece.bouncing.ToString();
        this.supportAttacks.text = piece.supportsAttacking.ToString();
        this.supportDefends.text = piece.supportsDefending.ToString();


        this.pieceName.text=piece.name;
        this.value.text=": "+piece.releaseCost;
        this.image.sprite=piece.GetComponent<SpriteRenderer>().sprite;
    }
    public IEnumerator SetAbilities(Chessman piece){
        yield return null;
        List<Ability> multiples = new List<Ability>();
        foreach (var ability in piece.abilities)
        {
            if(multiples.Contains(ability))
                continue;

            int abilityCount = piece.abilities.Where(s=>s!=null && s.Equals(ability)).Count();
            if(abilityCount>1){
                var icon=Instantiate(abilityUI, infoBox.transform);
                icon.GetComponent<AbilityUI>().SetIcon(ability.sprite);
                icon.GetComponent<AbilityUI>().ability=ability;
                icon.GetComponentInChildren<TMP_Text>().text=$"x{abilityCount}";
                multiples.Add(ability);
            }else{
                var icon=Instantiate(abilityUI, infoBox.transform);
                icon.GetComponent<AbilityUI>().SetIcon(ability.sprite);
                icon.GetComponent<AbilityUI>().ability=ability;
                icon.GetComponentInChildren<TMP_Text>().text="";
            }
            
            
        }
    }

    public void AttackUp(){
        if (GameManager._instance.hero.playerBlood >=1){
            piece.attack+=1;
            GameManager._instance.hero.playerBlood -=1;
        }
        updateStats();
        ArmyManager._instance.UpdateCurrency();
        ShopManager._instance.UpdateCurrency();
    }

    public void DiplomacyUp(){
        if (GameManager._instance.hero.playerCoins >=10){
            piece.diplomacy+=1;
            GameManager._instance.hero.playerCoins -=10;
        }
        updateStats();
        ArmyManager._instance.UpdateCurrency();
        ShopManager._instance.UpdateCurrency();
    }

    public void DefenseUp(){
        if (GameManager._instance.hero.playerBlood >=1){
            piece.defense+=1;
            GameManager._instance.hero.playerBlood -=1;
        }
        updateStats();
        ArmyManager._instance.UpdateCurrency();
        ShopManager._instance.UpdateCurrency();
    }

    public void SupportUp(){
        if (GameManager._instance.hero.playerBlood >=1){
            piece.support+=1;
            GameManager._instance.hero.playerBlood -=1;
        }
        updateStats();
        ArmyManager._instance.UpdateCurrency();
        ShopManager._instance.UpdateCurrency();
    }

    public void HideStats(){
        gameObject.SetActive(false);
        GameManager._instance.isInMenu=false;
        this.attack.text=string.Empty;
        this.defense.text=string.Empty;
        this.support.text=string.Empty;
        this.pieceName.text=string.Empty;
        this.image.sprite=null;
        ShopManager._instance.toggleCardColliders();
        PopUpCanvas.SetActive(false);
    }

}
