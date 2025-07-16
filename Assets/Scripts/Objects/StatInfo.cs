using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StatInfo : MonoBehaviour, IInteractable
{
    public StatType statType;
    public int baseStat;
    public int total;
    public Dictionary<string, int> dictionary;

    public void OnClick(Board board)
    {
        //throw new NotImplementedException();
    }

    public void OnHover(Board board)
    {
        Debug.Log("Hovered over stat info");
        PopUpManager._instance.SetAndShowStatInfo(this);
    }

    public void OnHoverExit(Board board)
    {
        PopUpManager._instance.HideInfo();
    }

    public void OnRightClick(Board board)
    {
        //throw new NotImplementedException();
    }

}