using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool isSelectCheck;
    public int plusitemCount;

    [SerializeField]
    Image itemSprite;
    [SerializeField]
    ItemInpo itemInpos;

    public GameObject itemInpo { private get; set; }

    bool IsSelectItem = false;
    private void Awake()
    {
        GameObject dummyParent = GameObject.Find("PlayerCanvas");
        itemInpo = dummyParent.transform.GetChild(2).gameObject;
        if (transform.parent.name.Contains("Select"))
            IsSelectItem = true;
    }

    public void QuestItemInit(int _questItemNumber)
    {
        int.TryParse(itemSprite.sprite.name, out _questItemNumber);
        itemInpos.itemName = DBManager.Instance.itemInpos[_questItemNumber].itemName;
        itemInpos.itemClass = DBManager.Instance.itemInpos[_questItemNumber].itemClass;
        itemInpos.itemCount = DBManager.Instance.itemInpos[_questItemNumber].itemCount;
        itemInpos.type = DBManager.Instance.itemInpos[_questItemNumber].type;
        itemInpos.wear = DBManager.Instance.itemInpos[_questItemNumber].wear;
        itemInpos.explanation = DBManager.Instance.itemInpos[_questItemNumber].explanation;
        itemInpos.limitLv = DBManager.Instance.itemInpos[_questItemNumber].limitLv;
        itemInpos.kinds = DBManager.Instance.itemInpos[_questItemNumber].kinds;
        itemInpos.spritePath = DBManager.Instance.itemInpos[_questItemNumber].spritePath;
        itemInpos.price = DBManager.Instance.itemInpos[_questItemNumber].price;

        itemInpos.itemCount = plusitemCount;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsSelectItem)
            return;

        if (!GetComponent<Toggle>().interactable)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!isSelectCheck)
            {
                isSelectCheck = true;
                
            }
            else
            {
                isSelectCheck = false;
            }
            transform.parent.GetComponent<SelectItem>().IsSelectOnStop();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        itemInpo.transform.position = transform.GetChild(0).position;

        if (itemInpo.transform.localPosition.y <= -32)
            itemInpo.transform.GetChild(0).localPosition = new Vector2(itemInpo.transform.GetChild(0).localPosition.x, itemInpo.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
        else
            itemInpo.transform.GetChild(0).localPosition = new Vector2(itemInpo.transform.GetChild(0).localPosition.x, 0);

        itemInpo.SetActive(true);

        TextGroup inpo = itemInpo.GetComponent<TextGroup>();

        inpo.textGroup[0].text = Utill.Instance.SetItemName(itemInpos.itemClass, itemInpos.itemName);
        inpo.textGroup[1].text = itemInpos.type.ToString();
        inpo.textGroup[2].text = itemInpos.itemClass.ToString();
        inpo.textGroup[3].text = itemInpos.limitLv.ToString();
        inpo.textGroup[4].text = itemInpos.wear;
        inpo.textGroup[5].text = itemInpos.explanation;
        inpo.textGroup[6].text = itemInpos.price.ToString("0");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
        TextGroup inpo = itemInpo.GetComponent<TextGroup>();

        for (int i = 0; i < inpo.textGroup.Count; i++)
        {
            inpo.textGroup[i].text = "";
        }
        itemInpo.SetActive(false);
    }

    public void QuestItemCom()
    {
        Item newItem = Instantiate(Resources.Load<Item>("Prefabs/UI/ItemTest"));
        newItem.itemInpos = itemInpos.Copy();
        newItem.name = newItem.itemInpos.itemName;
        string spritePath = "";

        if (newItem.itemInpos.spritePath <= 100)
        {
            spritePath = "Sprite/Item/Consumption/" + newItem.itemInpos.spritePath;
        }
        else if (newItem.itemInpos.spritePath > 100 && newItem.itemInpos.spritePath <= 1000)
        {
            spritePath = "Sprite/Item/Etc/" + newItem.itemInpos.spritePath;
        }
        else
        {
            if (newItem.itemInpos.spritePath > 1000 && newItem.itemInpos.spritePath <= 1100)
            {
                spritePath = "Sprite/Item/Equipment/00.Weapon/" + newItem.itemInpos.spritePath;
            }
            else if (newItem.itemInpos.spritePath > 1100 && newItem.itemInpos.spritePath <= 1200)
            {
                spritePath = "Sprite/Item/Equipment/01.Helmet/" + newItem.itemInpos.spritePath;
            }
            else if (newItem.itemInpos.spritePath > 1200 && newItem.itemInpos.spritePath <= 1300)
            {
                spritePath = "Sprite/Item/Equipment/02.Chest/" + newItem.itemInpos.spritePath;
            }
            else if (newItem.itemInpos.spritePath > 1300 && newItem.itemInpos.spritePath <= 1400)
            {
                spritePath = "Sprite/Item/Equipment/03.Pants/" + newItem.itemInpos.spritePath;
            }
            else if (newItem.itemInpos.spritePath > 1400 && newItem.itemInpos.spritePath <= 1500)
            {
                spritePath = "Sprite/Item/Equipment/04.Shoes/" + newItem.itemInpos.spritePath;
            }
            else if (newItem.itemInpos.spritePath > 1500 && newItem.itemInpos.spritePath <= 1600)
            {
                spritePath = "Sprite/Item/Equipment/05.Gloves/" + newItem.itemInpos.spritePath;
            }
            else if (newItem.itemInpos.spritePath > 1600 && newItem.itemInpos.spritePath <= 1700)
            {
                spritePath = "Sprite/Item/Equipment/06.Necklace/" + newItem.itemInpos.spritePath;
            }
            else if (newItem.itemInpos.spritePath > 1700 && newItem.itemInpos.spritePath <= 1800)
            {
                spritePath = "Sprite/Item/Equipment/07.Ring/" + newItem.itemInpos.spritePath;
            }
        }

        newItem.GetComponent<Image>().sprite = Resources.Load<Sprite>(spritePath);

        if (newItem.GetComponent<Item>().itemInpos.type == Item.ItemType.Equipment)
            Inventory.Instance.ItemAdd(newItem);
        else
            Inventory.Instance.ItemBundleAdd(newItem, plusitemCount);
    }

    private void OnDisable()
    {
        itemInpo.SetActive(false);
    }
}
