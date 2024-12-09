using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

internal class itemGrp
{
    internal string itemName;
    internal Sprite itemSprite;
    internal int itemCount;
    internal List<GameObject> itemList;
    public itemGrp(string n, Sprite s, int c, List<GameObject> l)
    {
        itemName = n;
        itemSprite = s;
        itemCount = c;
        itemList = l;
    }
}

public class PlayerAction : MonoBehaviour, IAttackable
{
    [SerializeField] LayerMask _combinedMask;
    public float _speed;
    float _h, _v;
    Rigidbody2D _rigid;
    Animator _anime;
    GameObject _scanObject;
    bool _hDown, _hUp;
    bool _vDown, _vUp;
    public Vector2 _dirVec;

    internal List<itemGrp> _bagList = new();

    bool _isAttacked;
    bool _isStun;

    bool isNewInput;
    Coroutine newInputCoroutine;
    Vector2 checkVector;

    #region 스킬 관련 필드
    [SerializeField] GameObject _skill;
    [SerializeField] internal float _skill0CoolTime = 3;
    float _skill0CoolDown;
    [SerializeField] GameObject _skill0;
    Vector2 _lastMoveDirection;
    Coroutine coolDownCoroutine;
    #endregion

    #region status 관련 필드
    [SerializeField] Slider _hpBar;
    [SerializeField] internal Slider _hungerBar;
    [SerializeField] internal Slider _thirstBar;
    public GameObject _equipment;
    [SerializeField] internal float _itemAtk;
    [SerializeField] float _atk;
    [SerializeField] float _def;
    float _realHp;
    float _resultHp;
    [SerializeField] float lossSpeed;
    [SerializeField] float _hungerTick;
    [SerializeField] float _thirstTick;
    [SerializeField] Image _portrait;
    [SerializeField] float _hungerByMove;
    [SerializeField] float minSpeedFactor = 0.1f;
    bool _isDead = false;
    #endregion

    private AudioSource audioSource;
    public AudioClip[] soundEffects;

    void Start()
    {
        if (GameManager._instance == null)
        {
            Debug.LogError("GameManager instance is null");
            return;
        }
        InitializePlayer();
    }


    void OnEnable()
    {
        InitializePlayer();
    }

    void InitializePlayer()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _rigid.interpolation = RigidbodyInterpolation2D.Interpolate;
        _anime = GetComponent<Animator>();
        _hungerBar.value = _hungerBar.maxValue;
        _thirstBar.value = _thirstBar.maxValue;
        _realHp = _hpBar.maxValue;
        _resultHp = _hpBar.maxValue;

