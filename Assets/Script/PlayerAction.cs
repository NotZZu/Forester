using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
public class PlayerAction : MonoBehaviour
{
    public float Speed;
    float h, v;
    Rigidbody2D rigid;
    Animator anime;
    [SerializeField] GameObject mCamera; // 카메라 SerializeField로 설정
    float smoothSpeed = 0.125f;
    [SerializeField] GameObject collectPanel;
    [SerializeField] Text collectText; // UI Text 컴포넌트를 추가
    GameObject scanObject;
    [SerializeField] Manager manager;
    bool hDown, hUp;
    bool vDown, vUp;
    Vector3 dirVec;

    internal List<itemGrp> Bag = new();
    [SerializeField] GameObject bag;

    [SerializeField] Item ItemManager;
    [SerializeField] CraftScroll craftScroll;

    [SerializeField] GameObject realBag;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.interpolation = RigidbodyInterpolation2D.Interpolate;
        anime = GetComponent<Animator>();
        mCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
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

        Debug.DrawRay(transform.position, dirVec * 1.2f, new Color(0, 0, 0));


        LayerMask combinedMask = (1 << 4) | (1 << 7) | (1 << 9) | (1 << 10) | (1 << 11);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirVec, 1.2f, combinedMask);

        if (hit.collider != null)
        {
            scanObject = hit.collider.gameObject;
        }
        else
        {
            scanObject = null;
        }

        if (Input.GetKeyDown(KeyCode.E) && scanObject != null)
        {
            //scanObject.GetComponent<IObject>().DropObject(transform.position, hit.point, scanObject);
            ItemManager.DropObject(transform.position, hit.point, scanObject);
            //manager.Collect(scanObject);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            manager.ToggleExpansion();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            craftScroll.ToggleScroll();
        }

        //Vector3 ve = new Vector3(h, v);
        //rigid.linearVelocity = new Vector2(h, v) * Speed;
        //transform.position = ve.normalized * Speed * Time.deltaTime + transform.position;
    }

    void FixedUpdate()
    {
        //Vector3 ve = new Vector3(h, v);
        rigid.linearVelocity = new Vector2(h, v) * Speed;
        //transform.position = ve.normalized * Speed + transform.position;


    }
    public Vector3 _smoothCamera;  // SmoothDamp 메소드 참조용(ref) 변수

    void LateUpdate()
    {
        if (mCamera != null)
        {
            Vector3 desiredPosition = transform.position;
            desiredPosition.z = mCamera.transform.position.z;
            Vector3 smoothedPosition = Vector3.Lerp(mCamera.transform.position, desiredPosition, smoothSpeed);
            mCamera.transform.position = smoothedPosition;

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
            itemPos.position = Vector2.Lerp(here, transform.position , Mathf.Pow(coolDown, 2));
            itemPos.localScale = new Vector3(1 - coolDown, 1 - coolDown, 0.5f);
            yield return null;
        }
        
    }
    void GetItem(Collider2D collision)
    {
        bool isFound = false;
        itemGrp item = null;
        
        ItemInfo itemInfo = collision.GetComponent<ItemInfo>();

        for (int i = 0; i < Bag.Count; i++)
        {
            if (Bag[i].itemName == itemInfo.itemName)
            {
                Bag[i].itemList.Add(collision.gameObject);
                Bag[i].itemCount++;
                isFound = true;
                item = Bag[i];
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
            collision.transform.SetParent(bag.transform);
            item = new itemGrp(itemInfo.itemName, itemInfo.itemSprite, 1, new List<GameObject>() { collision.gameObject });
            Bag.Add(item);
            
        }
        manager.Collect(item, itemInfo);
    }
    internal void GetItem(ItemInfo resultItem)
    {
        bool isFound = false;
        itemGrp item = null;
        
        ItemInfo itemInfo = resultItem.GetComponent<ItemInfo>();
        for (int i = 0; i < Bag.Count; i++)
        {
            if (Bag[i].itemName == itemInfo.itemName)
            {
                Bag[i].itemList.Add(resultItem.gameObject);
                Bag[i].itemCount++;
                isFound = true;
                item = Bag[i];
                break;
            }
        }
        if (isFound == false)
        {
            resultItem.transform.SetParent(bag.transform);
            item = new itemGrp(itemInfo.itemName, itemInfo.itemSprite, 1, new List<GameObject>() { resultItem.gameObject });
            Bag.Add(item);
        }
        manager.Collect(item, itemInfo);
    }
    internal void DropItem(ItemInfo itemInfo)
    {
        for (int i = 0; i < Bag.Count; i++)
        {
            if (Bag[i].itemSprite == itemInfo.itemSprite)
            {
                Bag.RemoveAt(i);
                ObjectPool objectPool = FindAnyObjectByType<ObjectPool>();
                var child = realBag.transform.GetChild(i);
                child.SetParent(objectPool.transform);
                break;
            }
        }
    }
}
