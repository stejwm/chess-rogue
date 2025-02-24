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
            this.gameObject.SetActive(true);
            order = Game._instance.hero.orders[0];
            title.text= order.Name;
            effect.text= order.Description;
        }
        else{
            this.gameObject.SetActive(false);
        }
        
    }

    void OnMouseDown(){
        Debug.Log("Order Used");
        StartCoroutine(order.Use());
    }
}