        if (GameManager._instance != null)
        {
            GameManager._instance._playerAtkCollDownBar.maxValue = _skill0CoolTime;
            GameManager._instance._playerAtkCollDownBar.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("GameManager instance is null");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }


void Update()
    {
        living();

        warningLessValue();

        Move();


        if (_scanObject != null)
        {
            GameManager._instance._collectPanel.SetActive(true);
            GameManager._instance._collectPanel.GetComponentInChildren<Text>().text = _scanObject.GetComponent<MonobehaviourItem>()._ObjName + " 채집\n";
            GameManager._instance._collectPanel.GetComponentInChildren<Text>().text += _scanObject.GetComponent<MonobehaviourItem>()._requiredAttr == "" ||
                _scanObject.GetComponent<MonobehaviourItem>()._requiredAttr == null
                ? "" : _scanObject.GetComponent<MonobehaviourItem>()._requiredAttr + "필요";
        }
        else
        {
            GameManager._instance._collectPanel.SetActive(false);
        }
        // _dirVec의 방향으로 1.0f 만큼 떨어진 지점에서부터 raycast 시작
        Vector3 raycastStart = (Vector2)transform.position + _dirVec.normalized * 0.8f;

        Debug.DrawRay(raycastStart, _dirVec * 1.2f, new Color(0, 0, 0));


        //combinedMask = LayerMask.GetMask("Tree", "Water", "Soil", "Branch", "Stone"); //(1 << 4) | (1 << 7) | (1 << 9) | (1 << 10) | (1 << 11);
        RaycastHit2D hit = Physics2D.Raycast(raycastStart, _dirVec, 1.2f, _combinedMask);

        if (hit.collider != null)
        {
            _scanObject = hit.collider.gameObject;
        }
        else
        {
            _scanObject = null;
        }

        if (Input.GetKeyDown(KeyCode.E) && _scanObject != null)
        {

            if (_scanObject.GetComponent<MonobehaviourItem>()._requiredAttr != "" && _scanObject.GetComponent<MonobehaviourItem>()._requiredAttr != null)
            {
                if (_equipment == null)
                {
                    return;
                }
                if (_equipment.GetComponent<ItemInfo>().itemAttr.Contains(_scanObject.GetComponent<MonobehaviourItem>()._requiredAttr) == false)
                {
                    return;
                }
            }
            //scanObject.GetComponent<IObject>().DropObject(transform.position, hit.point, scanObject);
            _scanObject.GetComponent<IObject>().DropObject(transform.position, hit.point, _scanObject);
            //_ItemManager.DropObject(transform.position, hit.point, _scanObject);
            //manager.Collect(scanObject);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            GameManager._instance._manager.ToggleExpansion();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameManager._instance._craftScroll.ToggleScroll();
        }
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    _hpBar.GetComponent<Animator>().SetTrigger("Auch!");
        //    _portrait.GetComponent<Animator>().SetTrigger("Auch!");
        //    Resources.Load<MonobehaviourItem>("Prefab/bear");
        //}

        //Vector3 ve = new Vector3(h, v);
        //rigid.linearVelocity = new Vector2(h, v) * Speed;
        //transform.position = ve.normalized * Speed * Time.deltaTime + transform.position;

        Attack();
        GradualHpLoss();
    }
    //IEnumerator CheckPlayerHp()
    //{
    //    if (_hpBar.value / _hpBar.maxValue < 0.3f)
    //    {
    //        _portrait.GetComponent<Animator>().SetTrigger("Auch!");
    //    }
    //    yield return null;
    //}
    void living()
    {
        if (_hungerBar.value > 0)
        {
            _hungerBar.value -= _hungerTick * Time.deltaTime;
        }
        else
        {
            _hpBar.value -= _hungerTick * Time.deltaTime;
        }
        if (_thirstBar.value > 0)
        {
            _thirstBar.value -= _thirstTick * Time.deltaTime;
        }
        else
        {
            _hpBar.value -= _thirstTick * Time.deltaTime;
        }

    }
    void warningLessValue()
    {
        if (_hpBar.value <= _hpBar.maxValue * 0.3f)
        {
            _hpBar.GetComponent<Animator>().SetTrigger("Auch!");
        }
        if (_hungerBar.value <= _hungerBar.maxValue * 0.3f)
        {
            _hungerBar.GetComponent<Animator>().SetTrigger("Auch!");
        }
        if (_thirstBar.value <= _thirstBar.maxValue * 0.3f)
        {
            _thirstBar.GetComponent<Animator>().SetTrigger("Auch!");
        }
    }
    void GradualHpLoss()
    {
        if (_resultHp != _realHp)
        {
            _resultHp = Mathf.MoveTowards(_resultHp, _realHp, lossSpeed * Time.deltaTime);
            if (Mathf.Abs(_realHp - _resultHp) <= 0.001) { _resultHp = _realHp; }
            _hpBar.value = _resultHp;
        }

        if (_hpBar.value <= _hpBar.minValue && !_isDead)
        {
            _isDead = true;

            audioSource.clip = soundEffects[2];
            audioSource.Play();

            // Light 색상 점진적 변경 시작
            StartCoroutine(FadeToBlack());

        }
    }

