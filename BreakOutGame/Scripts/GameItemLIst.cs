using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameItemLIst : MonoBehaviour
{
    public static GameItemLIst instance;

    // public TextMeshProUGUI coinsText;
    // public TextMeshProUGUI descriptionText;
    public GameObject itemListPanel;
    public GameItems[] gameItems;
    public Transform GameItemListContent;
    public GameObject itemPrefab;



    // Start is called before the first frame update
    
    private void Awake(){
        if(instance == null){
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        itemListPanel.SetActive(false);
        foreach(GameItems gameItem in gameItems){
            GameObject item = Instantiate(itemPrefab, GameItemListContent);

            gameItem.itemRef = item;
            
            foreach( Transform child in item.transform){
                if(child.gameObject.name == "Name"){
                    child.GetComponent<TextMeshProUGUI>().text = gameItem.name;
                } else if (child.gameObject.name == "Description"){
                    child.GetComponent<TextMeshProUGUI>().text = gameItem.description;
                } else if (child.gameObject.name == "Image"){
                    child.GetComponent<Image>().sprite = gameItem.image;
                } else if (child.gameObject.name == "Cost"){
                    child.GetComponent<TextMeshProUGUI>().text = gameItem.cost.ToString();
                }
            }
        
        }
       
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI() {
        // coinsText.text = "Coins: " ;
    }

    public void ToggleGameItemPanel() {
        itemListPanel.SetActive(!itemListPanel.activeSelf);
    }

 
    [System.Serializable]
    public class GameItems
    {
        public string name;
        public int cost;
        public string description;
        public Sprite image;
        [HideInInspector] public GameObject itemRef;
    }
}
