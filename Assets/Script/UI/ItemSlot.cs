using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemInfo itemInfo;
    //[SerializeField] internal CraftPanel craftPanel;
    bool toggle = false;

    void Awake()
    {
        // craftPanel = FindAnyObjectByType<CraftPanel>();
        var button = gameObject.AddComponent<Button>();
        button.onClick.AddListener(OnItemClick);

    }

    void OnItemClick()
    {

        if (GameManager._instance._craftPanel.IsMainMaterialSlotSelected || GameManager._instance._craftPanel.IsSubMaterialSlotSelected)
        {
            if (GameManager._instance._craftPanel.mainMaterialSlot.GetComponent<Image>().sprite != null &&
                GameManager._instance._craftPanel.subMaterialSlot.GetComponent<Image>().sprite != null)
            {
                return;
            }
            if (GameManager._instance._craftPanel.IsMainMaterialSlotSelected == true)
            {
                //if (itemInfo.itemAmount <= 1)
                //{
                //    GameManager._instance._manager.ItemDecline(itemInfo);
                //    return;
                //}
                if (GameManager._instance._craftPanel.SetMainMaterial(itemInfo))
                {
                    GameManager._instance._manager.ItemDecline(itemInfo);
                    if (itemInfo.itemAmount <= 1)
                    {
                        return;
                    }

                }
            }
            else if (GameManager._instance._craftPanel.IsSubMaterialSlotSelected == true)
            {
                //if (itemInfo.itemAmount <= 1)
                //{
                //    GameManager._instance._manager.ItemDecline(itemInfo);
                //    return;
                //}
                if (GameManager._instance._craftPanel.SetSubMaterial(itemInfo))
                {
                    GameManager._instance._manager.ItemDecline(itemInfo);
                    if (itemInfo.itemAmount <= 1)
                    {
                        return;
                    }
                }
            }

        }
        Equip();
    }
    public void Equip()
    {
        if (itemInfo.itemAttr.Contains("�Ŀ�") || itemInfo.itemAttr.Contains("����"))
        {
            GameManager._instance._player._hungerBar.value += itemInfo._hungerFill;
            GameManager._instance._player._thirstBar.value += itemInfo._thirstFill;
            GameManager._instance._manager.ItemDecline(itemInfo);
            return;
        }
        
        GameManager._instance._equipment.sprite = this.itemInfo.itemSprite;
        GameManager._instance._player._equipment = this.itemInfo.gameObject;
        GameManager._instance._playerAtkCollDownBar.maxValue = this.itemInfo._itemAtkDelay;
        GameManager._instance._equipment.gameObject.SetActive(true);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("����");
        GameManager._instance._propertyNotice.SetActive(true);
        Text properties = GameManager._instance._propertyNotice.GetComponentInChildren<Text>();
        properties.text = string.Join("\n", itemInfo.itemAttr);
        properties.text += "\n" + itemInfo._itemAtk;
        //GameManager._instance._propertyNotice.transform.position = Input.mousePosition;

        PositionPropertyBox();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("����");
        GameManager._instance._propertyNotice.SetActive(false);
    }
    private IEnumerator TogglePropertyBox()
    {
        toggle = !toggle;
        Text properties = GameManager._instance._propertyNotice.GetComponentInChildren<Text>();
        if (toggle)
        {
            properties.text = string.Join("\n", itemInfo.itemAttr);
            PositionPropertyBox();
        }
        else
        {
            properties.text = string.Empty;
        }
        foreach (Transform child in GameManager._instance._propertyNotice.transform)
        {
            child.gameObject.SetActive(toggle);
        }
        yield return null;
    }
    private void PositionPropertyBox()
    {
        RectTransform canvasRect = GameManager._instance._invenCan.GetComponent<RectTransform>();
        RectTransform noticeRect = GameManager._instance._propertyNotice.GetComponent<RectTransform>();
        Vector2 mousePos = Input.mousePosition; // ���콺 ��ġ�� ���
        Vector2 anchorPos = mousePos / canvasRect.lossyScale.x;
        float xPos = Mathf.Clamp(anchorPos.x, 0, canvasRect.rect.width - noticeRect.rect.width);
        float yPos = Mathf.Clamp(anchorPos.y, 0, canvasRect.rect.height - noticeRect.rect.height);

        // ������ ������ �� ����
        float baseOffsetX = noticeRect.rect.width * 1.3f; // ���ϴ� ���� ������ ��
        float baseOffsetY = noticeRect.rect.height;

        // �߾����κ����� �Ÿ� ���� ���
        float relativeY = Mathf.Abs(anchorPos.y - canvasRect.rect.height / 2) / (canvasRect.rect.height / 2);

        // lerp�� �̿��� �߾ӿ� �������� �������� �پ�鵵�� ����
        float offsetY = Mathf.Lerp(10f, baseOffsetY, relativeY);

        // x��ǥ ��ġ ����
        if (anchorPos.x > canvasRect.rect.width / 2)
        {
            xPos = Mathf.Clamp(xPos - baseOffsetX, 0, canvasRect.rect.width - noticeRect.rect.width);
        }
        else
        {
            xPos = Mathf.Clamp(xPos + baseOffsetX, 0, canvasRect.rect.width - noticeRect.rect.width);
        }

        // y��ǥ ��ġ ����
        if (anchorPos.y > canvasRect.rect.height / 2)
        {
            yPos = Mathf.Clamp(yPos - offsetY, 0, canvasRect.rect.height - noticeRect.rect.height);
        }
        else
        {
            yPos = Mathf.Clamp(yPos + offsetY, 0, canvasRect.rect.height - noticeRect.rect.height);
        }

        noticeRect.anchoredPosition = new Vector2(xPos, yPos);
    }


}
