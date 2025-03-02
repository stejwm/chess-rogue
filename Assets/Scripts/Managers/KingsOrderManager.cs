using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class KingsOrderManager : MonoBehaviour
{
    public KingsOrder order;
    public TMP_Text title;
    public TMP_Text effect;
    public static KingsOrderManager _instance;

    public GameObject parent;


    void Awake()
    {
        
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }

    public void Setup(){
        if (Game._instance.hero.orders.Count>0)
        {
            parent.SetActive(true);
            order = Game._instance.hero.orders[0];
            UpdateCardUI();
        }
        else{
            parent.SetActive(false);
        }
        
    }

    void OnMouseDown(){
        int index = Game._instance.hero.orders.IndexOf(order);

        Game._instance.hero.orders.Remove(order);
        
        StartCoroutine(order.Use());
        if (index==0){
            parent.SetActive(false);
            order=null;
        }
        else{
            order= Game._instance.hero.orders[0];
            UpdateCardUI();
        }
        
        
    }

    public void UpdateCardUI(){
        title.text= order.Name;
        effect.text= order.Description;
    }

    public void CardLeft(){
        int index = Game._instance.hero.orders.IndexOf(order);
        if (index>0){
            index--;
            order=Game._instance.hero.orders[index];
            UpdateCardUI();
        }else{
            this.GetComponent<MMSpringPosition>().BumpRandom();
        }
    }
    public void CardRight(){
        int index = Game._instance.hero.orders.IndexOf(order);
        if (index<Game._instance.hero.orders.Count-1){
            index++;
            order=Game._instance.hero.orders[index];
            UpdateCardUI();
        }
        else{
            this.GetComponent<MMSpringPosition>().BumpRandom();
        }
    }
}
