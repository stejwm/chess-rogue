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
    public GameObject heroImage;
    public TMP_Text enemyAttack;
    public TMP_Text enemyDefense;
    public TMP_Text enemySupport;
    public TMP_Text enemyTotal;
    public TMP_Text enemyPieceName;
    public TMP_Text result;
    public GameObject enemyImage;
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
    public void SetAndShowAttackingStats(string attack, string support, string total, string name, GameObject sprite, string enemyAttack, string enemySupport, string enemyTotal, string enemyPieceName, GameObject enemySprite){
        gameObject.SetActive(true);
        this.heroAttack.text="attack: ";
        this.heroSupport.text="support: ";
        this.heroTotal.text="total: ";
        this.heroPieceName.text=name;
        this.heroImage.transform.localPosition=new Vector3(-687,0,0);
        this.heroImage.GetComponent<SpriteRenderer>().sprite=sprite.GetComponent<SpriteRenderer>().sprite;
        this.heroImage.GetComponent<Animator>().runtimeAnimatorController=sprite.GetComponent<Animator>().runtimeAnimatorController;
        this.enemyAttack.text=" :defense";
        this.enemySupport.text=" :support";
        this.enemyTotal.text=" :total";
        this.enemyPieceName.text=enemyPieceName;
        this.enemyImage.transform.localPosition=new Vector3(687,0,0);
        this.enemyImage.GetComponent<SpriteRenderer>().sprite=enemySprite.GetComponent<SpriteRenderer>().sprite;
        this.enemyImage.GetComponent<Animator>().runtimeAnimatorController=enemySprite.GetComponent<Animator>().runtimeAnimatorController;

    }

    public void SetAndShowHeroAttack(int attack, float pitch){
        SpawnsBonusPopups.Instance.BonusAdded(attack, this.heroAttack.transform.position, pitch);
        this.heroAttack.text=this.heroAttack.text+attack;
        
    }
    public void SetAndShowHeroSupport(int support, float pitch){
        SpawnsBonusPopups.Instance.BonusAdded(support, this.heroSupport.transform.position,pitch);
        this.heroSupport.text="support: "+support;
        
    }
    public void SetAndShowHeroTotal(int total, float pitch){
        SpawnsBonusPopups.Instance.BonusAdded(total, this.heroTotal.transform.position,pitch);
        this.heroTotal.text="total: "+total;
        
    }

    public void SetAndShowEnemyAttack(int attack, float pitch){
        SpawnsBonusPopups.Instance.BonusAdded(attack, this.enemyAttack.transform.position,pitch);
        this.enemyAttack.text=attack+ this.enemyAttack.text;
        
    }
    public void SetAndShowEnemySupport(int support, float pitch){
        var bonusPopUpInstance = SpawnsBonusPopups.Instance.BonusAdded(support, this.enemySupport.transform.position,pitch);        
        this.enemySupport.text=support+ this.enemySupport.text;
        
    }
    public void SetAndShowEnemyTotal(int total, float pitch){
        SpawnsBonusPopups.Instance.BonusAdded(total, this.enemyTotal.transform.position,pitch);
        this.enemyTotal.text=total+ this.enemyTotal.text;
        
    }

    public void SetAndShowDefendingStats(string attack, string support, string total, string name, GameObject sprite, string enemyAttack, string enemySupport, string enemyTotal, string enemyPieceName, GameObject enemySprite){
        gameObject.SetActive(true);
        this.heroAttack.text="defense: ";
        this.heroSupport.text="support: ";
        this.heroTotal.text="total: ";
        this.heroPieceName.text=name;
        this.heroImage.transform.localPosition=new Vector3(-687,0,0);
        this.heroImage.GetComponent<SpriteRenderer>().sprite=sprite.GetComponent<SpriteRenderer>().sprite;
        this.heroImage.GetComponent<Animator>().runtimeAnimatorController=sprite.GetComponent<Animator>().runtimeAnimatorController;
        this.enemyAttack.text=" :attack";
        this.enemySupport.text=" :support";
        this.enemyTotal.text=" :total";
        this.enemyPieceName.text=enemyPieceName;
        this.enemyImage.transform.localPosition=new Vector3(687,0,0);
        this.enemyImage.GetComponent<SpriteRenderer>().sprite=enemySprite.GetComponent<SpriteRenderer>().sprite;
        this.enemyImage.GetComponent<Animator>().runtimeAnimatorController=enemySprite.GetComponent<Animator>().runtimeAnimatorController;

    }

    public void SetAndShowResults(string result){
        //gameObject.SetActive(true);
        this.result.text=result;

    }
    public void HideResults(){
        //gameObject.SetActive(true);
        this.result.text="";
        result.gameObject.SetActive(false);

    }

    public void HideStats(){
        gameObject.SetActive(false);
    }
}
