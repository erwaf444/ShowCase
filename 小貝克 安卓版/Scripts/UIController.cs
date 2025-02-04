using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;

    void Start()
    {
        // 設置Slider的初始值為當前音量
        // _musicSlider.value = AudioManager.instance.musicSource.volume;
        // _sfxSlider.value = AudioManager.instance.sfxSource.volume;

        // 添加監聽器來即時更新音量
        _musicSlider.onValueChanged.AddListener(delegate { MusicVolume(); });
        _sfxSlider.onValueChanged.AddListener(delegate { SFXVolume(); });
    }

    public void ToggleMusic()
    {
        AudioManager.instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        AudioManager.instance.ToggleSFX();
    }

    public void MusicVolume()
    {
        AudioManager.instance.MusicVolume(_musicSlider.value);
        AudioManager.instance.SaveVolumeSettings();
    }

    public void SFXVolume()
    {
        AudioManager.instance.SFXVolume(_sfxSlider.value);
        AudioManager.instance.SaveVolumeSettings();
    }

    public void UpdateMusicSlider(float volume)
    {
        _musicSlider.value = volume;
    }

    public void UpdateSFXSlider(float volume)
    {
        _sfxSlider.value = volume;
    }

}
