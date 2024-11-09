using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemCombine : MonoBehaviour
{
    internal HashSet<CombineInfo> combinationRules;
    public ObjectPool objPool;
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
        combinationRules = new HashSet<CombineInfo>
        {
            new CombineInfo("막대", "단단함", "Axe", axeProperty, Resources.Load<Sprite>("Art/Axe")),
            new CombineInfo("막대", "단단함", "Pickaxe", pickAxeProperty, Resources.Load<Sprite>("Art/Pickaxe")),
            new CombineInfo("막대", "단단함", "Sword", swordProperty, Resources.Load<Sprite>("Art/Sword"))
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
            GameObject CombinedItem = objPool.GetObject(rule.Result);
            ItemInfo itemInfo = CombinedItem.GetComponent<ItemInfo>();
            itemInfo.SetProperties(CombinedItem.GetComponent<SpriteRenderer>().sprite, 1,
            rule.Result, rule.ResultProperty.ToArray());
            return itemInfo;
        }

        return null;
    }
}
