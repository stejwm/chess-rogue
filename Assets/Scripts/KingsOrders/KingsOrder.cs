using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

public abstract class KingsOrder : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite sprite;
    public int Cost = 15;
    public abstract IEnumerator Use();
    public bool canBeUsedFromManagement;
    protected KingsOrder(string name, string description)
    {
        Name = name;
        Description = description;
        Cost=15;
    }
    public KingsOrder Clone()
    {
        return Instantiate(this);
    }

}
