using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class BattlePanel : MonoBehaviour
{
    
    public static BattlePanel _instance;
    public TMP_Text heroAttack;
    public TMP_Text heroDefense;
    public TMP_Text heroSupport;
    public TMP_Text heroTotal;
    public TMP_Text heroPieceName;
    public Image heroImage;
    public TMP_Text enemyAttack;
    public TMP_Text enemyDefense;
    public TMP_Text enemySupport;
    public TMP_Text enemyTotal;
    public TMP_Text enemyPieceName;
    public TMP_Text result;
    public Image enemyImage;
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
    public void SetAndShowAttackingStats(string attack, string support, string total, string name, Sprite sprite, string enemyAttack, string enemySupport, string enemyTotal, string enemyPieceName, Sprite enemySprite){
        gameObject.SetActive(true);
        this.heroAttack.text="attack: ";
        this.heroSupport.text="support: ";
        this.heroTotal.text="total: ";
        this.heroPieceName.text=name;
        this.heroImage.sprite=sprite;
        this.enemyAttack.text=" :defense";
        this.enemySupport.text=" :support";
        this.enemyTotal.text=" :total";
        this.enemyPieceName.text=enemyPieceName;
        this.enemyImage.sprite=enemySprite;

    }

    public void SetAndShowHeroAttack(int attack){
        SpawnsBonusPopups.Instance.BonusAdded(attack, this.heroAttack.transform.position);
        this.heroAttack.text=this.heroAttack.text+attack;
        
    }
    public void SetAndShowHeroSupport(int support){
        SpawnsBonusPopups.Instance.BonusAdded(support, this.heroSupport.transform.position);
        this.heroSupport.text="support: "+support;
        
    }
    public void SetAndShowHeroTotal(int total){
        SpawnsBonusPopups.Instance.BonusAdded(total, this.heroTotal.transform.position);
        this.heroTotal.text="total: "+total;
        
    }

    public void SetAndShowEnemyAttack(int attack){
        SpawnsBonusPopups.Instance.BonusAdded(attack, this.enemyAttack.transform.position);
        this.enemyAttack.text=attack+ this.enemyAttack.text;
        
    }
    public void SetAndShowEnemySupport(int support){
        var bonusPopUpInstance = SpawnsBonusPopups.Instance.BonusAdded(support, this.enemySupport.transform.position);        
        this.enemySupport.text=support+ this.enemySupport.text;
        
    }
    public void SetAndShowEnemyTotal(int total){
        SpawnsBonusPopups.Instance.BonusAdded(total, this.enemyTotal.transform.position);
        this.enemyTotal.text=total+ this.enemyTotal.text;
        
    }

    public void SetAndShowDefendingStats(string attack, string support, string total, string name, Sprite sprite, string enemyAttack, string enemySupport, string enemyTotal, string enemyPieceName, Sprite enemySprite){
        gameObject.SetActive(true);
        this.heroAttack.text="defense: ";
        this.heroSupport.text="support: ";
        this.heroTotal.text="total: ";
        this.heroPieceName.text=name;
        this.heroImage.sprite=sprite;
        this.enemyAttack.text=" :attack";
        this.enemySupport.text=" :support";
        this.enemyTotal.text=" :total";
        this.enemyPieceName.text=enemyPieceName;
        this.enemyImage.sprite=enemySprite;

    }

    public void SetAndShowResults(string result){
        //gameObject.SetActive(true);
        this.result.text=result;

    }
    public void HideResults(){
        //gameObject.SetActive(true);
        this.result.text="";

    }

    public void HideStats(){
        gameObject.SetActive(false);
    }
}
