using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerAction : MonoBehaviour
{
    public float Speed;
    float h, v;
    Rigidbody2D rigid;
    Animator anime;
    [SerializeField] GameObject camera; // 카메라 SerializeField로 설정
    float smoothSpeed = 0.125f;
    [SerializeField] GameObject collectPanel;
    [SerializeField] Text collectText; // UI Text 컴포넌트를 추가
    GameObject scanObject;
    [SerializeField] Manager manager;
    bool hDown, hUp;
    bool vDown, vUp;
    Vector3 dirVec;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.interpolation = RigidbodyInterpolation2D.Interpolate;
        anime = GetComponent<Animator>();
        camera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        vDown = Input.GetButtonDown("Vertical");
        vUp = Input.GetButtonUp("Vertical");
        hDown = Input.GetButtonDown("Horizontal");
        hUp = Input.GetButtonUp("Horizontal");

        // 방향 설정
        if (vDown && v == 1) { dirVec = Vector3.up; }
        else if (vDown && v == -1) { dirVec = Vector3.down; }
        else if (hDown && h == -1) { dirVec = Vector3.left; }
        else if (hDown && h == 1) { dirVec = Vector3.right; }
        else if (h != 0 && v != 0) { dirVec = new Vector3(h, v).normalized; }
        else if (hUp && v == 1) { dirVec = Vector3.up; }
        else if (hUp && v == -1) { dirVec = Vector3.down; }
        else if (vUp && h == 1) { dirVec = Vector3.right; }
        else if (vUp && h == -1) { dirVec = Vector3.left; }

        if (anime.GetInteger("hAxisRaw") != h)
        {
            anime.SetBool("isChanged", true);
            anime.SetInteger("hAxisRaw", (int)h); 
        }
        else if (anime.GetInteger("vAxisRaw") != v)
        {
            anime.SetBool("isChanged", true);
            anime.SetInteger("vAxisRaw", (int)v); 
        }
        else
        {
            anime.SetBool("isChanged", false);
        }

        if (scanObject != null)
        {
            collectText.text = scanObject.name + " 채집";
            collectPanel.SetActive(true);
        }
        else
        {
            collectPanel.SetActive(false);
        }

        Vector3 ve = new Vector3(h, v);
        //rigid.linearVelocity = new Vector2(h, v) * Speed;
        transform.position = ve.normalized * Speed * Time.deltaTime + transform.position;
    }

    void FixedUpdate()
    {
        //Vector3 ve = new Vector3(h, v);
        ////rigid.linearVelocity = new Vector2(h, v) * Speed;
        //transform.position = ve.normalized * Speed + transform.position;

        Debug.DrawRay(transform.position, dirVec * 1.2f, new Color(0, 0, 0));


        LayerMask combinedMask = (1 << 8) | (1 << 7) | (1 << 9);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirVec, 1.2f, combinedMask);

        if (hit.collider != null)
        {
            scanObject = hit.collider.gameObject;
        }
        else
        {
            scanObject = null;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            manager.Collect(scanObject);
        }
    }
    public Vector3 _smoothCamera;  // SmoothDamp 메소드 참조용(ref) 변수

    void LateUpdate()
    {
        if (camera != null)
        {
            Vector3 desiredPosition = transform.position;
            desiredPosition.z = camera.transform.position.z;
            Vector3 smoothedPosition = Vector3.Lerp(camera.transform.position, desiredPosition, smoothSpeed);
            camera.transform.position = smoothedPosition;

            // 매 프레임마다 카메라가 플레이어를 부드럽게 추적
            //camera.transform.position = Vector3.SmoothDamp(camera.transform.position, transform.position + Vector3.back * 10, ref _smoothCamera, 0.1f);
        }
    }
}
