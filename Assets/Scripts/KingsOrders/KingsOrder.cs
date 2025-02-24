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
    public abstract IEnumerator Use();
    protected KingsOrder(string name, string description)
    {
        Name = name;
        Description = description;
    }
    public KingsOrder Clone()
    {
        return Instantiate(this);
    }

}
