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
        Sword,
        Meat,
        Bone,
        Leather,
        Fish,
        Coal,
        Fire,
        Dust,
        Sand,
        Dough,
        Brick
    }

    void Awake()
    {
        Setting();
    }

    void Setting()
    {
        CreatePool(ItemType.Tree, 100, itemSprites[0], "����");
        CreatePool(ItemType.Soil, 100, itemSprites[1], "��", "����");
        CreatePool(ItemType.Water, 100, itemSprites[2], "��", "��ü");
        CreatePool(ItemType.Stone, 100, itemSprites[3], "�ܴ���", "��");
        CreatePool(ItemType.Branch, 100, itemSprites[4], "��������", "����", "����");
        CreatePool(ItemType.Leaf, 100, itemSprites[5], "������");
        CreatePool(ItemType.Berry, 100, itemSprites[6], "����");
        CreatePool(ItemType.Axe, 100, itemSprites[7], "����");
        CreatePool(ItemType.Pickaxe, 100, itemSprites[8], "���");
        CreatePool(ItemType.Sword, 100, itemSprites[9], "��");
        CreatePool(ItemType.Meat, 100, itemSprites[10], "����", "�Ŀ�");
        CreatePool(ItemType.Bone, 100, itemSprites[11], "��", "�ܴ���", "����");
        CreatePool(ItemType.Leather, 100, itemSprites[12], "����");
        CreatePool(ItemType.Fish, 100, itemSprites[13], "������", "�Ŀ�");
        CreatePool(ItemType.Coal, 100, itemSprites[14], "��ź", "����");
        CreatePool(ItemType.Fire, 100, itemSprites[15], "��", "�߰ſ�");
        CreatePool(ItemType.Dust, 100, itemSprites[16], "����", "����");
        CreatePool(ItemType.Sand, 100, itemSprites[17], "��", "����");
        CreatePool(ItemType.Dough, 100, itemSprites[18], "����");
        CreatePool(ItemType.Brick, 100, itemSprites[19], "����", "�ܴ���");
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
