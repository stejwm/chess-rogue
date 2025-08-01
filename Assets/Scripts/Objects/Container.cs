using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;

public class Container : MonoBehaviour, IInteractable
{
    [SerializeField] private int value;
    [SerializeField] private bool blood;
    [SerializeField] private bool coin;
    public void OnClick(Board board)
    {
    
    }

    public void OnHover(Board board)
    {
        if(this.gameObject.activeSelf)
            PopUpManager._instance.SetAndShowValues(this.gameObject, value, coin, blood);
    }

    public void OnHoverExit(Board board)
    {
        PopUpManager._instance.HideValues();
    }

    public void OnRightClick(Board board)
    {
        
    }
}