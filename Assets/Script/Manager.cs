using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public Image[] inventoySlots; // 인벤토리 슬롯 UI 이미지를 담을 배열
    public Text[] inventoryTexts; // 인벤토리 슬롯 수량을 표시하는 텍스트 배열

    private Dictionary<Sprite, int> collectedItems = new Dictionary<Sprite, int>(); // 수집한 아이템과 수량을 저장하는 딕셔너리

    private Sprite soilSprite;
    private Sprite waterSprite;
    private Sprite woodSprite;

    void Start()
    {
        // 이미지 로드 (경로에 확장자 제거)
        soilSprite = Resources.Load<Sprite>("Art/soil");
        waterSprite = Resources.Load<Sprite>("Art/water");
        woodSprite = Resources.Load<Sprite>("Art/wood");
    }

    public void Collect(GameObject item)
    {
        Sprite itemSprite = null;
        if (item.name == "흙")
        {
            itemSprite = soilSprite;
        }
        else if (item.name == "물")
        {
            itemSprite = waterSprite;
        }
        else if (item.name == "나무")
        {
            itemSprite = woodSprite;
        }

        if (itemSprite != null)
        {
            // 이미 수집된 아이템인지 확인하고, 수량 증가
            if (collectedItems.ContainsKey(itemSprite))
            {
                collectedItems[itemSprite]++;
            }
            else
            {
                collectedItems[itemSprite] = 1;
            }
            UpdateInventoryUI();
        }
    }

    void UpdateInventoryUI()
    {
        int i = 0;
        foreach (var item in collectedItems)
        {
            if (i < inventoySlots.Length)
            {
                inventoySlots[i].sprite = item.Key;
                inventoySlots[i].enabled = true;
                inventoryTexts[i].text = item.Value.ToString();
                inventoryTexts[i].enabled = true;
                i++;
            }
        }
    }
}
