using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class ShopStatManager : MonoBehaviour
{
    
    public static ShopStatManager _instance;
    public Text attack;
    public Text defense;
    public Text support;
    public Text info;
    public Text pieceName;
    public Image image;
    public GameObject abilityUI;
    public Chessman piece;
    public GameObject infoBox;
    [SerializeField] GameObject PopUpCanvas;

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("awake!");
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
        PopUpCanvas.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void SetAndShowStats(Chessman piece){
        PopUpCanvas.SetActive(true);
        Game._instance.isInMenu=true;
        foreach(Transform child in infoBox.transform)
        {
            Destroy(child.gameObject);
        }
        gameObject.SetActive(true);
        this.piece=piece;
        updateStats();
        StartCoroutine(SetAbilities(piece));
    }
    private void updateStats(){
        this.attack.text="attack: "+piece.attack;
        this.defense.text="defense: "+piece.defense;
        this.support.text="support: "+piece.support;
        this.info.text=piece.info;
        this.pieceName.text=piece.name;
        this.image.sprite=piece.GetComponent<SpriteRenderer>().sprite;
    }
    public IEnumerator SetAbilities(Chessman piece){
        yield return null;
        foreach (var ability in piece.abilities)
        {
            Instantiate(abilityUI, infoBox.transform);
        }
    }

    public void AttackUp(){
        Debug.Log("Adding attack, blood at "+Game._instance.hero.playerBlood);
        if (Game._instance.hero.playerBlood >=1){
            piece.attack+=1;
            Game._instance.hero.playerBlood -=1;
        }
        updateStats();
        ShopManager._instance.UpdateCurrency();
    }

    public void DefenseUp(){
        if (Game._instance.hero.playerBlood >=1){
            piece.defense+=1;
            Game._instance.hero.playerBlood -=1;
        }
        updateStats();
        ShopManager._instance.UpdateCurrency();
    }

    public void SupportUp(){
        if (Game._instance.hero.playerBlood >=1){
            piece.support+=1;
            Game._instance.hero.playerBlood -=1;
        }
        updateStats();
        ShopManager._instance.UpdateCurrency();
    }

    public void HideStats(){
        gameObject.SetActive(false);
        Game._instance.isInMenu=false;
        this.attack.text=string.Empty;
        this.defense.text=string.Empty;
        this.support.text=string.Empty;
        this.info.text=string.Empty;
        this.pieceName.text=string.Empty;
        this.image.sprite=null;
        PopUpCanvas.SetActive(false);
    }
}
