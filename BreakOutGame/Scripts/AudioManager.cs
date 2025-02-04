using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    [Header("----Audio Sources----")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("----Audio Clip----")]
    public AudioClip menuBgm;
    public AudioClip death;
    public AudioClip destroy;
    public AudioClip startGame;
    public AudioClip Win;
    public AudioClip clickButton;

    [Header("----VolumeSetting----")]
    public VolumeSetting volumeSetting;

 

    private void Start()
    {
        musicSource.clip = menuBgm;
        musicSource.loop = true;
        // musicSource.volume = 0.2f;

        // float savedVolume = PlayerPrefs.GetFloat("musicVolume", 1f); 
        // musicSource.volume = savedVolume;
        // AudioListener.volume = musicSource.volume;
        volumeSetting.Load();
        musicSource.Play();


    }

    public void BrickDestroyed(AudioClip clip)
    {
        if (clip != null)
        {
            SFXSource.PlayOneShot(clip);
        }
    }

    public void PlayDestroySound()
    {
        BrickDestroyed(destroy);
    }

    

    
}
