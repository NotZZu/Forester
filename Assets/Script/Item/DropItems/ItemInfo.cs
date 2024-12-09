using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;
using static UnityEditor.Progress;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] internal Sprite itemSprite;
    [SerializeField] internal List<string> itemAttr = new();
    [SerializeField] internal int itemAmount;
    [SerializeField] internal string itemName;
    internal bool _isCrafted;
    [SerializeField] internal CircleCollider2D circle;
    [SerializeField] internal int _itemIndex;
    [SerializeField] internal float _itemAtk;
    [SerializeField] internal float _itemAtkDelay;
    [SerializeField] internal float _itemKnockBack;
    [SerializeField] internal float _hungerFill;
    [SerializeField] internal float _thirstFill;

    internal void SetProperties(Sprite s, int i, string n, float dmz, float weight, float hungerFill, float thirstFill , params string[] l)
    {
        _itemAtk = dmz;
        itemSprite = s;
        itemAmount = i;
        itemName = n;
        _itemAtkDelay = weight <= 1 ? 1 : weight;
        _itemKnockBack = weight <= 3 ? 3 : weight;
        _hungerFill = hungerFill;
        _thirstFill = thirstFill;
        //foreach (var ll in l)
        //{
        //    itemAttr.Add(ll);
        //}
        foreach (var prop in l)
        {
            if (itemAttr.Contains(prop) == true)
            {
                continue;
            }
            itemAttr.Add(prop);
        }
        
    }
    internal void SetProperties(ItemData item)
    {
        _itemAtk = item.damage;
        itemSprite = item.sprite;
        itemAmount = 1;
        itemName = item.itemName;
        _itemAtkDelay = item.weight <= 1 ? 1 : item.weight;
        _itemKnockBack = item.weight <= 3 ? 3 : item.weight;
        _hungerFill = item.hungerFill;
        _thirstFill = item.thirstFill;
        //foreach (var ll in l)
        //{
        //    itemAttr.Add(ll);
        //}
        foreach (var prop in item.properties)
        {
            if (itemAttr.Contains(prop) == true)
            {
                continue;
            }
            itemAttr.Add(prop);
        }

    }
    private void OnEnable()
    {
        if (_isCrafted)
        {
            circle.enabled = true;
        }
        else
        {
            StartCoroutine(colliderAwake());

        }
    }
        private void OnDisable()
    {
        circle.enabled = false;
        circle.GetComponent<SpriteRenderer>().sortingOrder = 1;
    }
    IEnumerator colliderAwake()
    {
        yield return new WaitForSeconds(2);
        circle.enabled = true;
    }

}
public interface ICollectible
{
    void Collect();
    string GetItemName();
}