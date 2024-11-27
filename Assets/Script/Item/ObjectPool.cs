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
        CreatePool(ItemType.Tree, 100, itemSprites[0], 3,  "����");
        CreatePool(ItemType.Soil, 100, itemSprites[1], 1 ,"��", "����");
        CreatePool(ItemType.Water, 100, itemSprites[2], 0.1f, "��", "��ü");
        CreatePool(ItemType.Stone, 100, itemSprites[3], 7, "�ܴ���", "��");
        CreatePool(ItemType.Branch, 100, itemSprites[4], 1.5f,  "��������", "����", "����");
        CreatePool(ItemType.Leaf, 100, itemSprites[5], 0.1f, "������");
        CreatePool(ItemType.Berry, 100, itemSprites[6],0.1f,  "����");
        CreatePool(ItemType.Axe, 100, itemSprites[7], 15,  "����");
        CreatePool(ItemType.Pickaxe, 100, itemSprites[8], 12f,  "���");
        CreatePool(ItemType.Sword, 100, itemSprites[9], 20f, "��");
        CreatePool(ItemType.Meat, 100, itemSprites[10], 1 , "���", "�Ŀ�");
        CreatePool(ItemType.Bone, 100, itemSprites[11], 4, "��", "�ܴ���", "����");
        CreatePool(ItemType.Leather, 100, itemSprites[12], 1, "����");
        CreatePool(ItemType.Fish, 100, itemSprites[13], 1, "�����", "�Ŀ�");
        CreatePool(ItemType.Coal, 100, itemSprites[14], 1, "��ź", "����");
        CreatePool(ItemType.Fire, 100, itemSprites[15], 10, "��", "�߰ſ�");
        CreatePool(ItemType.Dust, 100, itemSprites[16], 0.02f, "����", "����");
        CreatePool(ItemType.Sand, 100, itemSprites[17], 0.02f, "��", "����");
        CreatePool(ItemType.Dough, 100, itemSprites[18], 0.5f, "����");
        CreatePool(ItemType.Brick, 100, itemSprites[19], 9, "����", "�ܴ���");
    }

    void CreatePool(ItemType itemType, int count, Sprite sprite, float damage, params string[] properties)
    {
        List<GameObject> itemList = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            GameObject item = Instantiate(itemPrefab);
            item.transform.SetParent(transform);
            item.SetActive(false);
            itemList.Add(item);

            ItemInfo itemInfo = item.GetComponent<ItemInfo>();
            itemInfo.SetProperties(sprite, 1, itemType.ToString(), damage ,properties);
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
    internal GameObject GetObject(string itemType, string flag)
    {
        if (itemPools.TryGetValue(itemType, out TargetPool))
        {
            for (int i = 0; i < TargetPool.Count; i++)
            {
                if (!TargetPool[i].activeSelf)
                {
                    TargetPool[i].GetComponent<ItemInfo>()._isCrafted = true;
                    TargetPool[i].SetActive(true);
                    return TargetPool[i];
                }
            }
        }
        return null;
    }
}
