using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    internal float passedTime = 0;


    #region ���� �ν��Ͻ� �ʵ�
    //public Light _light;
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
    public Canvas _statusCan;
    public Canvas _craftCan;
    public Canvas _invenCan;
    public Canvas _gameOverCan;
    public Image _equipment;
    public Slider _playerAtkCollDownBar;
    public GameObject _bgm;
    #endregion

    public static GameManager _instance; // ���� �ν��Ͻ�
    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
//DontDestroyOnLoad(gameObject); // ���� ����Ǿ �ı����� �ʵ��� ����
        }
        else if (_instance != this)
        {
            //Destroy(gameObject); // ���� �ν��Ͻ��� ���� ��� ���� ������ �ν��Ͻ��� �ı�
            return;
        }

        InitializeGameManager();
    }



    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            if (_collectPanel == null)
            {
                _collectPanel = GameObject.Find("CollectPanel"); // ������Ʈ �̸��� �°� ����
            }

            // �߰��� �ʿ��� �ʵ带 �������� �ʱ�ȭ
        }
    }


    private void InitializeGameManager()
    {
        // ��� �ʵ尡 ������ �ʱ�ȭ�Ǿ����� Ȯ���ϰ�, ������ �ʵ带 �������� �Ҵ�
        if (_propertyNotice == null)
        {
            _propertyNotice = Instantiate(_prefab);
            _propertyNotice.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
            _propertyNotice.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            _propertyNotice.transform.SetParent(_invenCan.transform);
            _propertyNotice.gameObject.SetActive(true); // ������ �о�ͼ� Ȱ��ȭ
        }

        if (_itemInfo == null)
        {
            _itemInfo = FindAnyObjectByType<ItemInfo>();
        }

        // �߰��� �ʿ��� �ʱ�ȭ �۾��� ���⿡ �߰�
    }

    void Update()
    {
        if (_collectPanel == null && SceneManager.GetActiveScene().name == "MainScene")
        {
            _collectPanel = GameObject.Find("CollectPanel");
        }

        passedTime += Time.deltaTime;
    }

}
