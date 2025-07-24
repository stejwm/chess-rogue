using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class Settings : MonoBehaviour
{
    [SerializeField] private float masterVolume = 1f;
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private float musicVolume = 1f;
    [SerializeField] private float waitTime = 0.25f;

    public static Settings Instance;

    public float WaitTime { get => waitTime; set => waitTime = value; }
    public float MusicVolume { get => musicVolume; set => musicVolume = value; }
    public float SfxVolume { get => sfxVolume; set => sfxVolume = value; }
    public float MasterVolume { get => masterVolume; set => masterVolume = value; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public void Start()
    {
        
    }

}