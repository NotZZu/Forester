using UnityEngine;
using UnityEngine.EventSystems;
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
    public class UIPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject popupText; // 팝업 텍스트 UI 요소

        // 마우스가 UI 요소에 들어올 때 호출됩니다.
        public void OnPointerEnter(PointerEventData eventData)
        {
            popupText.SetActive(true); // 팝업 텍스트 활성화
        }

        // 마우스가 UI 요소에서 나갈 때 호출됩니다.
        public void OnPointerExit(PointerEventData eventData)
        {
            popupText.SetActive(false); // 팝업 텍스트 비활성화
        }
    }

    void OnItemClick()
    {
        if (craftPanel != null && itemInfo != null)
        {
            if (craftPanel.mainMaterialSlot.GetComponent<Image>().sprite != null &&
                craftPanel.subMaterialSlot.GetComponent<Image>().sprite != null)
            {
                return;
            }
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
