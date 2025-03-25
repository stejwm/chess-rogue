using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class StatBox : MonoBehaviour
{
    public TMP_Text name;
    public TMP_Text attack;
    public TMP_Text defense;
    public TMP_Text support;
    public TMP_Text diplomacy;
    public GameObject abilityBox;
    public GameObject abilityUI;
    public Image image;

    public void SetStats(Chessman target)
    {
        if(gameObject.activeSelf==false)
        {
            gameObject.SetActive(true);
        }
        foreach (Transform child in abilityBox.transform)
        {
            Destroy(child.gameObject);
        }
        this.name.text = target.name;
        this.attack.text = "<sprite name=\"sword\">: " + target.CalculateAttack();
        this.defense.text = "<sprite name=\"shield\">: " + target.CalculateDefense();
        this.support.text = "<sprite name=\"cross\">: " + target.CalculateSupport();
        this.diplomacy.text = target.diplomacy.ToString();
        this.image.sprite = target.GetComponent<SpriteRenderer>().sprite;
        

        List<Ability> multiples = new List<Ability>();
        foreach (var ability in target.abilities)
        {
            if(multiples.Contains(ability))
                continue;

            int abilityCount = target.abilities.Where(s=>s!=null && s.Equals(ability)).Count();
            if(abilityCount>1){
                var icon=Instantiate(abilityUI, abilityBox.transform);
                icon.GetComponent<AbilityUI>().SetIcon(ability.sprite);
                icon.GetComponent<AbilityUI>().ability=ability;
                icon.GetComponentInChildren<TMP_Text>().text=$"x{abilityCount}";
                multiples.Add(ability);
            }else{
                var icon=Instantiate(abilityUI, abilityBox.transform);
                icon.GetComponent<AbilityUI>().SetIcon(ability.sprite);
                icon.GetComponent<AbilityUI>().ability=ability;
                icon.GetComponentInChildren<TMP_Text>().text="";
            }
            
            
        }
    }

    public void HideStats()
    {
        gameObject.SetActive(false);
    }
}
