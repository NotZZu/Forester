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
        HashSet<string> axeProperty = new HashSet<string> { "도끼" };
        HashSet<string> pickAxeProperty = new HashSet<string> { "곡괭이" };
        HashSet<string> swordProperty = new HashSet<string> { "검", "날카로움" };
        HashSet<string> fireProperty = new HashSet<string> { "불" };
        HashSet<string> doughProperty = new HashSet<string> { "반죽" };
        HashSet<string> brickProperty = new HashSet<string> { "벽돌", "단단함" };
        combinationRules = new HashSet<CombineInfo>
        {
            new CombineInfo("단단함", "막대", "Axe", axeProperty, Resources.Load<Sprite>("Art/Axe")),
            new CombineInfo("단단함", "막대", "Pickaxe", pickAxeProperty, Resources.Load<Sprite>("Art/Pickaxe")),
            new CombineInfo("단단함", "막대", "Sword", swordProperty, Resources.Load<Sprite>("Art/Sword")),
            new CombineInfo("연료", "나무", "Fire", fireProperty, Resources.Load<Sprite>("Art/Fire")),
            new CombineInfo("가루", "액체", "Dough", fireProperty, Resources.Load<Sprite>("Art/Dough")),
            new CombineInfo("반죽", "불", "Brick", fireProperty, Resources.Load<Sprite>("Art/Brick"))
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
