using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    internal float passedTime = 0;


    #region 공용 인스턴스 필드
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

    public static GameManager _instance; // 정적 인스턴스
    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
//DontDestroyOnLoad(gameObject); // 씬이 변경되어도 파괴되지 않도록 설정
        }
        else if (_instance != this)
        {
            //Destroy(gameObject); // 기존 인스턴스가 있을 경우 새로 생성된 인스턴스를 파괴
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
                _collectPanel = GameObject.Find("CollectPanel"); // 오브젝트 이름에 맞게 수정
            }

            // 추가로 필요한 필드를 동적으로 초기화
        }
    }


    private void InitializeGameManager()
    {
        // 모든 필드가 적절히 초기화되었는지 확인하고, 누락된 필드를 동적으로 할당
        if (_propertyNotice == null)
        {
            _propertyNotice = Instantiate(_prefab);
            _propertyNotice.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
            _propertyNotice.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            _propertyNotice.transform.SetParent(_invenCan.transform);
            _propertyNotice.gameObject.SetActive(true); // 프리팹 읽어와서 활성화
        }

        if (_itemInfo == null)
        {
            _itemInfo = FindAnyObjectByType<ItemInfo>();
        }

        // 추가로 필요한 초기화 작업을 여기에 추가
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
