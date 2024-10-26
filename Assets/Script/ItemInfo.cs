using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    internal Sprite itemSprite;
    internal List<string> itemAttr = new();
    internal int itemAmount;
    internal string itemName;
    [SerializeField] CircleCollider2D circle;

    internal void SetProperties(Sprite s, int i, string n, params string[] l)
    {
        itemSprite = s;
        itemAmount = i;
        itemName = n;
        //foreach (var ll in l)
        //{
        //    itemAttr.Add(ll);
        //}
        itemAttr.AddRange(l);
    }
    private void OnEnable()
    {
        StartCoroutine(colliderAwake());
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
