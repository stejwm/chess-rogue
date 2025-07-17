using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour, IInteractable
{
    public Image image;
    public Ability ability;

    public void OnClick(Board board)
    {
        //throw new NotImplementedException();
    }

    public void OnHover(Board board)
    {
        Debug.Log("Hovered over ability icon");
        PopUpManager._instance.SetAndShowAbilityInfo(this);
    }

    public void OnHoverExit(Board board)
    {
        Debug.Log("exited over ability icon");
        PopUpManager._instance.HideInfo();
    }

    public void OnRightClick(Board board)
    {
        //throw new NotImplementedException();
    }

    // Start is called before the first frame update
    public void SetIcon(Sprite sprite){
        //Debug.Log("Sprite: "+sprite);
        image.sprite=sprite;
    }

}
