using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using System.Collections;
using System.Linq;

public class Manager : MonoBehaviour
{
    public Image[] inventorySlots; // �κ��丮 ���� UI �̹����� ���� �迭
    public Text[] inventoryTexts; // �κ��丮 ���� ������ ǥ���ϴ� �ؽ�Ʈ �迭
    [SerializeField] Image ExInventory;
    [SerializeField] CraftPanel craftPanel; // CraftPanel ���� �߰�
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
                if (inventorySlots[i].GetComponent<ItemSlot>().itemInfo.itemAttr.Count != itemInfo.itemAttr.Count) { continue; }
                foreach (var attr in inventorySlots[i].GetComponent<ItemSlot>().itemInfo.itemAttr)
                {
                    if (itemInfo.itemAttr.Contains(attr) == false)
                    {
                        continue;
                    }
                }
                
                int currentCount = int.Parse(inventoryTexts[i].text);
                inventoryTexts[i].text = (currentCount + 1).ToString();

                ItemSlot slot = inventorySlots[i].GetComponent<ItemSlot>();
                if (slot == null)
                {
                    slot = inventorySlots[i].gameObject.AddComponent<ItemSlot>();
                }
                //slot.itemInfo = itemInfo;
                slot.itemInfo.itemAmount++;
                //slot.craftPanel = craftPanel; // CraftPanel ����
                
                return;
            }
            if (inventorySlots[i].sprite == null)
            {
                // �������� �ʴ� �������̸� ���ο� ������ �߰�
                inventorySlots[i].sprite = item.itemSprite;
                inventorySlots[i].enabled = true;
                //inventoryTexts[i].text = itemInfo.itemAmount.ToString();
                inventoryTexts[i].text = "1";
                itemInfo._itemIndex = i;
                itemInfo.itemAmount = 1;
                inventoryTexts[i].enabled = true;

                ItemSlot slot = inventorySlots[i].GetComponent<ItemSlot>();
                if (slot == null)
                {
                    slot = inventorySlots[i].gameObject.AddComponent<ItemSlot>();
                }
                slot.itemInfo = itemInfo;
                //slot.craftPanel = craftPanel; // CraftPanel ����
                break;
            }
        }
    }
    internal void ItemDecline(ItemInfo iteminfo)
    {

            //if (inventorySlots[iteminfo._itemIndex].sprite == iteminfo.itemSprite)
            //{
                iteminfo.itemAmount--;
                inventoryTexts[iteminfo._itemIndex].text = iteminfo.itemAmount.ToString();
                if (iteminfo.itemAmount <= 0)
                {
                    inventorySlots[iteminfo._itemIndex].sprite = null;
                    inventoryTexts[iteminfo._itemIndex].enabled = false;
                    PlayerAction player = FindAnyObjectByType<PlayerAction>();
                    player.DropItem(iteminfo);
                }
            //}
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
