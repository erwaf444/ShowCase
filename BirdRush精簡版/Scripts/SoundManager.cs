using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum SoundType
{
    CoinPickup,
    GameOver,
    GameStart,
    GameMusic,
    MainMenuMusic,
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    // AudioSource audioSource;
    AudioSource soundEffectsSource;  // 用於播放音效的AudioSource
    AudioSource musicSource;  // 用於播放背景音樂的AudioSource
    public static SoundManager instance;
    public AudioController audioController;
    public GameObject SFXButton;
    public GameObject MusicButton;

    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public Sprite sfxOnSprite;
    public Sprite sfxOffSprite;
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string MusicMuteKey = "MusicMute";
    private const string SFXMuteKey = "SFXMute";




    void Awake()
    {
        // if (instance != null && instance != this)
        // {
        //     Destroy(gameObject);
        //     return;
        // }
        instance = this;
        // DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
         if (audioController == null)
        {
            Debug.LogError("AudioController is not initialized!");
            return;
        }
        AudioSource[] sources = GetComponents<AudioSource>();
        soundEffectsSource = sources[0];
        musicSource = sources[1];
        // if (sources.Length > 1)
        // {
        //     musicSource = sources[1];
        // }
        // else
        // {
        //     musicSource = gameObject.AddComponent<AudioSource>();
        //     musicSource.loop = true;  // 背景音樂應該循環播放
        // }

        // 加載音量和靜音狀態
        musicSource.volume = PlayerPrefs.GetFloat(MusicVolumeKey, 1.0f);
        Debug.Log(musicSource.volume);
        audioController._musicSlider.value = musicSource.volume;

        
        soundEffectsSource.volume = PlayerPrefs.GetFloat(SFXVolumeKey, 1.0f);
        Debug.Log(soundEffectsSource.volume);
        audioController._sfxSlider.value = soundEffectsSource.volume;


        musicSource.mute = PlayerPrefs.GetInt(MusicMuteKey, 0) == 1;
        soundEffectsSource.mute = PlayerPrefs.GetInt(SFXMuteKey, 0) == 1;
        PlayMusicOnMainMenu();

        // 根據靜音狀態設置按鈕圖片
        UpdateMusicButtonImage();
        UpdateSFXButtonImage();
    }

    void UpdateMusicButtonImage()
    {
        MusicButton.GetComponent<Image>().sprite = musicSource.mute ? musicOffSprite : musicOnSprite;
    }

    void UpdateSFXButtonImage()
    {
        SFXButton.GetComponent<Image>().sprite = soundEffectsSource.mute ? sfxOffSprite : sfxOnSprite;
    }


    public static void PlaySound(SoundType soundType, float volume = 1f)
    {
        instance.soundEffectsSource.PlayOneShot(instance.audioClips[(int)soundType], volume);
    }


    // 播放背景音樂
    public static void PlayMusic(SoundType musicType, float volume = 1f)
    {
        instance.musicSource.clip = instance.audioClips[(int)musicType];
        instance.musicSource.Play();
    }

    // 停止背景音樂
    public static void StopMusic()
    {
        if (instance.musicSource.isPlaying)
        {
            instance.musicSource.Stop();
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;

        // 保存靜音狀態
        PlayerPrefs.SetInt(MusicMuteKey, musicSource.mute ? 1 : 0);
        PlayerPrefs.Save();

        // 更新按鈕圖片
        UpdateMusicButtonImage();
    }

    public void ToggleSFX()
    {
        soundEffectsSource.mute = !soundEffectsSource.mute;

        // 保存靜音狀態
        PlayerPrefs.SetInt(SFXMuteKey, soundEffectsSource.mute ? 1 : 0);
        PlayerPrefs.Save();

        // 更新按鈕圖片
        UpdateSFXButtonImage();
    }

    // 設置音效音量
    public static void SetSoundEffectsVolume(float volume)
    {
        instance.soundEffectsSource.volume = volume;

        // 保存音量
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
        PlayerPrefs.Save();
        Debug.Log("SFX volume set to: " + volume);
    }

    // 設置背景音樂音量
    public static void SetMusicVolume(float volume)
    {
        instance.musicSource.volume = volume;

        // 保存音量
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
        PlayerPrefs.Save();
        Debug.Log("Music volume set to: " + volume);
    }

    // 獲取當前的背景音樂音量
    public static float GetMusicVolume()
    {
        // return instance.musicSource.volume;
        float volume = PlayerPrefs.GetFloat(MusicVolumeKey, 1.0f);
        Debug.Log("Loaded music volume from PlayerPrefs: " + volume);
        return volume;
    }

    // 獲取當前的音效音量
    public static float GetSoundEffectsVolume()
    {
        return instance.soundEffectsSource.volume;
    }

    public void PlayMusicOnMainMenu()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            PlayMusic(SoundType.MainMenuMusic);
        }
    }

}
