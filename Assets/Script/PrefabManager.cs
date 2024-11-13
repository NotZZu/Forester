using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using System.Linq;

public class PrefabManager : MonoBehaviour
{
    static GameObject[][] _prefabArray = new GameObject[4][];
    static int[][] _prefabWeight = new int[4][];
    int[][] _prefabCount = new int[4][];
    float _maxCnt = 0;
    [SerializeField] Grid _grid;

    void Start()
    {
        _prefabArray[0] = Resources.LoadAll<GameObject>("Prefab/ToRead/WaterPrefab");
        _prefabArray[1] = Resources.LoadAll<GameObject>("Prefab/ToRead/SoilPrefab");
        _prefabArray[2] = Resources.LoadAll<GameObject>("Prefab/ToRead/CollectiblePrefab");
        _prefabArray[3] = Resources.LoadAll<GameObject>("Prefab/ToRead/AnimalPrefab");
        _prefabWeight[0] = new int[] {15, 10, 10, 10, 25, 5, 25 };
        _prefabWeight[1] = new int[] {15, 10, 10, 10, 25, 5, 25 };
        _prefabWeight[2] = new int[] {2, 3, 5, 25, 25, 5, 10, 5, 5, 5, 5, 5 };
        _prefabWeight[3] = new int[] {20, 20, 35, 25 };
        _prefabCount[0] = new int[] {0, 3};
        _prefabCount[1] = new int[] {0, 3};
        _prefabCount[2] = new int[] {10, 20};
        _prefabCount[3] = new int[] {1, 3};
        SearchPlace(new Vector3(20, 20));
        SearchPlace(new Vector3(-20, 20));
        SearchPlace(new Vector3(20, -20));
        SearchPlace(new Vector3(-20, -20));
    }

    internal void SearchPlace(Vector3 position)
    {
        if (Physics2D.BoxCast(position, new Vector2(30, 30), 0, Vector2.zero, 0, LayerMask.GetMask("Obstacle_Collectible", "Collectible")).collider != null)
        {
            return;
        }

        StartCoroutine(PlacePrefab(position));
    }
    IEnumerator PlacePrefab(Vector3 position)
    {
        for (int i = 0; i < _prefabArray.GetLength(0); i++)
        {
            int randCnt = Random.Range(_prefabCount[i][0], _prefabCount[i][1] + 1);
            for (int j = 0; j < randCnt; j++)
            {
                int randPrefab = GetRandomPrefab(_prefabWeight[i]);
                Vector3 randPos = new();
                while (_maxCnt <= 100)
                {
                    randPos = GetRandPos(position);
                    float maxLen = GetMaxLength(_prefabArray[i][randPrefab]);
                    RaycastHit2D hit = Physics2D.BoxCast(randPos, new Vector2(maxLen, maxLen),
                        0, Vector2.zero, 0, LayerMask.GetMask("Obstacle_Collectible", "Player"));

                    if (hit.collider == null)
                    {
                        break;
                    }
                    _maxCnt++;
                }
                _maxCnt = 0;


                if (_prefabArray[i][randPrefab] != null)
                {
                    // �������� �ν��Ͻ�ȭ�Ͽ� ���� ���� ������Ʈ�� �ڽ����� ����
                    GameObject instantiatedPrefab = Instantiate(_prefabArray[i][randPrefab], randPos, Quaternion.identity, _grid.transform);

                    // Tilemap �߽� ����
                    CenterTilemap(instantiatedPrefab);

                    //instantiatedPrefab.transform.position = randPos;

                }
            }
            yield return null;
        }
        
    }
    int GetRandomPrefab(int[] prefabWeight)
    {
        int totalWeight = prefabWeight.Sum();

        int randomValue = Random.Range(0, totalWeight);
        int sum = 0;

        for (int i = 0; i < prefabWeight.Length; i++)
        {
            sum += prefabWeight[i];
            if (randomValue < sum)
            {
                return i;
            }
        }
        return prefabWeight.Length - 1;
    }
    float GetMaxLength(GameObject prefab)
    {
        Tilemap tilemap = prefab.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            // Ÿ�ϸ��� ���� �� ���� ���� ���
            return Mathf.Max(tilemap.cellBounds.size.x, tilemap.cellBounds.size.y);
        }
        else
        {
            Renderer renderer = prefab.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Ÿ�ϸ��� �ƴ� ���, �������� �ٿ�忡�� ���� �� ���� ���� ���
                return Mathf.Max(renderer.bounds.size.x, renderer.bounds.size.y, renderer.bounds.size.z);
            }
        }
        // Ÿ�ϸʵ� �ƴϰ� �������� ���� ��� �⺻������ 1 ��ȯ
        return 1.0f;
    }
            Vector3 GetRandPos(Vector3 position)
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        float randX = Random.Range(position.x - 20, position.x + 20);
        float randY = Random.Range(position.y - 20, position.y + 20);

        return new Vector3(randX, randY, position.z);
    }

    void CenterTilemap(GameObject prefab)
    {
        Tilemap tilemap = prefab.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            // Ÿ�ϸ��� Bounds ��������
            BoundsInt bounds = tilemap.cellBounds;

            // �߾� ��ǥ ���
            Vector3Int centerCell = new Vector3Int(bounds.xMin + bounds.size.x / 2, bounds.yMin + bounds.size.y / 2, 0);
            Vector3 centerPosition = tilemap.CellToWorld(centerCell);

            // �������� Ÿ�ϸ� ��ġ�� �߾����� ����
            prefab.transform.position = centerPosition;
        }
    }
}
