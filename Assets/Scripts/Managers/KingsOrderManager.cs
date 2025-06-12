using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class KingsOrderManager : MonoBehaviour
{

    public TMP_Text title;
    public TMP_Text effect;
    public ParticleSystem flames;
    [SerializeField] Board board;
    [SerializeField] private Material dissolveMaterial;
    public GameObject parent;


    public void Setup(){
        if (board.Hero.orders.Count>0)
        {
            parent.SetActive(true);
            KingsOrder order = board.Hero.orders[0];
            this.GetComponent<Renderer>().material = dissolveMaterial;
            dissolveMaterial.SetFloat("_Weight", 0);
            title.color = Color.white;
            effect.color = Color.white;
            UpdateCardUI(order);
        }
        else{
            parent.SetActive(false);
        }
        
    }

    public void HandleClick(GameObject clicked)
    {
        KingsOrder order = clicked.GetComponent<KingsOrder>();
        if (order != null)
        {
            StartCoroutine(HandleOrderClick(order));
        }
    }

    private IEnumerator HandleOrderClick(KingsOrder order)
    {
        if (!order.canBeUsedFromManagement) {
            this.GetComponent<MMSpringPosition>().BumpRandom();
            yield break;
        }
        
        
        board.Hero.orders.Remove(order);
        flames.Play();
        yield return StartCoroutine(order.Use(board));
        flames.Stop();
        yield return StartCoroutine(Dissolve());
        
        if (board.Hero.orders.Count <= 0)
        {
            parent.SetActive(false);
        }
        else
        {
            dissolveMaterial.SetFloat("_Weight", 0);
            UpdateCardUI(order);
        }
    }

    public void UpdateCardUI(KingsOrder order){
        title.color = Color.white;
        effect.color = Color.white;

        title.text= order.Name;
        effect.text= order.Description;
    }

    public void CardLeft(){
       // int index = board.Hero.orders.IndexOf(order);
        /* if (index>0){
            index--;
            KingsOrder order=board.Hero.orders[index];
            UpdateCardUI(order);
        }else{
            this.GetComponent<MMSpringPosition>().BumpRandom();
        } */
    }
    public void CardRight(){
        //int index = board.Hero.orders.IndexOf(order);
        /* if (index<board.Hero.orders.Count-1){
            index++;
            KingsOrder order = board.Hero.orders[index];
            UpdateCardUI(order);
        }
        else{
            this.GetComponent<MMSpringPosition>().BumpRandom();
        } */
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
