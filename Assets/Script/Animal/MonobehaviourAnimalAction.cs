using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonobehaviourAnimalAction : MonoBehaviour, IAttackable
{
    [SerializeField] float _detectRange;
    [SerializeField] int _playerLayer;
    bool _isPlayerDetected = false;
    Transform _playerTransform;
    [SerializeField] float _speed;
    [SerializeField] Animator _anime;
    Vector2 _spawnPosition;
    
    bool _isAttack;

    [SerializeField] float _wanderingMinX;
    [SerializeField] float _wanderingMaxX;
    [SerializeField] float _wanderingMinY;
    [SerializeField] float _wanderingMaxY;

    bool _lastDetect = false;
    float _hVectorThink;
    float _vVectorThink;
    float _thinkTime;
    Vector2 _targetPosition;
    Vector2 _nextMovePosition;
    Vector3 TargetPosition
    {
        get { return _targetPosition; }
        set
        {
            if (_homecomingLock == false)
            {
                _targetPosition = value;
            }
        }
    }
    bool _thinkLock;
    bool _homecomingLock;

    [SerializeField] LayerMask _obstacleLayer;

    [SerializeField] float _areaOutCount;
    float _areaOutCoolDown;
    [SerializeField] Vector2 _areaSize;

    float _homecomingCheckCount = 0.25f;
    float _homecomingCheckCoolDown;

    [SerializeField] Vector2 _boxForRay;

    bool _isAttacked;

    [SerializeField] float _attackTime; 
    [SerializeField] float _attackDelay;

    RaycastHit2D[] _hitArr = { };

    #region hp및 데미지 관련 필드
    [SerializeField] float _knockBackPower;
    Slider _hpBar;
    [SerializeField] float _atk;
    [SerializeField] float _atkCoolTime;
    [SerializeField] float _atkCoolDown;
    [SerializeField] float _def;
    Coroutine _hpBarCoroutine;
    float _realHp;
    float _resultHp;
    [SerializeField] float lossSpeed;
    [SerializeField] Slider _animalSlider;
    bool _isDead = false;
    #endregion



    void Awake()
    {
        _playerLayer = LayerMask.GetMask("Player");
        _obstacleLayer = LayerMask.GetMask("Obstacle_Collectible");
        _anime = GetComponent<Animator>();
        _spawnPosition = transform.position; // 스폰 위치 저장
        Think();
        _areaOutCoolDown = _areaOutCount;
        _homecomingCheckCoolDown = _homecomingCheckCount;
        _boxForRay = new Vector2(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y);
        _hpBar = GetComponentInChildren<Slider>();
        _hpBar.gameObject.SetActive(false);
        _realHp = _hpBar.maxValue;
        _resultHp = _hpBar.maxValue;
    }

    void Update()
    {
        if (_isDead == true) { return; }
        PlayerScanning();

        if (_isPlayerDetected == true)
        {
            TargetPosition = _playerTransform.position;
        }
        else if (_isPlayerDetected == false)
        {
            Think();
        }
        HomeComing();

        nextMoveSetting();

        Attacking();

        GradualHpLoss();

        Moving();

        AttackTry();

        Debug.DrawRay(transform.position, (_targetPosition - (Vector2)transform.position).normalized, Color.red);
        Debug.DrawRay(_targetPosition, Vector2.up, Color.blue);

    }

    void GradualHpLoss()
    {
        if (_resultHp != _realHp)
        {
            _resultHp = Mathf.MoveTowards(_resultHp, _realHp, lossSpeed * Time.deltaTime);
            if (Mathf.Abs(_realHp - _resultHp) <= 0.001) { _resultHp = _realHp; }
            _hpBar.value = _resultHp;
            if (_hpBar.value <= _hpBar.minValue) // 몬스터가 플레이어의 공격을 당해 죽으면
            {
                _anime.SetTrigger("Dead"); // 사망 애니메이션 출력
                _isDead = true; // 사망했음을 알림
                //gameObject.layer = LayerMask.GetMask("Obstacle_Collectible"); // 이후 통행불가-채집가능 레이어로 변경하여 아이템 채집 기능활성
                gameObject.layer = LayerMask.NameToLayer("Obstacle_Collectible");
                gameObject.AddComponent<DeadBodyItem>();
                gameObject.GetComponent<DeadBodyItem>().SetDropItem(ObjectPool.ItemType.Meat, ObjectPool.ItemType.Bone, ObjectPool.ItemType.Leather);
            }
        }
    }
    void nextMoveSetting()
    {
        //RaycastHit2D hit = Physics2D.BoxCast(transform.position, BoxForRay, 0, (targetPosition - transform.position), Vector2.Distance(targetPosition, transform.position), ObstacleLayer);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (_targetPosition - (Vector2)transform.position), Vector2.Distance(_targetPosition, transform.position), _obstacleLayer);


        if (hit.collider == null)
        {
            _nextMovePosition = _targetPosition;
            return;
        }
        else if (hit.collider != null)
        {
            //nextMovePosition = Vector2.Dot(hit.normal, (hit.point - (Vector2)transform.position)) * hit.normal;
            _nextMovePosition = Vector2.Reflect((hit.point - (Vector2)transform.position), hit.normal).normalized + hit.point;
        }
        Debug.DrawRay(hit.point, _nextMovePosition - hit.point, Color.blue);
    }

    void PlayerScanning()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, _detectRange, Vector2.zero, 0f, _playerLayer);

        if (hit.collider == null)
        {
            _isPlayerDetected = false;
            _targetPosition = _lastDetect == true ? transform.position : TargetPosition;
            _lastDetect = false;
        }
        else
        {
            //Debug.Log(hit.collider.name);
            _isPlayerDetected = true;
            _playerTransform = hit.transform;
            _lastDetect = true;
        }
    }

    void Think()
    {
        if (_thinkLock == false)
        {
            _thinkTime -= Time.deltaTime;
        }

        if (_thinkTime <= 0)
        {
            _thinkTime = Random.Range(1f, 2f);
            _hVectorThink = Random.Range(_wanderingMinX, _wanderingMaxX);
            _vVectorThink = Random.Range(_wanderingMinY, _wanderingMaxY);
            TargetPosition = new Vector2(_hVectorThink, _vVectorThink) + _spawnPosition;

        }

        //RaycastHit2D hit = Physics2D.BoxCast(transform.position, BoxForRay, 0, targetPosition - transform.position , Vector2.Distance(targetPosition, transform.position), ObstacleLayer);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (_targetPosition - (Vector2)transform.position), Vector2.Distance(_targetPosition, transform.position), _obstacleLayer);

        if (hit.collider != null)
        {
            TargetPosition = ((Vector2)transform.position - hit.point) - hit.point;
        }
    }

    void Attacking()
    {
        if (!_isAttack) { return; }
        _nextMovePosition = transform.position;
    }

    void Moving()
    {
        if (_isAttacked) { return; }
        if (Vector2.Distance(transform.position, _nextMovePosition) >= 0.07f)
        {
            Vector3 direction = (_nextMovePosition - (Vector2)transform.position).normalized;
            if (direction.x < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (direction.x > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }

            transform.position += direction * _speed * Time.deltaTime;
            //Vector2.MoveTowards(transform.position, nextMovePosition, Speed * Time.deltaTime);
            _anime.SetBool("isRunning", true);
            _thinkLock = true;
        }
        //else if (Vector2.Distance(transform.position, targetPosition) <= 1.4f && isPlayerDetected == true)
        //{
        //    anime.SetTrigger("Attacking");

        //}
        else
        {
            _anime.SetBool("isRunning", false);
            _thinkLock = false;
            _homecomingLock = false;

        }
    }
    void HomeComing()
    {
        if (_isPlayerDetected == true)
        {
            return;
        }
        _homecomingCheckCoolDown -= Time.deltaTime;
        if (_homecomingCheckCoolDown <= 0)
        {
            _hitArr = null;
            _hitArr = Physics2D.BoxCastAll(_spawnPosition, _areaSize, 0f, Vector3.zero, 0, LayerMask.GetMask("Animal"));
            _homecomingCheckCoolDown = _homecomingCheckCount;
        }
        if (_hitArr.Length != 0)
        {
            for (int i = 0; i < _hitArr.Length; i++)
            {
                if (_hitArr[i].collider.gameObject.Equals(gameObject))
                {
                    return;
                }
            }
        }
        _areaOutCoolDown -= Time.deltaTime;
        if (_areaOutCoolDown <= 0)
        {
            TargetPosition = _spawnPosition;
            _areaOutCoolDown = _areaOutCount;
            _homecomingLock = true;
        }
    }

    void AttackTry()
    {
        _atkCoolDown += Time.deltaTime;
        if (_isAttack) { return; }
        if (_atkCoolDown < _atkCoolTime) { return; }
        if (Vector2.Distance(transform.position, _targetPosition) <= 1.4f && _isPlayerDetected == true)
        {
            StartCoroutine(AsyncAttack());
            _atkCoolDown = 0;
        }

        IEnumerator AsyncAttack()
        {
            _anime.SetTrigger("Attacking");
            _isAttack = true;
            yield return new WaitForSeconds(_attackDelay);
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, _boxForRay, 0,
                _targetPosition - (Vector2)transform.position, 1.5f, _playerLayer);
            if(hit.collider != null)
            {
                hit.collider.GetComponent<IAttackable>().Attacked((Vector2)transform.position, _knockBackPower, _atk);
            }
            yield return new WaitForSeconds(_attackTime - _attackDelay);
            _isAttack = false;
        }
    }

    public void Attacked(Vector2 attacker, float knockBackPower, float damage)
    {
        if (_isDead == true) { return; }
        StartCoroutine(AsyncAttacked(attacker, knockBackPower, damage));
    }

    IEnumerator AsyncAttacked(Vector2 attacker, float knockBackPower, float damage)
    {
        _isAttacked = true;
        float knockTime = 0.4f;
        float knockCoolDown = 0;

        _realHp -= (damage - _def);
        if (Mathf.Round(_realHp / _hpBar.maxValue * 100) >= 20)
        {
            //_animalSlider.GetComponent<Animator>().SetTrigger("Auch!");
        }

        if (_hpBarCoroutine != null) { StopCoroutine(_hpBarCoroutine); }
        _hpBarCoroutine = StartCoroutine(HpBarShow());

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
    IEnumerator HpBarShow()
    {
        _hpBar.gameObject.SetActive(true);
        yield return new WaitForSeconds(3); // 대기상태로 진입하고 3초 이후 준비 상태로 진입
        _hpBar.gameObject.SetActive(false);
    }
}
