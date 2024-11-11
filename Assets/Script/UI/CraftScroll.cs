using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class CraftScroll : MonoBehaviour
{
    [SerializeField] GameObject itemButtonPrefab;
    [SerializeField] Transform content;
    private List<CombineInfo> itemList;
    [SerializeField] ItemCombine combine;
    [SerializeField] CraftPanel craftPanel;
    bool toggleScroll = false;
    [SerializeField] GameObject scrollView;

    void Start()
    {
        if (itemButtonPrefab == null || content == null || combine == null || scrollView == null || craftPanel == null) 
        {
            return; 
        }
        itemList = new();
        StartCoroutine(InitializeAfterCombine());

    }
    IEnumerator InitializeAfterCombine()
    {
        while (combine != null && combine.isInitialized == false)
        {
            yield return null;
        }
        foreach (var res in combine.combinationRules)
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
            yield return null;
        }
    }
    void OnItemButtonClick(CombineInfo itemName)
    {
        craftPanel.GetComponent<CraftPanel>().OpenCraftingPanel(itemName);
    }
    internal void ToggleScroll()
    {
        scrollView.SetActive(toggleScroll);
        toggleScroll = !toggleScroll;
    }

}
