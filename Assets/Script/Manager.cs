using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Manager : MonoBehaviour
{
    public Image[] inventorySlots; // �κ��丮 ���� UI �̹����� ���� �迭
    public Text[] inventoryTexts; // �κ��丮 ���� ������ ǥ���ϴ� �ؽ�Ʈ �迭

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
                // �������� �ʴ� �������̸� ���ο� ������ �߰�
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
