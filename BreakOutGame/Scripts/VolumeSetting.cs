using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine;

public class VolumeSetting : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    public GameObject On;
    public GameObject Off;





    
  

    void Start()
    {

        if(PlayerPrefs.HasKey("musicVolume"))
        {
            Load();
        }
        else
        {
            PlayerPrefs.SetFloat("musicVolume", 0.2f);
            Save();
        }
        volumeSlider.onValueChanged.AddListener(delegate { ChangeVolume(); }); // 监听音量滑动条的变化
    }

    public void ChangeVolume()
    {
        
        // if (volumeSlider.value == 0)
        // {
        //     volumeSlider.value = 0.001f;
        // }
        AudioListener.volume = volumeSlider.value;
        if (volumeSlider.value == 0f)
        {
            On.SetActive(false);
            Off.SetActive(true);
        } else if (volumeSlider.value > 0.001f)
        {
            On.SetActive(true);
            Off.SetActive(false);
        }
        Save();
    }

    public void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        // if (volumeSlider.value < 0.2f)
        // {
        //     volumeSlider.value = 0.2f;
        //     // volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");

        // }
        AudioListener.volume = volumeSlider.value;
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
        PlayerPrefs.Save(); 
    }

    public void Mute()
    {
        // PlayerPrefs.SetFloat("musicMute", 1);
        volumeSlider.value = 0;
        AudioListener.volume = volumeSlider.value;
        // PlayerPrefs.SetFloat("musicVolume", 0.001f);
        PlayerPrefs.Save();
        On.SetActive(false);
        Off.SetActive(true);
        // PlayerPrefs.Save();
    }
    
    public void UnMute()
    {
        // PlayerPrefs.SetFloat("musicMute", 0);
        volumeSlider.value = 0.2f;
        AudioListener.volume = volumeSlider.value;
        // PlayerPrefs.SetFloat("musicVolume", 0.2f);
        PlayerPrefs.Save();
        On.SetActive(true);
        Off.SetActive(false);
        // PlayerPrefs.Save();
    }
  

}