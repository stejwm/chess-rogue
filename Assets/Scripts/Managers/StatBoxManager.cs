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
    public Text info;
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
    public void SetAndShowStats(int attack, int defense, int support, string info, string name, Sprite sprite){
        gameObject.SetActive(true);
        this.attack.text="attack: "+attack;
        this.defense.text="defense: "+defense;
        this.support.text="support: "+support;
        this.info.text=info;
        this.pieceName.text=name;
        this.image.sprite=sprite;

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
