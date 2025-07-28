using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class SoundManager : MonoBehaviour
{

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    public AudioClip cardFlip;
    public AudioClip cardHover;
    public AudioClip pieceBonus;
    public AudioClip flames;
    public AudioClip applyAbility;
    public AudioClip dropIn;
    public AudioClip pieceMove;
    public AudioClip capture;
    public AudioClip bounce;
    public AudioClip purchase;
    public static SoundManager Instance;
    void Awake()
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

    public void PlaySoundFXClip(AudioClip clip, float pitch, float volume)
    {
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, volume);
    }
    public void PlaySoundFXClip(AudioClip clip, float pitch)
    {
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, Settings.Instance.SfxVolume);
    }
    public void PlaySoundFXClip(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip, Settings.Instance.SfxVolume);
    }
    public void ResetPitch()
    {
        sfxSource.pitch = 1f;
    }
}
