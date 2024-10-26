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
        CreatePool("Tree", 100, itemSprites[0], "나뭇가지", "나무", "막대기");
        // 다른 아이템 종류도 같은 방식으로 추가
        // CreatePool("Stone", 100, anotherSprite, "돌", "단단함", "돌 아이템");
        CreatePool("Soil", 100, itemSprites[1], "흙", "가루");
        CreatePool("Water", 100, itemSprites[2], "물", "액체", "마시기");
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
