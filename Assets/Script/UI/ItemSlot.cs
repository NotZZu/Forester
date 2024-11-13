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
        public GameObject popupText; // �˾� �ؽ�Ʈ UI ���

        // ���콺�� UI ��ҿ� ���� �� ȣ��˴ϴ�.
        public void OnPointerEnter(PointerEventData eventData)
        {
            popupText.SetActive(true); // �˾� �ؽ�Ʈ Ȱ��ȭ
        }

        // ���콺�� UI ��ҿ��� ���� �� ȣ��˴ϴ�.
        public void OnPointerExit(PointerEventData eventData)
        {
            popupText.SetActive(false); // �˾� �ؽ�Ʈ ��Ȱ��ȭ
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
