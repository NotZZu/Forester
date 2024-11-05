using System.Collections;
using UnityEngine;

public class MonobehaviourAnimalAction : MonoBehaviour, IAttackable
{
    [SerializeField] float _detectRange;
    [SerializeField] int _playerLayer;
    bool _isPlayerDetected = false;
    Transform _playerTransform;
    [SerializeField] float _speed;
    Animator _anime;
    Vector2 _spawnPosition;
    [SerializeField] float _knockBackPower;
    bool _isAttack;


    bool _lastDetect = false;
    float _hVectorThink;
    float _vVectorThink;
    float _thinkTime;
    bool _isThinking = false;
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

    int _obstacleLayer;

    [SerializeField] float _areaOutCount;
    float _areaOutCoolDown;
    [SerializeField] Vector2 _areaSize;

    float _homecomingCheckCount = 0.25f;
    float _homecomingCheckCoolDown;

    [SerializeField] Vector2 _boxForRay;

    bool _isAttacked;

    RaycastHit2D[] _hitArr = { };
    void Awake()
    {
        _playerLayer = LayerMask.GetMask("Player");
        _obstacleLayer = LayerMask.GetMask("Tree", "Water", "Stone");
        _anime = GetComponent<Animator>();
        _spawnPosition = transform.position; // 스폰 위치 저장
        Think();
        _areaOutCoolDown = _areaOutCount;
        _homecomingCheckCoolDown = _homecomingCheckCount;
        _boxForRay = new Vector2(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y);
    }

    void Update()
    {
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

        Moving();

        AttackTry();

        Debug.DrawRay(transform.position, (_targetPosition - (Vector2)transform.position).normalized, Color.red);
        Debug.DrawRay(_targetPosition, Vector2.up, Color.blue);

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
            Debug.Log(hit.collider.name);
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
            _anime.SetTrigger("Thinking");
        }

        if (_thinkTime <= 0)
        {
            _anime.SetTrigger("ThinkEnd");
            _thinkTime = Random.Range(1f, 2f);
            _hVectorThink = Random.Range(-10f, 10f);
            _vVectorThink = Random.Range(-10f, 10f);
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
        if (Vector2.Distance(transform.position, _nextMovePosition) >= 0.01f)
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
        if (_isAttack) { return; }
        if (Vector2.Distance(transform.position, _targetPosition) <= 1.4f && _isPlayerDetected == true)
        {
            StartCoroutine(AsyncAttack());
        }

        IEnumerator AsyncAttack()
        {
            _anime.SetTrigger("Attacking");
            _isAttack = true;
            yield return new WaitForSeconds(0.8f);
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, _boxForRay, 0,
                _targetPosition - (Vector2)transform.position, 1.5f, _playerLayer);
            if(hit.collider != null)
            {
                hit.collider.GetComponent<IAttackable>().Attacked((Vector2)transform.position, _knockBackPower);
            }
            yield return new WaitForSeconds(0.2f);
            _isAttack = false;
        }
    }

    public void Attacked(Vector2 attacker, float knockBackPower)
    {
        StartCoroutine(AsyncAttacked(attacker, knockBackPower));
    }

    IEnumerator AsyncAttacked(Vector2 attacker, float knockBackPower)
    {
        _isAttacked = true;
        float knockTime = 0.4f;
        float knockCoolDown = 0;
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
