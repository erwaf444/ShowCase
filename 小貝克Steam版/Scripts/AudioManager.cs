using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Steamworks;


[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    float fadeInDuration = 2.0f; // 音樂淡入的持續時間
    float targetMusicVolume = 0.2f; // 音樂最終的目標音量

    public UIController uiController;

    public Sound[] vehicleSounds;
    public GameObject musicButton;
    public Sprite[] musicIcons;
    public GameObject sfxButton;
    public Sprite[] sfxIcons;
    // public bool sfxMute = false;

    private List<AudioSource> activeSFXSources = new List<AudioSource>();
    float fadeOutDuration = 2.0f; // 音樂淡出的持續時間

    // Steamworks API屬性
    public const string MusicVolumeKey = "MusicVolume";
    public const string SFXVolumeKey = "SFXVolume";
    public const string MusicMuteKey = "MusicMute";
    public const string SFXMuteKey = "SFXMute";


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    


    void Start()
    {
    
        uiController = FindObjectOfType<UIController>();
        if (uiController == null)
        {
            Debug.LogError("UI Controller is null. Please check if it has been assigned.");
        }

        if (SteamManager.Initialized)
        {
            LoadVolumeSettings();
            UpdateMusicIcon();
            UpdateSFXIcon();
        }
        // Debug.Log("Saved Music Volume: " + PlayerPrefs.GetFloat(MusicVolumeKey));
        // Debug.Log("Saved SFX Volume: " + PlayerPrefs.GetFloat(SFXVolumeKey));
        // sfxMute = false;
        
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            PlayMusicWithFadeIn("bgmOne", fadeInDuration);
        }
        if (SceneManager.GetActiveScene().name == "TestGameScene")
        {   
            PlayNextMusic();
            StartCoroutine(PlayRandomVehicleSound());
        }
    }

    public void PlayNextMusic()
    {
        // 隨機選擇一首音樂
        Debug.Log("Play Music");
        int randomIndex = UnityEngine.Random.Range(0, 3);
        string[] musicNames = { "gameBgm1", "gameBgm2", "gameBgm3" };
        string selectedMusic = musicNames[randomIndex];

        // 播放音樂並淡入
        PlayMusicWithFadeIn(selectedMusic, fadeInDuration);

        // 啟動協程來監控音樂的結束並觸發下一首播放
        StartCoroutine(CheckMusicEnd());
    }

    // 檢查當前音樂是否結束
    IEnumerator CheckMusicEnd()
    {
        // 等待音樂即將結束，然後淡出
        yield return new WaitForSeconds(musicSource.clip.length - fadeOutDuration);

        // 開始淡出音樂
        yield return StartCoroutine(FadeOutMusic(fadeOutDuration));

        // 播放下一首音樂
        PlayNextMusic();
    }

    // 音樂淡出協程
    IEnumerator FadeOutMusic(float duration)
    {
        float currentTime = 0f;
        float startVolume = musicSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, currentTime / duration);
            yield return null;
        }

        musicSource.Stop();  // 確保音樂在淡出後停止
        musicSource.volume = startVolume; // 重置音量以便下一首音樂使用
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0 代表左鍵
        {
            PlaySFX("Click"); // 播放點擊音效
        }
    }
    
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        if (!sfxSource.mute)
        {
            Sound s = Array.Find(sfxSounds, x => x.name == name);

            if (s == null)
            {
                Debug.Log("Sound not found");
            }
            else
            {
                AudioSource newSfxSource = gameObject.AddComponent<AudioSource>(); // 创建新的 AudioSource 实例
                newSfxSource.volume = sfxSource.volume;
                newSfxSource.clip = s.clip;
                newSfxSource.Play();
                activeSFXSources.Add(newSfxSource); // 将新实例添加到列表中
                StartCoroutine(ReleaseAudioSource(newSfxSource)); // 启动协程以释放 AudioSource
            }
        }
    }

    private IEnumerator ReleaseAudioSource(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying); // 等待直到音效播放完毕
        activeSFXSources.Remove(source); // 从列表中移除
        Destroy(source); // 销毁 AudioSource 实例
    }

    public void StopSFX(string name)
    {
        foreach (var source in activeSFXSources)
        {
            if (source.clip.name == name)
            {
                source.Stop(); // 停止正在播放的音效
                activeSFXSources.Remove(source); // 从列表中移除
                Destroy(source); // 销毁 AudioSource 实例
                break; // 停止后退出循环
            }
        }
    }

    // public void PlaySFX(string name)
    // {
    //     Sound s = Array.Find(sfxSounds, x => x.name == name);

    //     if (s == null)
    //     {
    //         Debug.Log("Sound not found");
    //     }
    //     else
    //     {
    //         sfxSource.clip = s.clip;
    //         sfxSource.PlayOneShot(s.clip);
    //     }
    // }

    // public void StopSFX(string name)
    // {
    //     Sound s = Array.Find(sfxSounds, x => x.name == name);

    //     if (s == null)
    //     {
    //         Debug.Log("Sound not found");
    //     }
    //     else
    //     {
    //         sfxSource.clip = s.clip;
    //         sfxSource.Stop();
    //     }
    // }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
        UpdateMusicIcon();
        SaveVolumeSettings();
    }

    private void UpdateMusicIcon()
    {
        Image musicIconImage = musicButton.GetComponent<Image>(); // 獲取音樂圖標的Image組件
        if (musicSource.mute)
        {
            musicIconImage.sprite = musicIcons[1]; // 設置為靜音圖標
        }
        else
        {
            musicIconImage.sprite = musicIcons[0]; // 設置為正常圖標
        }
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
        // sfxMute = !sfxMute;
        UpdateSFXIcon();
        SaveVolumeSettings();

    }

    private void UpdateSFXIcon()
    {
        Image sfxIconImage = sfxButton.GetComponent<Image>(); // 獲取音樂圖標的Image組件
        if (sfxSource.mute)
        {
            sfxIconImage.sprite = sfxIcons[1]; // 設置為靜音圖標
        }
        else
        {
            sfxIconImage.sprite = sfxIcons[0]; // 設置為正常圖標
        }
    }

   

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }



    // 播放音樂並淡入
    public void PlayMusicWithFadeIn(string name, float duration)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.volume = 0f; // 開始音量為0
            musicSource.Play();
            StartCoroutine(FadeInMusic(duration)); // 啟動淡入協程
        }
    }

    // 音樂淡入協程
    IEnumerator FadeInMusic(float duration)
    {
        float currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetMusicVolume, currentTime / duration); // 緩慢增大音量
            yield return null;
        }
        musicSource.volume = targetMusicVolume; // 確保音量到達目標
        uiController._musicSlider.value = musicSource.volume;
    }

    // 隨機播放車輛經過聲音
    IEnumerator PlayRandomVehicleSound()
    {
        while (true)
        {
            // 隨機等待時間，範圍在 5 到 15 秒之間
            float randomDelay = UnityEngine.Random.Range(15f, 30f);
            yield return new WaitForSeconds(randomDelay);

            // 隨機選擇一個車輛聲音
            int randomIndex = UnityEngine.Random.Range(0, vehicleSounds.Length);
            Sound vehicleSound = vehicleSounds[randomIndex];

            if (vehicleSound != null)
            {
                sfxSource.PlayOneShot(vehicleSound.clip);
            }
        }
    }

    //Playerprefs方法
    public void LoadVolumeSettings()
    {
        Debug.Log("LoadVolumeSettings");

        // 從 Steam Cloud 加載音量值
        float musicVolume = 0.2f; // 默認音量
        float sfxVolume = 0.5f;

        // bool successGetMusic = SteamUserStats.GetStat(MusicVolumeKey, out musicVolume);

        // if (successGetMusic)
        // {
        //     Debug.Log("Get Music Volume: " + musicVolume);
        // } else
        // {
        //     Debug.Log("Get Music Volume Failed");
        // }

        if (SteamUserStats.GetStat(MusicVolumeKey, out float savedMusicVolume))
        {
            musicVolume = savedMusicVolume;
        }

        if (SteamUserStats.GetStat(SFXVolumeKey, out float savedSFXVolume))
        {
            sfxVolume = savedSFXVolume;
        }

        if (SteamUserStats.GetStat(MusicMuteKey, out int savedMusicMute))
        {
            musicSource.mute = savedMusicMute == 1;
            Debug.Log($"Loaded music mute state: {musicSource.mute}");
        }

        if (SteamUserStats.GetStat(SFXMuteKey, out int savedSFXMute))
        {
            sfxSource.mute = savedSFXMute == 1;
            // sfxMute = sfxSource.mute; 
            Debug.Log($"Loaded music mute state: {sfxSource.mute}");
        }

        targetMusicVolume = musicVolume;
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;

        Debug.Log("Loaded music volume: " + musicVolume);
        Debug.Log("Loaded SFX volume: " + sfxVolume);

        // 更新滑桿
        if (uiController != null)
        {
            uiController._musicSlider.value = musicVolume;
            Debug.Log("Update Music Slider: " + uiController._musicSlider.value);
            uiController._sfxSlider.value = sfxVolume;
            Debug.Log("Update SFX Slider: " + uiController._sfxSlider.value);
            UpdateMusicIcon();
            UpdateSFXIcon();
        }
        else
        {
            Debug.LogError("UI Controller is null. Please check if it has been assigned.");
        }
    }

    public void SaveVolumeSettings()
    {
        // 保存音量值到 Steam Cloud
        SteamUserStats.SetStat(MusicVolumeKey, musicSource.volume);
        SteamUserStats.SetStat(SFXVolumeKey, sfxSource.volume);
        // SteamUserStats.SetStat(MusicMuteKey, musicSource.mute ? 1 : 0);
        bool musicMutesuccess = SteamUserStats.SetStat(MusicMuteKey, musicSource.mute ? 1 : 0);
        if (musicMutesuccess){
            Debug.Log($"Saving music mute state: {musicSource.mute}, Success: {musicMutesuccess}");
        }

        bool sfxMutesuccess = SteamUserStats.SetStat(SFXMuteKey, sfxSource.mute ? 1 : 0);
        if (sfxMutesuccess){
            Debug.Log($"Saving SFX mute state: {sfxSource.mute}, Success: {sfxMutesuccess}");
        }

        Debug.Log("Saved music volume: " + musicSource.volume);
        Debug.Log("Saved SFX volume: " + sfxSource.volume);
        Debug.Log("Saved music mute state: " + musicSource.mute);


        // 確保數據被提交到 Steam Cloud
        SteamUserStats.StoreStats();
    }
}


