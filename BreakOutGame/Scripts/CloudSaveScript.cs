using UnityEngine;
using UnityEngine.UI;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using TMPro;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;



public class CloudSaveScript : MonoBehaviour
{

    public TextMeshProUGUI status;
    public TMP_InputField input;
    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();
    }

   
    public async void SaveData()
    {   
        var data = new Dictionary<string, object> { {"firstData", input.text} };

        try
        {
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        status.text = "Data saved successfully!";
        }
        catch (Exception ex)
        {
        status.text = $"Error saving data: {ex.Message}";
        }
    }

    public async void LoadData()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "firstData" });
        if (data.ContainsKey("firstData"))
        {
        status.text = $"Loaded data: {data["firstData"]}";
        }
        else
        {
        status.text = "No data found for key 'firstData'";
        }
    }

    public async void DeleteKey()
    {
        await CloudSaveService.Instance.Data.ForceDeleteAsync("musicVolume");
    }

    public async void RetriveAllKeys()
    {
        List<string> keys = await CloudSaveService.Instance.Data.RetrieveAllKeysAsync();

        for (int i = 0; i < keys.Count; i++)
        {
            print(keys[i]);
        }
    }

}
