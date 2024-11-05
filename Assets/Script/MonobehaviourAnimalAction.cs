using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AI;

public class MonobehaviourAnimalAction : MonoBehaviour/*, IAttackable*/
{
    [SerializeField] float detectRange;
    [SerializeField] int playerLayer;
    private bool isPlayerDetected = false;
    private Transform PlayerTransform;
    [SerializeField] float Speed;
    Animator anime;
    private Vector2 spawnPosition;
    

    private bool lastDetect = false;
    float hVectorThink;
    float vVectorThink;
    float ThinkTime;
    private bool isThinking = false;
    Vector2 targetPosition;
    Vector2 nextMovePosition;
    Vector3 TargetPosition
    {
        get { return targetPosition; }
        set
        {
            if (HomecomingLock == false)
            {
                targetPosition = value;
            }
        }
    }
    private bool ThinkLock;
    private bool HomecomingLock;

    private int ObstacleLayer;

    [SerializeField] float AreaOutCount;
    float AreaOutCoolDown;
    [SerializeField] Vector2 AreaSize;

    private float HomecomingCheckCount = 0.25f;
    private float HomecomingCheckCoolDown;

    [SerializeField] Vector2 BoxForRay;

    RaycastHit2D[] hits = { };
    void Awake()
    {
        playerLayer = LayerMask.GetMask("Player");
        ObstacleLayer = LayerMask.GetMask("Tree", "Water", "Stone");
        anime = GetComponent<Animator>();
        spawnPosition = transform.position; // 스폰 위치 저장
        Think();
        AreaOutCoolDown = AreaOutCount;
        HomecomingCheckCoolDown = HomecomingCheckCount;
        BoxForRay = new Vector2(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y);
    }

    void Update()
    {
        PlayerScanning();

        if (isPlayerDetected == true)
        {
            TargetPosition = PlayerTransform.position;
        }
        else if (isPlayerDetected == false)
        {
            Think();
        }
        HomeComing();

        nextMoveSetting();

        Moving();

        Debug.DrawRay(transform.position, (targetPosition - (Vector2)transform.position).normalized, Color.red);
        Debug.DrawRay(targetPosition, Vector2.up, Color.blue);
        
    }
    void nextMoveSetting()
    {
        //RaycastHit2D hit = Physics2D.BoxCast(transform.position, BoxForRay, 0, (targetPosition - transform.position), Vector2.Distance(targetPosition, transform.position), ObstacleLayer);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (targetPosition - (Vector2)transform.position), Vector2.Distance(targetPosition, transform.position), ObstacleLayer);
        

        if (hit.collider == null)
        {
            nextMovePosition = targetPosition;
            return;
        }
        else if (hit.collider != null)
        {
            //nextMovePosition = Vector2.Dot(hit.normal, (hit.point - (Vector2)transform.position)) * hit.normal;
            nextMovePosition = Vector2.Reflect((hit.point - (Vector2)transform.position), hit.normal).normalized + hit.point;
        }
        Debug.DrawRay(hit.point , nextMovePosition - hit.point, Color.blue);
    }

    void PlayerScanning()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, detectRange, Vector2.zero, 0f, playerLayer);

        if (hit.collider == null)
        {
            isPlayerDetected = false;
            targetPosition = lastDetect == true ? transform.position : TargetPosition;
            lastDetect = false;
        }
        else
        {
            Debug.Log(hit.collider.name);
            isPlayerDetected = true;
            PlayerTransform = hit.transform;
            lastDetect = true;
        }
    }

    void Think()
    {
        if (ThinkLock == false)
        {
            ThinkTime -= Time.deltaTime;
            anime.SetTrigger("Thinking");
        }

        if (ThinkTime <= 0)
        {
            anime.SetTrigger("ThinkEnd");
            ThinkTime = Random.Range(1f, 2f);
            hVectorThink = Random.Range(-10f, 10f);
            vVectorThink = Random.Range(-10f, 10f);
            TargetPosition = new Vector2(hVectorThink, vVectorThink) + spawnPosition;

        }

        //RaycastHit2D hit = Physics2D.BoxCast(transform.position, BoxForRay, 0, targetPosition - transform.position , Vector2.Distance(targetPosition, transform.position), ObstacleLayer);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (targetPosition - (Vector2)transform.position), Vector2.Distance(targetPosition, transform.position), ObstacleLayer);

        if (hit.collider != null)
        {
            TargetPosition = ((Vector2)transform.position - hit.point) - hit.point;
        }
    }
    void Moving()
    {
        if (Vector2.Distance(transform.position, nextMovePosition) >= 0.01f && isPlayerDetected == false)
        {
            Vector3 direction = (nextMovePosition - (Vector2)transform.position).normalized;
            if (direction.x < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (direction.x > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }

            transform.position += direction * Speed * Time.deltaTime;
            //Vector2.MoveTowards(transform.position, nextMovePosition, Speed * Time.deltaTime);
            anime.SetBool("isRunning", true);
            ThinkLock = true;
        }
        else if (Vector2.Distance(transform.position, targetPosition) <= 1.4f && isPlayerDetected == true)
        {
            anime.SetTrigger("Attacking");

        }
        else
        {
            anime.SetBool("isRunning", false);
            ThinkLock = false;
            HomecomingLock = false;
            
        }
    }
    void HomeComing()
    {
        if (isPlayerDetected == true)
        {
            return;
        }
        HomecomingCheckCoolDown -= Time.deltaTime;
        if (HomecomingCheckCoolDown <= 0)
        {
            hits = null;
            hits = Physics2D.BoxCastAll(spawnPosition, AreaSize, 0f, Vector3.zero, 0, LayerMask.GetMask("Animal"));
            HomecomingCheckCoolDown = HomecomingCheckCount;
        }
        if (hits.Length != 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.gameObject.Equals(gameObject))
                {
                    return;
                }
            }
        }
        AreaOutCoolDown -= Time.deltaTime;
        if (AreaOutCoolDown <= 0)
        {
            TargetPosition = spawnPosition;
            AreaOutCoolDown = AreaOutCount;
            HomecomingLock = true;
        }
    }

}
