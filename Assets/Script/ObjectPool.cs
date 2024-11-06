using UnityEngine;
using System.Collections.Generic;
using System;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] List<Sprite> itemSprites;

    internal Dictionary<string, List<GameObject>> itemPools = new Dictionary<string, List<GameObject>>();
    List<GameObject> TargetPool = new();

    public enum ItemType
    {
        Tree,
        Soil,
        Water,
        Stone,
        Branch,
        Leaf,
        Berry,
        Axe,
        Pickaxe,
        Sword
    }

    void Awake()
    {
        Setting();
    }

    void Setting()
    {
        CreatePool(ItemType.Tree, 100, itemSprites[0], "³ª¹«");
        CreatePool(ItemType.Soil, 100, itemSprites[1], "Èë", "°¡·ç");
        CreatePool(ItemType.Water, 100, itemSprites[2], "¹°", "¾×Ã¼");
        CreatePool(ItemType.Stone, 100, itemSprites[3], "´Ü´ÜÇÔ", "µ¹");
        CreatePool(ItemType.Branch, 100, itemSprites[4], "³ª¹µ°¡Áö", "³ª¹«", "¸·´ë");
        CreatePool(ItemType.Leaf, 100, itemSprites[5], "³ª¹µÀÙ");
        CreatePool(ItemType.Berry, 100, itemSprites[6], "¿­¸Å");
        CreatePool(ItemType.Axe, 100, itemSprites[7], "µµ³¢");
        CreatePool(ItemType.Pickaxe, 100, itemSprites[8], "°î±ªÀÌ");
        CreatePool(ItemType.Sword, 100, itemSprites[9], "°Ë");
    }

    void CreatePool(ItemType itemType, int count, Sprite sprite, params string[] properties)
    {
        List<GameObject> itemList = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            GameObject item = Instantiate(itemPrefab);
            item.transform.SetParent(transform);
            item.SetActive(false);
            itemList.Add(item);

            ItemInfo itemInfo = item.GetComponent<ItemInfo>();
            itemInfo.SetProperties(sprite, 1, itemType.ToString(),  properties);
            item.GetComponent<SpriteRenderer>().sprite = sprite;
        }
        itemPools[itemType.ToString()] = itemList;
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
