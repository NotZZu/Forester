using System.Collections.Generic;
using System.Collections;
using UnityEngine;

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

    internal void SetProperties(Sprite s, int i, string n, float dmz, params string[] l)
    {
        _itemAtk = dmz;
        itemSprite = s;
        itemAmount = i;
        itemName = n;
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