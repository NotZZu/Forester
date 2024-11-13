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
                    // 프리팹을 인스턴스화하여 현재 게임 오브젝트의 자식으로 설정
                    GameObject instantiatedPrefab = Instantiate(_prefabArray[i][randPrefab], randPos, Quaternion.identity, _grid.transform);

                    // Tilemap 중심 설정
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
            // 타일맵의 가장 긴 변의 길이 계산
            return Mathf.Max(tilemap.cellBounds.size.x, tilemap.cellBounds.size.y);
        }
        else
        {
            Renderer renderer = prefab.GetComponent<Renderer>();
            if (renderer != null)
            {
                // 타일맵이 아닌 경우, 렌더러의 바운드에서 가장 긴 변의 길이 계산
                return Mathf.Max(renderer.bounds.size.x, renderer.bounds.size.y, renderer.bounds.size.z);
            }
        }
        // 타일맵도 아니고 렌더러도 없는 경우 기본값으로 1 반환
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
            // 타일맵의 Bounds 가져오기
            BoundsInt bounds = tilemap.cellBounds;

            // 중앙 좌표 계산
            Vector3Int centerCell = new Vector3Int(bounds.xMin + bounds.size.x / 2, bounds.yMin + bounds.size.y / 2, 0);
            Vector3 centerPosition = tilemap.CellToWorld(centerCell);

            // 프리팹의 타일맵 위치를 중앙으로 설정
            prefab.transform.position = centerPosition;
        }
    }
}
