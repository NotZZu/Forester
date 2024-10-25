using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public Image[] inventoySlots; // �κ��丮 ���� UI �̹����� ���� �迭
    public Text[] inventoryTexts; // �κ��丮 ���� ������ ǥ���ϴ� �ؽ�Ʈ �迭

    private Dictionary<Sprite, int> collectedItems = new Dictionary<Sprite, int>(); // ������ �����۰� ������ �����ϴ� ��ųʸ�

    private Sprite soilSprite;
    private Sprite waterSprite;
    private Sprite woodSprite;

    void Start()
    {
        // �̹��� �ε� (��ο� Ȯ���� ����)
        soilSprite = Resources.Load<Sprite>("Art/soil");
        waterSprite = Resources.Load<Sprite>("Art/water");
        woodSprite = Resources.Load<Sprite>("Art/wood");
    }

    public void Collect(GameObject item)
    {
        Sprite itemSprite = null;
        if (item.name == "��")
        {
            itemSprite = soilSprite;
        }
        else if (item.name == "��")
        {
            itemSprite = waterSprite;
        }
        else if (item.name == "����")
        {
            itemSprite = woodSprite;
        }

        if (itemSprite != null)
        {
            // �̹� ������ ���������� Ȯ���ϰ�, ���� ����
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
