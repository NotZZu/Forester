using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using static UnityEditor.Progress;

public class CraftPanel : MonoBehaviour
{
    [SerializeField] internal GameObject craftPanel;
    [SerializeField] Text itemNameText;
    [SerializeField] Text mainMaterialPropertiesText;
    [SerializeField] Text subMaterialPropertiesText;
    [SerializeField] Button craftButton;
    [SerializeField] Button exitButton;
    [SerializeField] internal Transform mainMaterialSlot;
    [SerializeField] internal Transform subMaterialSlot;
    [SerializeField] Transform objMaterialSlot;
    [SerializeField] ItemCombine craft;
    [SerializeField] PlayerAction player;
    ItemInfo mainMaterial;
    ItemInfo subMaterial;
    string objMaterial;
    private bool isMainMaterialSlotSelected = false;
    public bool IsMainMaterialSlotSelected { get { return isMainMaterialSlotSelected; } }
    private bool isSubMaterialSlotSelected = false;
    public bool IsSubMaterialSlotSelected { get { return isSubMaterialSlotSelected; } }


    void Awake()
    {
        craftButton.onClick.AddListener(OnCraftButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);

        var MainButton = mainMaterialSlot.AddComponent<Button>();
        MainButton.onClick.AddListener(SelectedMainMaterialSlot);

        var SubButton = subMaterialSlot.AddComponent<Button>();
        SubButton.onClick.AddListener(SelectedSubMaterialSlot);

        StartCoroutine(TogglePanel());
    }
    IEnumerator TogglePanel()
    {
        craftPanel.SetActive(false);
        yield break;
    }
    public void SelectedMainMaterialSlot()
    {
        if (isSubMaterialSlotSelected == true && mainMaterialSlot.GetComponent<Image>().sprite != null)
        {
            mainMaterialSlot.GetComponent<Image>().color = Color.white;
            subMaterialSlot.GetComponent<Image>().color = Color.white;
            return;
        }
        isMainMaterialSlotSelected = true;
        isSubMaterialSlotSelected = false;
        mainMaterialSlot.GetComponent<Image>().color = Color.black;
        subMaterialSlot.GetComponent<Image>().color = Color.white;

    }
    public void SelectedSubMaterialSlot()
    {
        if (isMainMaterialSlotSelected == true && subMaterialSlot.GetComponent<Image>().sprite != null)
        {
            mainMaterialSlot.GetComponent<Image>().color = Color.white;
            subMaterialSlot.GetComponent<Image>().color = Color.white;
            return;
        }
        isMainMaterialSlotSelected = false;
        isSubMaterialSlotSelected = true;
        mainMaterialSlot.GetComponent<Image>().color = Color.white;
        subMaterialSlot.GetComponent<Image>().color = Color.black;

    }
    //public bool IsMainMaterialSlotSelected()
    //{
    //    return isMainMaterialSlotSelected;
    //}
    //public bool IsSubMaterialSlotSelected()
    //{
    //    return isSubMaterialSlotSelected;
    //}
    public void OpenCraftingPanel(CombineInfo combineItem)
    {
        itemNameText.text = combineItem.Result;
        objMaterial = combineItem.Result;
        objMaterialSlot.GetComponent<Image>().sprite = combineItem.ResultSprite;
        mainMaterialPropertiesText.text = string.Join(",", combineItem.MainProperty);
        subMaterialPropertiesText.text = string.Join(",", combineItem.SubProperty);
        craftPanel.SetActive(true);
    }
    public bool SetMainMaterial(ItemInfo item)
    {
        bool res = false;
        if ((res = CheckProperties(item, mainMaterialPropertiesText.text)) == true)
        {
            mainMaterial = item;
            mainMaterialSlot.GetComponent<Image>().sprite = item.itemSprite;
            SelectedSubMaterialSlot();
        }
        return res;
    }
    public bool SetSubMaterial(ItemInfo item)
    {
        bool res = false;
        if ( (res = CheckProperties(item, subMaterialPropertiesText.text)) == true)
        {
            subMaterial = item;
            subMaterialSlot.GetComponent<Image>().sprite = item.itemSprite;
            SelectedMainMaterialSlot();
        }
        return res;
    }

    bool CheckProperties(ItemInfo item, string requiredProperties)
    {
        string[] properties = requiredProperties.Split(',', System.StringSplitOptions.None);
        foreach(var prop in properties)
        {
            if (item.itemAttr.Contains(prop))
            {
                return true;
            }
        }
        return false;
    }
    void OnCraftButtonClick()
    {
        if (mainMaterial != null && subMaterial != null)
        {
            player.GetItem(craft.Combine(mainMaterial, subMaterial, objMaterial));
            mainMaterial = null;
            subMaterial = null;
            mainMaterialSlot.GetComponent<Image>().sprite = null;
            subMaterialSlot.GetComponent<Image>().sprite = null;
            isMainMaterialSlotSelected = false;
            isSubMaterialSlotSelected = false;
            craftPanel.SetActive(false);
        }
    }
    void OnExitButtonClick()
    {
        if (mainMaterialSlot.GetComponent<Image>().sprite != null)
        {
            player.GetItem(mainMaterial);
            mainMaterialSlot.GetComponent<Image>().sprite = null;
            mainMaterial = null;
        }   
        if (subMaterialSlot.GetComponent<Image>().sprite != null)
        {
            player.GetItem(subMaterial);
            subMaterialSlot.GetComponent<Image>().sprite = null;
            subMaterial = null;
        }

        if (isMainMaterialSlotSelected || isSubMaterialSlotSelected)
        {
            isMainMaterialSlotSelected = false;
            isSubMaterialSlotSelected = false;
            mainMaterialSlot.GetComponent<Image>().color = Color.white;
            subMaterialSlot.GetComponent<Image>().color = Color.white;
        }

        craftPanel.SetActive(false);
    }
}
