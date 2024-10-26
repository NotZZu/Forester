using UnityEngine;
using System.Collections;

public class WaterItem : MonoBehaviour, IObject
{
    [SerializeField] ObjectPool objPool;
    [SerializeField] float Speed;

    public void DropObject(Vector2 playerPos, Vector2 hitPos, GameObject hit)
    {
        GameObject gameObj = objPool.GetObject(hit.name);
        StartCoroutine(ThrowItem(playerPos, hitPos, gameObj.transform));
    }
    IEnumerator ThrowItem(Vector2 playerPos, Vector2 hitPos, Transform itemTransform)
    {
        Vector2 ObjtoPlayer = playerPos - hitPos;
        ObjtoPlayer = Quaternion.AngleAxis(GetRandomAngle(), Vector3.forward) * ObjtoPlayer;
        ObjtoPlayer += hitPos;

        float coolDown = 0;
        float moveProgress = 0;
        while (coolDown <= 1)
        {
            coolDown += Time.deltaTime;
            moveProgress += (1 - moveProgress) * Time.deltaTime * Speed;
            itemTransform.position = Vector2.Lerp(hitPos, ObjtoPlayer, moveProgress);
            yield return null;
        }
    }
    float GetRandomAngle()
    {
        return Random.Range(45, -45.0f);
    }
}
