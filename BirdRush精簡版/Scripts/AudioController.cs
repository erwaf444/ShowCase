using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioController : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;

    

    void Start()
    {
    
        StartCoroutine(InitializeSliders());
    }

    IEnumerator InitializeSliders()
    {
        yield return null; // 等待一帧，确保 SoundManager 完成初始化

        _musicSlider.value = SoundManager.GetMusicVolume();
        _sfxSlider.value = SoundManager.GetSoundEffectsVolume();

        // 设置Slider的监听器以处理音量变化
        _musicSlider.onValueChanged.AddListener(delegate { MusicVolume(); });
        _sfxSlider.onValueChanged.AddListener(delegate { SFXVolume(); });
    }
    public void ToggleMusic()
    {
        SoundManager.instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        SoundManager.instance.ToggleSFX();
    }

    public void MusicVolume()
    {
        SoundManager.SetMusicVolume(_musicSlider.value);
    }

    public void SFXVolume()
    {
        SoundManager.SetSoundEffectsVolume(_sfxSlider.value);
    }
}
