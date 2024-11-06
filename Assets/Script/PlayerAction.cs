using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public float _speed;
    float _h, _v;
    Rigidbody2D _rigid;
    Animator _anime;
    [SerializeField] GameObject _mCamera; // 카메라 SerializeField로 설정
    float _smoothSpeed = 0.125f;
    [SerializeField] GameObject _collectPanel;
    [SerializeField] Text _collectText; // UI Text 컴포넌트를 추가
    GameObject _scanObject;
    [SerializeField] Manager _manager;
    bool _hDown, _hUp;
    bool _vDown, _vUp;
    Vector2 _dirVec;

    internal List<itemGrp> _bagList = new();
    [SerializeField] GameObject _bag;

    [SerializeField] Item _ItemManager;
    [SerializeField] CraftScroll _craftScroll;

    [SerializeField] GameObject _realBag;

    bool _isAttacked;
    bool _isStun;

    bool isNewInput;
    Coroutine newInputCoroutine;
    Vector2 checkVector;

    #region 스킬 관련 필드
    [SerializeField] GameObject _skill;
    float _skill0CoolTime = 3;
    float _skill0CoolDown;
    [SerializeField] GameObject _skill0;
    Vector2 _lastMoveDirection;
    #endregion

    #region status 관련 필드
    [SerializeField] Slider _hpBar;
    [SerializeField] Slider _hungerBar;
    [SerializeField] Slider _thirstBar;
    //[SerializeField] float 
    [SerializeField] float _atk;
    [SerializeField] float _def;
    #endregion

    void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _rigid.interpolation = RigidbodyInterpolation2D.Interpolate;
        _anime = GetComponent<Animator>();
        _mCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        _hpBar.value = _hpBar.maxValue;
        _hungerBar.value = _hungerBar.maxValue;
        _thirstBar.value = _thirstBar.maxValue;
    }

    void Update()
    {
        Move();


        if (_scanObject != null)
        {
            _collectText.text = _scanObject.name + " 채집";
            _collectPanel.SetActive(true);
        }
        else
        {
            _collectPanel.SetActive(false);
        }

        Debug.DrawRay(transform.position, _dirVec * 1.2f, new Color(0, 0, 0));


        LayerMask combinedMask = LayerMask.GetMask("Tree", "Water", "Soil", "Branch", "Stone"); //(1 << 4) | (1 << 7) | (1 << 9) | (1 << 10) | (1 << 11);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _dirVec, 1.2f, combinedMask);

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
            //scanObject.GetComponent<IObject>().DropObject(transform.position, hit.point, scanObject);
            _ItemManager.DropObject(transform.position, hit.point, _scanObject);
            //manager.Collect(scanObject);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            _manager.ToggleExpansion();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            _craftScroll.ToggleScroll();
        }

        //Vector3 ve = new Vector3(h, v);
        //rigid.linearVelocity = new Vector2(h, v) * Speed;
        //transform.position = ve.normalized * Speed * Time.deltaTime + transform.position;

        Attack();
    }

    void Move()
    {
        //if (_isAttacked) { return; }
        /*_h = Input.GetAxisRaw("Horizontal");
        _v = Input.GetAxisRaw("Vertical");
        _vDown = Input.GetButtonDown("Vertical");
        _vUp = Input.GetButtonUp("Vertical");
        _hDown = Input.GetButtonDown("Horizontal");
        _hUp = Input.GetButtonUp("Horizontal");

        // 방향 설정
        if (_vDown && _v == 1) { _dirVec = Vector3.up; }
        else if (_vDown && _v == -1) { _dirVec = Vector3.down; }
        else if (_hDown && _h == -1) { _dirVec = Vector3.left; }
        else if (_hDown && _h == 1) { _dirVec = Vector3.right; }
        else if (_h != 0 && _v != 0) { _dirVec = new Vector3(_h, _v).normalized; }
        else if (_hUp && _v == 1) { _dirVec = Vector3.up; }
        else if (_hUp && _v == -1) { _dirVec = Vector3.down; }
        else if (_vUp && _h == 1) { _dirVec = Vector3.right; }
        else if (_vUp && _h == -1) { _dirVec = Vector3.left; }*/

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
        _rigid.linearVelocity = curVector * _speed; // 실질적인 플레이어 움직임
        if (curVector != Vector2.zero)
        {
            _lastMoveDirection = curVector;
        }
        //transform.position = ve.normalized * Speed + transform.position;


    }
    public Vector3 _smoothCamera;  // SmoothDamp 메소드 참조용(ref) 변수

    void LateUpdate()
    {
        if (_mCamera != null)
        {
            Vector3 desiredPosition = transform.position;
            desiredPosition.z = _mCamera.transform.position.z;
            Vector3 smoothedPosition = Vector3.Lerp(_mCamera.transform.position, desiredPosition, _smoothSpeed);
            _mCamera.transform.position = smoothedPosition;

            // 매 프레임마다 카메라가 플레이어를 부드럽게 추적
            //camera.transform.position = Vector3.SmoothDamp(camera.transform.position, transform.position + Vector3.back * 10, ref _smoothCamera, 0.1f);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        ItemInfo itemInfo = collision.GetComponent<ItemInfo>();
        if (itemInfo != null)
        {
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
    IEnumerator ItemAbsorb(Transform itemPos)
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
            collision.transform.SetParent(_bag.transform);
            item = new itemGrp(itemInfo.itemName, itemInfo.itemSprite, 1, new List<GameObject>() { collision.gameObject });
            _bagList.Add(item);

        }
        _manager.Collect(item, itemInfo);
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
            resultItem.transform.SetParent(_bag.transform);
            item = new itemGrp(itemInfo.itemName, itemInfo.itemSprite, 1, new List<GameObject>() { resultItem.gameObject });
            _bagList.Add(item);
        }
        _manager.Collect(item, itemInfo);
    }
    internal void DropItem(ItemInfo itemInfo)
    {
        for (int i = 0; i < _bagList.Count; i++)
        {
            if (_bagList[i].itemSprite == itemInfo.itemSprite)
            {
                _bagList.RemoveAt(i);
                ObjectPool objectPool = FindAnyObjectByType<ObjectPool>();
                var child = _realBag.transform.GetChild(i);
                child.SetParent(objectPool.transform);
                break;
            }
        }
    }

    void Attack()
    {
        _skill0CoolDown += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && _skill0CoolDown > _skill0CoolTime)
        {
            StartCoroutine(AsyncAttack());
            _skill0CoolDown = 0;
        }

        IEnumerator AsyncAttack()
        {
            float _skillShotAngle = Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(Vector2.up, (_dirVec).normalized));
            if (_dirVec.x > 0) { _skillShotAngle = 360 - _skillShotAngle; }
            _skill.transform.rotation = Quaternion.Euler(0f, 0f, _skillShotAngle);
            _skill0.SetActive(true);
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(1.1f, 0.6f), _skill.transform.rotation.z,
                _dirVec, 0.6f, LayerMask.GetMask("Animal"));
            if (hit.collider != null)
            {
                hit.collider.GetComponent<IAttackable>().Attacked((Vector2)transform.position, 1.5f, _atk);
            }
            _isStun = true;
            yield return new WaitForSeconds(0.417f);
            _skill0.SetActive(false);
            yield return new WaitForSeconds(0.083f);
            _isStun = false;
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
        _hpBar.value -= (damage - _def);
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
