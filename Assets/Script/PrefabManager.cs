using UnityEngine;
using System.Collections.Generic;

public class PrefabManager : MonoBehaviour
{
    static GameObject[][] _prefabArray = new GameObject[4][];
    void Start()
    {
        _prefabArray[0] = Resources.LoadAll<GameObject>("Assets/Resources/Prefab/ToRead/WaterPrefab");
        _prefabArray[1] = Resources.LoadAll<GameObject>("Assets/Resources/Prefab/ToRead/SoilPrefab");
        _prefabArray[2] = Resources.LoadAll<GameObject>("Assets/Resources/Prefab/ToRead/CollectiblePrefab");
        _prefabArray[3] = Resources.LoadAll<GameObject>("Assets/Resources/Prefab/ToRead/AnimalPrefab");
    }
    static internal void PlacePrefab(Vector3 position)
    {
        if (Physics2D.BoxCast(position, new Vector2(40, 40), 0, Vector2.zero, 0, LayerMask.GetMask("Obstacle_Collectible", "Collectible")).collider != null)
        {
            return;
        }

        for (int i = 0; i <  _prefabArray.GetLength(0); i++)
        {
            float randX = Random.Range(position.x - 20, position.x + 20);
            float randY = Random.Range(position.y - 20, position.y + 20);
        }
    }
}
