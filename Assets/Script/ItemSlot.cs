using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemInfo itemInfo;
    [SerializeField] internal CraftPanel craftPanel;
    [SerializeField] Manager manager;

    void Awake()
    {
        manager = FindAnyObjectByType<Manager>();
        craftPanel = FindAnyObjectByType<CraftPanel>();
        var button = gameObject.AddComponent<Button>();
        button.onClick.AddListener(OnItemClick);
    }
    void OnItemClick()
    {
        if (craftPanel != null && itemInfo != null)
        {
            if (craftPanel.IsMainMaterialSlotSelected == true)
            {
                if (itemInfo.itemAmount <= 0)
                {
                    manager.ItemDecline(itemInfo);
                    return;
                }
                
                if (craftPanel.SetMainMaterial(itemInfo))
                {
                    manager.ItemDecline(itemInfo);
                }
            }
            else if(craftPanel.IsSubMaterialSlotSelected == true)
            {
                if (itemInfo.itemAmount <= 0)
                {
                    manager.ItemDecline(itemInfo);
                    return;
                }
                if (craftPanel.SetSubMaterial(itemInfo))
                {
                    manager.ItemDecline(itemInfo);
                }
            }
        }
    }
    public void SetItemInfo(ItemInfo info)
    {
        itemInfo = info;
    }
}
