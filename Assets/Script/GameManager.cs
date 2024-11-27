using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region 공용 인스턴스 필드
    public PlayerAction _player;
    public CraftPanel _craftPanel;
    public Manager _manager;
    public ItemInfo _itemInfo;
    public MonobehaviourItem _itemManager;
    public ObjectPool _objPool;
    public ItemCombine _combine;
    public GameObject _collectPanel;
    public GameObject _bag;
    public CraftScroll _craftScroll;
    public GameObject _scrollView;
    public ItemCombine _itemCombine;
    public GameObject _prefab;
    public GameObject _propertyNotice;
    public Canvas _invenCan;
    public Image _equipment;
    #endregion

    public static GameManager _instance; // 정적 인스턴스
    public void Awake()
    {
        _propertyNotice = Instantiate(_prefab);
        _propertyNotice.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
        _propertyNotice.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        _propertyNotice.transform.SetParent(_invenCan.transform);
        _propertyNotice.gameObject.SetActive(true); // 프리펩 읽어와서 비활성화 해주기

        _itemInfo = FindAnyObjectByType<ItemInfo>();
        if (_instance == null)
        {
            _instance = this; 
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 기존 인스턴스가 있을 경우 파괴
        }
    }
}
