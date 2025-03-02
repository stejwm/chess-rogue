using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class EnemyStatBoxManager : MonoBehaviour
{
    
    public static EnemyStatBoxManager _instance;
    public TMP_Text attack;
    public TMP_Text defense;
    public TMP_Text support;
    public GameObject abilityBox;
    public GameObject abilityUI;
    public Text pieceName;
    public Image image;
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

    // Update is called once per frame
    void Update()
    {
        //transform.position=Input.mousePosition;
    }
    public void SetAndShowStats(Chessman piece){
        foreach (Transform child in abilityBox.transform)
        {
            Destroy(child.gameObject);
        }
        gameObject.SetActive(true);
        this.attack.text="<sprite name=\"sword\">: "+piece.CalculateAttack();
        this.defense.text="<sprite name=\"shield\">: "+piece.CalculateDefense();
        this.support.text="<sprite name=\"cross\">: "+piece.CalculateSupport();
        
        this.pieceName.text=piece.name;
        this.image.sprite=piece.GetComponent<SpriteRenderer>().sprite;
        foreach (var ability in piece.abilities)
        {
            var icon=Instantiate(abilityUI, abilityBox.transform);
            icon.GetComponent<AbilityUI>().SetIcon(ability.sprite);
            icon.GetComponent<AbilityUI>().ability=ability;
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
