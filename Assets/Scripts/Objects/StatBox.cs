using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        foreach (var ability in target.abilities)
        {
            var icon=Instantiate(abilityUI, abilityBox.transform);
            icon.GetComponent<AbilityUI>().SetIcon(ability.sprite);
            icon.GetComponent<AbilityUI>().ability=ability;
        }
    }

    public void HideStats()
    {
        gameObject.SetActive(false);
    }
}
