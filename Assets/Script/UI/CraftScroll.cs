using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class CraftScroll : MonoBehaviour
{
    [SerializeField] GameObject itemButtonPrefab;
    [SerializeField] Transform content;
    private List<CombineInfo> itemList;
    bool toggleScroll = false;

    void Start()
    {
        if (itemButtonPrefab == null || content == null || GameManager._instance._combine == null || GameManager._instance._scrollView == null || GameManager._instance._craftPanel == null) 
        {
            return; 
        }
        itemList = new();
        StartCoroutine(InitializeAfterCombine());

    }
    IEnumerator InitializeAfterCombine()
    {
        while (GameManager._instance._combine != null && GameManager._instance._combine.isInitialized == false)
        {
            yield return null;
        }
        foreach (var res in GameManager._instance._combine.combinationRules)
        {
            itemList.Add(res);
        }
        StartCoroutine(PopulateScrollView());
        //PopulateScrollView();
        ToggleScroll();
    }
    
    IEnumerator PopulateScrollView()
    {
        foreach (var item in itemList)
        {
            GameObject button = Instantiate(itemButtonPrefab, content);
            button.GetComponentInChildren<Text>().text = item.Result;
            button.GetComponentInChildren<Text>().fontSize = 25;
            button.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Art/Panel White");
            button.GetComponent<Button>().onClick.AddListener(() => OnItemButtonClick(item));
            
        }
        yield return null;
    }
    void OnItemButtonClick(CombineInfo itemName)
    {
        GameManager._instance._craftPanel.GetComponent<CraftPanel>().OpenCraftingPanel(itemName);
    }
    internal void ToggleScroll()
    {
        GameManager._instance._scrollView.SetActive(toggleScroll);
        toggleScroll = !toggleScroll;
    }

}
