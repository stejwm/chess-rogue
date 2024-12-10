using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class RewardStatManager : MonoBehaviour
{
    
    public static RewardStatManager _instance;
    public Text attack;
    public Text defense;
    public Text support;
    public Text info;
    public Text pieceName;
    public Image image;
    public GameObject abilityUI;
    public Chessman piece;
    public GameObject infoBox;
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
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void SetAndShowStats(Chessman piece){
        foreach(Transform child in infoBox.transform)
        {
            Destroy(child.gameObject);
        }
        gameObject.SetActive(true);
        this.attack.text="attack: "+piece.attack;
        this.defense.text="defense: "+piece.defense;
        this.support.text="support: "+piece.support;
        this.info.text=piece.info;
        this.pieceName.text=piece.name;
        this.image.sprite=piece.GetComponent<SpriteRenderer>().sprite;
        this.piece=piece;
        StartCoroutine(SetAbilities(piece));
        

    }
    public IEnumerator SetAbilities(Chessman piece){
        yield return null;
        foreach (var ability in piece.abilities)
        {
            Instantiate(abilityUI, infoBox.transform);
        }
    }

    public void HideStats(){
        gameObject.SetActive(false);
        this.attack.text=string.Empty;
        this.defense.text=string.Empty;
        this.support.text=string.Empty;
        this.info.text=string.Empty;
        this.pieceName.text=string.Empty;
        this.image.sprite=null;
    }
}
