using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemCombine : MonoBehaviour
{
    internal HashSet<CombineInfo> combinationRules;
    public bool isInitialized = false;

    void Awake()
    {
        InitializeCombinationRules();
        isInitialized = true;
    }
    void InitializeCombinationRules()
    {
        HashSet<string> axeProperty = new HashSet<string> { "����" };
        HashSet<string> pickAxeProperty = new HashSet<string> { "���" };
        HashSet<string> swordProperty = new HashSet<string> { "��", "��ī�ο�" };
        HashSet<string> fireProperty = new HashSet<string> { "��" };
        HashSet<string> doughProperty = new HashSet<string> { "����" };
        HashSet<string> brickProperty = new HashSet<string> { "����", "�ܴ���" };
        combinationRules = new HashSet<CombineInfo>
        {
            new CombineInfo("�ܴ���", "����", "Axe", axeProperty, Resources.Load<Sprite>("Art/Axe")),
            new CombineInfo("�ܴ���", "����", "Pickaxe", pickAxeProperty, Resources.Load<Sprite>("Art/Pickaxe")),
            new CombineInfo("�ܴ���", "����", "Sword", swordProperty, Resources.Load<Sprite>("Art/Sword")),
            new CombineInfo("����", "����", "Fire", fireProperty, Resources.Load<Sprite>("Art/Fire")),
            new CombineInfo("����", "��ü", "Dough", fireProperty, Resources.Load<Sprite>("Art/Dough")),
            new CombineInfo("����", "��", "Brick", fireProperty, Resources.Load<Sprite>("Art/Brick"))
        };
    }
    public ItemInfo Combine(ItemInfo mainItem, ItemInfo subItem, string selectedItem)
    {
        foreach (var rule in combinationRules)
        {
            if (mainItem.itemAttr.Contains(rule.MainProperty) == false)
            {
                continue;
            }
            if (subItem.itemAttr.Contains(rule.SubProperty) == false)
            {
                continue;
            }
            if (rule.Result != selectedItem)
            {
                continue;
            }
            //rule.ResultProperty.UnionWith(mainItem.itemAttr);
            foreach(var prop in mainItem.itemAttr)
            {
                rule.ResultProperty.Add(prop);
            }
            //rule.ResultProperty.UnionWith(subItem.itemAttr);
            foreach (var prop in subItem.itemAttr)
            {
                rule.ResultProperty.Add(prop);
            }
            GameObject CombinedItem = GameManager._instance._objPool.GetObject(rule.Result, "crafted");
            ItemInfo itemInfo = CombinedItem.GetComponent<ItemInfo>();
            itemInfo.SetProperties(CombinedItem.GetComponent<SpriteRenderer>().sprite, 1,
            rule.Result, ((mainItem._itemAtk + subItem._itemAtk) * 0.2f + itemInfo._itemAtk * 0.9f), rule.ResultProperty.ToArray());
            return itemInfo;
        }

        return null;
    }
}
