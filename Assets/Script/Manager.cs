using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using System.Collections;

public class Manager : MonoBehaviour
{
    public Image[] inventorySlots; // 인벤토리 슬롯 UI 이미지를 담을 배열
    public Text[] inventoryTexts; // 인벤토리 슬롯 수량을 표시하는 텍스트 배열
    [SerializeField] Image ExInventory;
    [SerializeField] CraftPanel craftPanel; // CraftPanel 참조 추가
    bool isExpansion = false;
    
    public bool isSlowMo = false;
    void Start()
    {
        EXpansion();
    }


    internal void Collect(itemGrp item, ItemInfo itemInfo)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].sprite == item.itemSprite)
            {
                int currentCount = int.Parse(inventoryTexts[i].text);
                inventoryTexts[i].text = (currentCount + 1).ToString();

                ItemSlot slot = inventorySlots[i].GetComponent<ItemSlot>();
                if (slot == null)
                {
                    slot = inventorySlots[i].gameObject.AddComponent<ItemSlot>();
                }
                //slot.itemInfo = itemInfo;
                slot.itemInfo.itemAmount++;
                slot.craftPanel = craftPanel; // CraftPanel 설정
                
                return;
            }
            if (inventorySlots[i].sprite == null)
            {
                // 존재하지 않는 아이템이면 새로운 아이템 추가
                inventorySlots[i].sprite = item.itemSprite;
                inventorySlots[i].enabled = true;
                //inventoryTexts[i].text = itemInfo.itemAmount.ToString();
                inventoryTexts[i].text = "1";
                itemInfo.itemAmount = 1;
                inventoryTexts[i].enabled = true;

                ItemSlot slot = inventorySlots[i].GetComponent<ItemSlot>();
                if (slot == null)
                {
                    slot = inventorySlots[i].gameObject.AddComponent<ItemSlot>();
                }
                slot.itemInfo = itemInfo;
                slot.craftPanel = craftPanel; // CraftPanel 설정
                break;
            }
        }
    }
    internal void ItemDecline(ItemInfo iteminfo)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].sprite == iteminfo.itemSprite)
            {
                iteminfo.itemAmount--;
                inventoryTexts[i].text = iteminfo.itemAmount.ToString();
                if (iteminfo.itemAmount <= 0)
                {
                    inventorySlots[i].sprite = null;
                    inventoryTexts[i].enabled = false;
                    PlayerAction player = FindAnyObjectByType<PlayerAction>();
                    player.DropItem(iteminfo);
                }
            }
        }
    }
    internal void ToggleExpansion()
    {
        StartCoroutine("EXpansion");

    }
    private IEnumerator EXpansion()
    {
        foreach (Transform child in ExInventory.transform)
        {
            child.gameObject.SetActive(isExpansion);
        }
        ExInventory.enabled = isExpansion;
        isExpansion = !isExpansion;

        return null;
    }


        void Update()
    {
        if (isSlowMo == false)
        {
            Time.timeScale = 0.1f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }
}
