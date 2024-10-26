using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Sprite[] itemSprites;

    Dictionary<string, List<GameObject>> itemPools = new Dictionary<string, List<GameObject>>();
    List<GameObject> TargetPool = new();

    void Awake()
    {
        Setting();
    }

    void Setting()
    {
        CreatePool("Tree", 100, itemSprites[0], "��������", "����", "�����");
        // �ٸ� ������ ������ ���� ������� �߰�
        // CreatePool("Stone", 100, anotherSprite, "��", "�ܴ���", "�� ������");
        CreatePool("Soil", 100, itemSprites[1], "��", "����");
        CreatePool("Water", 100, itemSprites[2], "��", "��ü", "���ñ�");
    }

    void CreatePool(string itemType, int count, Sprite sprite, params string[] properties)
    {
        List<GameObject> itemList = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            GameObject item = Instantiate(itemPrefab);
            item.transform.SetParent(transform);
            item.tag = itemType;
            item.SetActive(false);
            itemList.Add(item);

            ItemInfo itemInfo = item.GetComponent<ItemInfo>();
            itemInfo.SetProperties(sprite, 1, itemType,  properties);
            item.GetComponent<SpriteRenderer>().sprite = sprite;
        }
        itemPools[itemType] = itemList;
    }

    internal GameObject GetObject(string itemType)
    {
        if (itemPools.TryGetValue(itemType, out TargetPool))
        {
            for (int i = 0; i < TargetPool.Count; i++)
            {
                if (!TargetPool[i].activeSelf)
                {
                    TargetPool[i].SetActive(true);
                    return TargetPool[i];
                }
            }
        }
        return null;
    }
}
