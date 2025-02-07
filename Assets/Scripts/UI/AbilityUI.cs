using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    public Image image;
    public Ability ability;
    // Start is called before the first frame update
    public void SetIcon(Sprite sprite){
        Debug.Log("Sprite: "+sprite);
        image.sprite=sprite;
    }
}