    IEnumerator FadeToBlack()
    {
        Light2D gameLight = FindAnyObjectByType<Light2D>();
        if (gameLight != null)
        {
            float duration = 2.0f; // 변경이 완료될 때까지 걸리는 시간 (초)
            Color initialColor = gameLight.color;
            Color targetColor = Color.black;
            float time = 0;

            while (time < duration)
            {
                gameLight.color = Color.Lerp(initialColor, targetColor, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            gameLight.color = targetColor; // 최종 색상 설정

            PlayerPrefs.SetFloat("passedTime", GameManager._instance.passedTime);
            SceneManager.LoadScene("EndScene");
        }
    }


    void Move()
    {
        _h = Input.GetAxisRaw("Horizontal");
        _v = Input.GetAxisRaw("Vertical");
        Vector2 curVector = new Vector2(_h, _v).normalized;
        _dirVec = curVector != Vector2.zero ? curVector : _dirVec;




        if (_anime.GetInteger("hAxisRaw") != _h)
        {
            _anime.SetBool("isChanged", true);
            _anime.SetInteger("hAxisRaw", (int)_h);
        }
        else if (_anime.GetInteger("vAxisRaw") != _v)
        {
            _anime.SetBool("isChanged", true);
            _anime.SetInteger("vAxisRaw", (int)_v);
        }
        else
        {
            _anime.SetBool("isChanged", false);
        }
    }

    void FixedUpdate()
    {
        //Vector3 ve = new Vector3(h, v);
        if (_isAttacked) { return; }
        Vector2 curVector = new Vector2(_h, _v);
        _rigid.linearVelocity = curVector * _speed * Mathf.Max(minSpeedFactor, (Mathf.Floor(_thirstBar.value / 50) >= 1 ? 1 : _thirstBar.value / 50));
        // 실질적인 플레이어 움직임 목마름이 50 미만이면 목마름 수치에 따라 이동속도 감소
        if (curVector != Vector2.zero)
        {
            _lastMoveDirection = curVector;
        }
        //transform.position = ve.normalized * Speed + transform.position;


    }
    public Vector3 _smoothCamera;  // SmoothDamp 메소드 참조용(ref) 변수

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (transform.parent != null) { return; }
        ItemInfo itemInfo = collision.GetComponent<ItemInfo>();
        if (itemInfo != null)
        {
            Debug.Log(collision.gameObject.name);
            collision.enabled = false;
            collision.GetComponent<SpriteRenderer>().sortingOrder = 3;
            StartCoroutine(ItemAbsorb(collision.transform));
            GetItem(collision);
        }
        //if ()
        //{
        //    collision.enabled = false;
        //    collision.GetComponent<SpriteRenderer>().sortingOrder = 3;
        //    StartCoroutine(ItemAbsorb(collision.transform));
        //    GetItem(collision);
        //}
    }
    internal IEnumerator ItemAbsorb(Transform itemPos)
    {
        Vector2 here = itemPos.position;
        float coolDown = 0;
        while (coolDown <= 1)
        {
            coolDown += Time.deltaTime;
            itemPos.position = Vector2.Lerp(here, transform.position, Mathf.Pow(coolDown, 2));
            itemPos.localScale = new Vector3(1 - coolDown, 1 - coolDown, 0.5f);
            yield return null;
        }

    }
    void GetItem(Collider2D collision)
    {
        bool isFound = false;
        itemGrp item = null;

        ItemInfo itemInfo = collision.GetComponent<ItemInfo>();

        for (int i = 0; i < _bagList.Count; i++)
        {
            if (_bagList[i].itemName == itemInfo.itemName)
            {
                _bagList[i].itemList.Add(collision.gameObject);
                _bagList[i].itemCount++;
                isFound = true;
                item = _bagList[i];
                break;
            }
        }
        if (itemInfo.itemAmount <= 0)
        {
            DropItem(itemInfo);
            return;
        }
        if (isFound != true)
        {
            collision.transform.SetParent(GameManager._instance._bag.transform);
            item = new itemGrp(itemInfo.itemName, itemInfo.itemSprite, 1, new List<GameObject>() { collision.gameObject });
            _bagList.Add(item);

        }
        GameManager._instance._manager.Collect(item, itemInfo);
    }
    internal void GetItem(ItemInfo resultItem)
    {
        bool isFound = false;
        itemGrp item = null;

        ItemInfo itemInfo = resultItem.GetComponent<ItemInfo>();
        for (int i = 0; i < _bagList.Count; i++)
        {
            if (_bagList[i].itemName == itemInfo.itemName)
            {
                _bagList[i].itemList.Add(resultItem.gameObject);
                _bagList[i].itemCount++;
                isFound = true;
                item = _bagList[i];
                break;
            }
        }
        if (isFound == false)
        {
            resultItem.transform.SetParent(GameManager._instance._bag.transform);
            item = new itemGrp(itemInfo.itemName, itemInfo.itemSprite, 1, new List<GameObject>() { resultItem.gameObject });
            _bagList.Add(item);
        }
        GameManager._instance._manager.Collect(item, itemInfo);
    }
    internal void DropItem(ItemInfo itemInfo)
    {
        for (int i = 0; i < _bagList.Count; i++)
        {
            if (_bagList[i].itemSprite == itemInfo.itemSprite)
            {
                _bagList.RemoveAt(i);
                ObjectPool objectPool = FindAnyObjectByType<ObjectPool>();
                var child = GameManager._instance._bag.transform.GetChild(i);
                child.SetParent(objectPool.transform);
                break;
            }
        }
    }

    void Attack()
    {
        _skill0CoolDown += Time.deltaTime;
        GameManager._instance._playerAtkCollDownBar.value = _skill0CoolDown;
        if (Input.GetKeyDown(KeyCode.Space) && (_equipment == null ? _skill0CoolDown >
            _skill0CoolTime : _skill0CoolDown > _equipment.GetComponent<ItemInfo>()._itemAtkDelay))
        {
            StartCoroutine(AsyncAttack());
            if (coolDownCoroutine != null) { StopCoroutine(coolDownCoroutine); }
            audioSource.clip = soundEffects[0];
            audioSource.Play();
            coolDownCoroutine = StartCoroutine(ShowAtkCoolDown());
            _skill0CoolDown = 0;
            _hungerBar.value -= _hungerByMove;
        }

        IEnumerator AsyncAttack()
        {
            float _skillShotAngle = Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(Vector2.up, (_dirVec).normalized));
            if (_dirVec.x > 0)
            {
                _skillShotAngle = 360 - _skillShotAngle;
            }
            
            _skill.transform.rotation = Quaternion.Euler(0f, 0f, _skillShotAngle); _skill0.SetActive(true);
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(1.1f, 0.6f), _skill.transform.rotation.z, _dirVec, 0.6f, LayerMask.GetMask("Animal"));
            if (hit.collider != null)
            {
                hit.collider.GetComponent<IAttackable>().Attacked((Vector2)transform.position, 1.5f, _equipment == null ? _atk : _equipment.GetComponent<ItemInfo>()._itemAtk);
            }
            //GameManager._instance._player._equipment
            _isStun = true;
            yield return new WaitForSeconds(0.417f);
            _skill0.SetActive(false);
            yield return new WaitForSeconds(0.083f); _isStun = false;
        }

