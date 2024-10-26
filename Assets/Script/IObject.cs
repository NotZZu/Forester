using UnityEngine;

public interface IObject
{
    void DropObject(Vector2 playerPos, Vector2 hitPos, GameObject hit);
}
