using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class StatBoxManager : MonoBehaviour
{
    
    public static StatBoxManager _instance;
    public Text attack;
    public Text defense;
    public Text support;
    public Text pieceName;
    public GameObject abilityBox;
    public GameObject abilityUI;
    public Image image;
    public bool lockView;
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
    }

    public void LockView(){
        lockView=true;
    }
    public void UnlockView(){
        lockView=false;
    }
    public void SetAndShowStats(Chessman piece){
        if(!lockView)
        {
            foreach (Transform child in abilityBox.transform)
            {
                Destroy(child.gameObject);
            }
            gameObject.SetActive(true);
            this.attack.text="attack: "+piece.CalculateAttack();
            this.defense.text="defense: "+piece.CalculateDefense();
            this.support.text="support: "+piece.CalculateSupport();
            
            this.pieceName.text=piece.name;
            this.image.sprite=piece.GetComponent<SpriteRenderer>().sprite;
            foreach (var ability in piece.abilities)
            {
                var icon=Instantiate(abilityUI, abilityBox.transform);
                icon.GetComponent<AbilityUI>().SetIcon(ability.sprite);
            }
        }

    }

    public void HideStats(){
        gameObject.SetActive(false);
        this.attack.text=string.Empty;
        this.defense.text=string.Empty;
        this.support.text=string.Empty;
        //this.info.text=string.Empty;
        this.pieceName.text=string.Empty;
        this.image.sprite=null;
    }
}