        IEnumerator ShowAtkCoolDown()
        {
            GameManager._instance._playerAtkCollDownBar.gameObject.SetActive(true);
            yield return new WaitForSeconds((_equipment == null ? _skill0CoolDown : _equipment.GetComponent<ItemInfo>()._itemAtkDelay) + 0.3f);
            GameManager._instance._playerAtkCollDownBar.gameObject.SetActive(false);
        }

    }


    //////////////////////////외부 접근 메소드/////////////////////////////
    public void Attacked(Vector2 attacker, float knockBackPower, float damage)
    {
        StartCoroutine(AsyncAttacked(attacker, knockBackPower, damage));
    }

    IEnumerator AsyncAttacked(Vector2 attacker, float knockBackPower, float damage)
    {
        _isAttacked = true;
        float knockTime = 0.4f;
        float knockCoolDown = 0;

        if (Mathf.Round((damage - _def) / _realHp * 100) >= 20)
        {
            float asdf = Mathf.Round((damage - _def) / _realHp * 100);
            _hpBar.GetComponent<Animator>().SetTrigger("Auch!");
            _portrait.GetComponent<Animator>().SetTrigger("Auch!");
            audioSource.clip = soundEffects[1];
            audioSource.Play();
        }
        _realHp -= (damage - _def);


        Vector2 startPos = transform.position;
        Vector2 endPos = (Vector2)transform.position + ((Vector2)transform.position - attacker).normalized * knockBackPower;

        while (knockCoolDown < knockTime)
        {
            knockCoolDown += Time.deltaTime;
            transform.position = Vector2.Lerp(startPos, endPos, knockCoolDown / knockTime);
            yield return null;
        }
        transform.position = endPos;

        _isAttacked = false;
    }
}
