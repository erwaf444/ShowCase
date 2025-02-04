using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SearchManager : MonoBehaviour
{
    public GameObject ContentHolder;
    public GameObject[] Element;
    public GameObject SearchBar;
    public int totalElement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        totalElement = ContentHolder.transform.childCount;

        Element = new GameObject[totalElement];

        for(int i = 0; i < totalElement; i++)
        {
            Element[i] = ContentHolder.transform.GetChild(i).gameObject;
        }
    }

    public void Search()
    {
        string searchText = SearchBar.GetComponent<TMP_InputField>().text.ToLower();
        int searchTextLength = searchText.Length;

        // Lists to store exact and partial matches
        List<GameObject> exactMatches = new List<GameObject>();
        List<GameObject> partialMatches = new List<GameObject>();

        foreach (GameObject ele in Element)
        {
            string elementText = ele.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.ToLower();

            if (elementText.Length >= searchTextLength)
            {
                // Check if it's an exact match
                if (elementText == searchText)
                {
                    exactMatches.Add(ele);
                    ele.SetActive(true);
                }
                // Check if it starts with the search text (partial match)
                else if (elementText.Substring(0, searchTextLength) == searchText)
                {
                    partialMatches.Add(ele);
                    ele.SetActive(true);
                }
                else
                {
                    ele.SetActive(false);
                }
            }
            else
            {
                ele.SetActive(false);
            }
        }

        // Combine exact matches and partial matches
        List<GameObject> sortedResults = new List<GameObject>();
        sortedResults.AddRange(exactMatches);
        sortedResults.AddRange(partialMatches);

        // Update Element array to reflect the sorted order
        for (int i = 0; i < sortedResults.Count; i++)
        {
            Element[i] = sortedResults[i];
        }
    }
}
