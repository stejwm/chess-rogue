using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class Settings : MonoBehaviour
{
    [SerializeField] private int masterVolume = 100;
    [SerializeField] private int sfxVolume = 100;
    [SerializeField] private int musicVolume = 100;
    [SerializeField] private float waitTime = 0.25f;

    public static Settings _instance;

    public float WaitTime { get => waitTime; set => waitTime = value; }
    public int MusicVolume { get => musicVolume; set => musicVolume = value; }
    public int SfxVolume { get => sfxVolume; set => sfxVolume = value; }
    public int MasterVolume { get => masterVolume; set => masterVolume = value; }

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    public void Start()
    {
        
    }

}