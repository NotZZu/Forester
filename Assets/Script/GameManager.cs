using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region ���� �ν��Ͻ� �ʵ�
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

    public static GameManager _instance; // ���� �ν��Ͻ�
    public void Awake()
    {
        _propertyNotice = Instantiate(_prefab);
        _propertyNotice.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
        _propertyNotice.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        _propertyNotice.transform.SetParent(_invenCan.transform);
        _propertyNotice.gameObject.SetActive(true); // ������ �о�ͼ� ��Ȱ��ȭ ���ֱ�

        _itemInfo = FindAnyObjectByType<ItemInfo>();
        if (_instance == null)
        {
            _instance = this; 
            DontDestroyOnLoad(gameObject); // ���� ����Ǿ �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // ���� �ν��Ͻ��� ���� ��� �ı�
        }
    }
}
