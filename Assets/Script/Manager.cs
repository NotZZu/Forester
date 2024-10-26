using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Manager : MonoBehaviour
{
    public Image[] inventorySlots; // 인벤토리 슬롯 UI 이미지를 담을 배열
    public Text[] inventoryTexts; // 인벤토리 슬롯 수량을 표시하는 텍스트 배열

    public bool isSlowMo = false;

    internal void Collect(itemGrp item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].sprite == item.itemSprite)
            {
                inventorySlots[i].sprite = item.itemSprite;
                inventorySlots[i].enabled = true;
                inventoryTexts[i].text = item.itemCount.ToString();
                inventoryTexts[i].enabled = true;
                return;
            }
            if (inventorySlots[i].sprite == null)
            {
                // 존재하지 않는 아이템이면 새로운 아이템 추가
                inventorySlots[i].sprite = item.itemSprite;
                inventorySlots[i].enabled = true;
                inventoryTexts[i].text = item.itemCount.ToString();
                inventoryTexts[i].enabled = true;
                break;
            }
        }
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
