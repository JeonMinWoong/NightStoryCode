using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class SlotType : MonoBehaviour, IPointerClickHandler , IPointerEnterHandler, IPointerExitHandler
{
    public enum ItemSlot
    {
        Etc,
        Weapon = 1,
        Helmet,
        Chest,
        Pants,
        Shoes,
        Gloves,
        Necklace,
        Ring,
        Con,
        OtherConsum
    }

    [SerializeField]
    Image itemSprite;
    [SerializeField]
    Text nameText;
    [SerializeField]
    Text priceText;
    [SerializeField]
    Texture2D purchaseCursor;

    public ItemSlot itemSlot;
    public int itemSellNumber;
    public PlayerStat playerStat;
    public ItemInpo itemInpos;
    public GameObject itemInpo { private get; set; }

    PointerEventData.InputButton btn1 = PointerEventData.InputButton.Left;
    PointerEventData.InputButton btn2 = PointerEventData.InputButton.Right;

    private void Awake()
    {
        int.TryParse(itemSprite.sprite.name, out itemSellNumber);
    }

    public void ItemSlotSet()
    {
        itemInpos = DBManager.Instance.itemInpos[itemSellNumber];
        itemSlot = (ItemSlot)itemInpos.kinds;
        nameText.text = Utill.Instance.SetItemName(itemInpos.itemClass, itemInpos.itemName);
        priceText.text = itemInpos.price.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == btn1 && eventData.clickCount >= 2 || eventData.button == btn2)
        {
            int.TryParse(priceText.text, out int purchasePrice);
            if (playerStat.CoinGet >= purchasePrice)
            {
                Debug.Log("±∏¿‘ : " + nameText.text);
                UIManager.Instance.PopUp_Purchase_Active(itemInpos.itemName, itemInpos.itemClass, gameObject);
            }
            else
            {
                Debug.Log("µ∑ ∫Œ¡∑");
            }
            eventData.clickCount = 0;

        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(purchaseCursor, new Vector2(purchaseCursor.width / 2,purchaseCursor.height / 2), CursorMode.ForceSoftware);
        itemInpo.transform.position = transform.GetChild(0).position;

        if (itemInpo.transform.localPosition.y <= -32)
            itemInpo.transform.GetChild(0).localPosition = new Vector2(itemInpo.transform.GetChild(0).localPosition.x, itemInpo.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
        else
            itemInpo.transform.GetChild(0).localPosition = new Vector2(itemInpo.transform.GetChild(0).localPosition.x, 0);

        itemInpo.SetActive(true);

        TextGroup inpo = itemInpo.GetComponent<TextGroup>();

        inpo.textGroup[0].text = Utill.Instance.SetItemName(itemInpos.itemClass, itemInpos.itemName);
        inpo.textGroup[1].text = itemInpos.type.ToString();
        inpo.textGroup[2].text = Utill.Instance.SetItemName(itemInpos.itemClass, itemInpos.itemClass.ToString());
        if (itemInpos.limitLv > Inventory.Instance.playerStat.Level)
            inpo.textGroup[3].text = "<color=#FF0000>" + itemInpos.limitLv.ToString() + "</color>";
        else
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

    public void PurchaseItem()
    {
        Item newItem = Instantiate(Resources.Load<Item>("Prefabs/UI/ItemTest"));

        newItem.itemInpos = itemInpos.Copy();

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
            Inventory.Instance.ItemBundleAdd(newItem, newItem.ItemCount);
    }

    private void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
        itemInpo.SetActive(false);
    }
}
