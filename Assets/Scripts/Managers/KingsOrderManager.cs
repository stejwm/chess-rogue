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
    [SerializeField] private ParticleSystem flames;
    [SerializeField] private Material dissolveMaterial;

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
            this.GetComponent<Renderer>().material = dissolveMaterial;
            dissolveMaterial.SetFloat("_Weight", 0);
            title.color = Color.white;
            effect.color = Color.white;
            UpdateCardUI();
        }
        else{
            parent.SetActive(false);
        }
        
    }

    void OnMouseDown(){
        
        
        StartCoroutine(UseAndHandleUI());
        
        
    }
    IEnumerator UseAndHandleUI()
    {
        int index = Game._instance.hero.orders.IndexOf(order);

        Game._instance.hero.orders.Remove(order);
        flames.Play();
        yield return StartCoroutine(order.Use()); // Wait for King's Order effect
        flames.Stop();
        yield return StartCoroutine(Dissolve());
        
        if (Game._instance.hero.orders.Count <= 0)
        {
            parent.SetActive(false); // Now deactivate it *after* the coroutine finishes
            order = null;
        }
        else
        {
            dissolveMaterial.SetFloat("_Weight", 0);
            order = Game._instance.hero.orders[0];
            UpdateCardUI();
        }
    }

    public void UpdateCardUI(){
        title.color = Color.white;
        effect.color = Color.white;

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

    public IEnumerator Dissolve(){
        
        float dissolveAmount = 0f;
        float fadeAmount = 0.0f;
        float duration = .5f;
        flames.Stop();
        Color originalColor = title.color;
            while (dissolveAmount < duration)  // Stop when fully dissolved
            {
                dissolveAmount += Time.deltaTime * 0.3f;
                dissolveMaterial.SetFloat("_Weight", dissolveAmount);
                fadeAmount += Time.deltaTime;
                title.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - (fadeAmount / duration));
                effect.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - (fadeAmount / duration));

                yield return null; // Wait for next frame
            }
    }

    public void Hide(){
        parent.SetActive(false);
    }
}
