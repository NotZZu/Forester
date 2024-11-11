using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MonobehaviourItem : MonoBehaviour, IObject
{
    [SerializeField] protected ObjectPool objPool;
    [SerializeField] float Speed = 20;
    [SerializeField] protected List<ObjectPool.ItemType> _dropItemList;

    void Awake()
    {
        objPool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
    }
        public void DropObject(Vector2 playerPos, Vector2 hitPos, GameObject hit)
    {
        ObjectPool.ItemType randomDropItem = _dropItemList[Random.Range(0, _dropItemList.Count)];
        GameObject dropItem = objPool.GetObject(randomDropItem.ToString());

        if (dropItem == null)
        {
            Debug.Log("드롭할 아이템이 존재하지 않습니다");
            return;
        }

        StartCoroutine(ThrowItem(playerPos, hitPos, dropItem.transform));

    }

    IEnumerator ThrowItem(Vector2 playerPos, Vector2 hitPos, Transform itemTransform)
    {
        Vector2 direction = playerPos - hitPos;
        direction = Quaternion.AngleAxis(GetRandomAngle(), Vector3.forward) * direction;
        direction += hitPos;
        float coolDown = 0;
        float moveProgress = 0;

        while (coolDown <= 1)
        {
            coolDown += Time.deltaTime;
            moveProgress += (1 - moveProgress) * Time.deltaTime * Speed;
            itemTransform.position = Vector2.Lerp(hitPos, direction, moveProgress);
            yield return null;
        }
    }

    float GetRandomAngle()
    {
        return Random.Range(45, -45.0f);
    }
    public void SetDropItem(params ObjectPool.ItemType[] items)
    {
        objPool = FindAnyObjectByType<ObjectPool>();
        _dropItemList = new();
        _dropItemList.AddRange(items);
        //foreach (var i in items)
        //{
        //    _dropItemList.Add(i);
        //}
    }
}
