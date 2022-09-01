using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Item;

[System.Serializable]
public class ItemInpo
{
    public ItemType type;
    public string itemName;
    public ItemClass itemClass;
    public int limitLv;
    [TextArea(3, 10)]
    public string wear;
    [TextArea(3, 10)]
    public string explanation;
    public ItemKinds kinds = ItemKinds.None;
    public int itemCount = 1;
    public int spritePath;
    public int price = 0;
    public bool isEquip;
    public ItemInpo()
    {

    }

    public ItemInpo( string _Name , ItemType _type, ItemClass _itemClass, int _limitLv, ItemKinds _kinds,string _wear, string _explanation,int _spritePath, int _price)
    {
        itemName = _Name;
        type = _type;
        itemClass = _itemClass;
        limitLv = _limitLv;
        kinds = _kinds;
        wear = _wear;
        explanation = _explanation;
        itemCount = 1;
        spritePath = _spritePath;
        price = _price;
    }

    public ItemInpo Copy()
    {
        ItemInpo newItemInpo = new ItemInpo();

        newItemInpo.itemName = itemName;
        newItemInpo.type = type;
        newItemInpo.itemClass = itemClass;
        newItemInpo.limitLv = limitLv;
        newItemInpo.kinds = kinds;
        newItemInpo.wear = wear;
        newItemInpo.explanation = explanation;
        newItemInpo.itemCount = itemCount;
        newItemInpo.spritePath = spritePath;
        newItemInpo.price = price;

        return newItemInpo;
    }
}

public class Item : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    public enum ItemType
    {
        Equipment,
        Consumption,
        Etc,
    }
    public enum ItemClass
    {
        Normal,
        Common,
        Rare,
        Unique,
        Legend,
    }
    public enum ItemKinds
    {
        None,
        Sword,
        Helmet,
        Chest,
        Pants,
        Shoes,
        Gloves,
        Necklace,
        Ring,
        HPPortion,
        OtherConsum,
    }

    PointerEventData.InputButton btn1 = PointerEventData.InputButton.Left;
    PointerEventData.InputButton btn2 = PointerEventData.InputButton.Right;

    [SerializeField]
    Texture2D sellCursor;

    public ItemInpo itemInpos = new ItemInpo();

    //public ItemType type;
    //public string itemName;
    //public ItemClass itemClass;
    //public int limitLv;
    //[TextArea(3,10)]
    //public string wear;
    //[TextArea(3, 10)]
    //public string explanation;
    //public ItemKinds kinds = ItemKinds.None;
    //[System.NonSerialized]
    //private int itemCount = 1;

    public int ItemCount
    {
        get
        {
            return itemInpos.itemCount;
        }
        set
        {
            itemInpos.itemCount = value;
        }
    }

    public GameObject inventoryWindow;
    GameObject storeWindow;
    bool isEquipment;
    public bool isDragEnd;
    public bool isDropItem;
    Vector3 orignPos;

    [SerializeField]
    GameObject dummyParent;
    GameObject dummy;
    GameObject copyItem;
    GameObject itemCountBar;
    Text itemCountText;

    [SerializeField]
    
    public GameObject itemCopy;
    public GameObject itemQuick;
    Image quickImage;
    GameObject equipOJ;
    public GameObject itemBody { get; set; }

    [SerializeField]
    GameObject itemInpo;
    [SerializeField]
    StatTextGroup euipmentGroup;

    void Start()
    {
        if (isDropItem)
            return;

        if (itemInpos.type != ItemType.Equipment)
        {
            itemCountBar = transform.GetChild(0).gameObject;
            itemCountText = itemCountBar.transform.GetChild(0).GetComponent<Text>();
            if (itemInpos.itemCount > 1)
            {
                itemCountText.text = itemInpos.itemCount.ToString();
                itemCountBar.SetActive(true);
            }
        }

        dummy = Resources.Load<GameObject>("Prefabs/UI/ItemDummy");
        quickImage = Resources.Load<Image>("Prefabs/UI/Quick");
        dummyParent = GameObject.Find("PlayerCanvas");
        itemInpo = dummyParent.transform.GetChild(2).gameObject;
        euipmentGroup = dummyParent.transform.GetChild(1).GetChild(0).GetComponent<StatTextGroup>();
        inventoryWindow = dummyParent.transform.GetChild(1).GetChild(1).gameObject;
        storeWindow = dummyParent.transform.GetChild(1).GetChild(4).gameObject;

    }

    public ItemInpo ItemInit(string itemInitName, int count)
    {
        int.TryParse(itemInitName, out int _itemInitName);

        itemInpos.itemName = DBManager.Instance.itemInpos[_itemInitName].itemName;
        itemInpos.itemClass = DBManager.Instance.itemInpos[_itemInitName].itemClass;
        itemInpos.itemCount = DBManager.Instance.itemInpos[_itemInitName].itemCount;
        itemInpos.type = DBManager.Instance.itemInpos[_itemInitName].type;
        itemInpos.wear = DBManager.Instance.itemInpos[_itemInitName].wear;
        itemInpos.explanation = DBManager.Instance.itemInpos[_itemInitName].explanation;
        itemInpos.limitLv = DBManager.Instance.itemInpos[_itemInitName].limitLv;
        itemInpos.kinds = DBManager.Instance.itemInpos[_itemInitName].kinds;
        itemInpos.spritePath = DBManager.Instance.itemInpos[_itemInitName].spritePath;
        itemInpos.price = DBManager.Instance.itemInpos[_itemInitName].price;

        itemInpos.itemCount = count;

        return itemInpos;
    }

    void Update()
    {
        ItemCountCheck();

        if (isDragEnd == true)
        {
            if (copyItem == null)
                return;

            if (copyItem.GetComponent<BoxCollider2D>().enabled == true)
                return;
            copyItem.transform.position = Vector2.Lerp(copyItem.transform.position, gameObject.transform.position, 0.2f);
            float dist = (gameObject.transform.position - copyItem.transform.position).magnitude;
            if(dist <= 1)
            {
                StartCoroutine(ItemReset());
            }
        }

        if (transform.parent == null)
            return;

        if(transform.parent.name.Equals("Slot"))
        {
            if (transform.childCount < 2)
                isEquipment = false;
            else
                isEquipment = true;
        }
        else
        {
            isEquipment = true;
        }
    }

    void ItemCountCheck()
    {
        if (itemInpos.type == ItemType.Equipment)
            return;

        if (isDropItem)
            return;

        if (itemInpos.itemCount != int.Parse(itemCountText.text))
        {
            if (itemInpos.itemCount > 1)
                itemCountBar.SetActive(true);
            else
            {
                itemCountBar.SetActive(false);
            }
            itemCountText.text = itemInpos.itemCount.ToString();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(storeWindow.activeSelf)
            Cursor.SetCursor(sellCursor, new Vector2(sellCursor.width / 2, sellCursor.height / 2), CursorMode.ForceSoftware);

        itemInpo.transform.position = transform.position;

        if (itemInpo.transform.localPosition.y <= -32)
            itemInpo.transform.GetChild(0).localPosition = new Vector2(itemInpo.transform.GetChild(0).localPosition.x, itemInpo.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
        else
            itemInpo.transform.GetChild(0).localPosition = new Vector2(itemInpo.transform.GetChild(0).localPosition.x, 0);
        itemInpo.SetActive(true);

        TextGroup inpo = itemInpo.GetComponent<TextGroup>();

        inpo.textGroup[0].text = Utill.Instance.SetItemName(itemInpos.itemClass,itemInpos.itemName);
        inpo.textGroup[1].text = itemInpos.type.ToString();
        inpo.textGroup[2].text = Utill.Instance.SetItemName(itemInpos.itemClass, itemInpos.itemClass.ToString());
        if (itemInpos.limitLv > Inventory.Instance.playerStat.Level)
            inpo.textGroup[3].text = "<color=#FF0000>" + itemInpos.limitLv.ToString() + "</color>";
        else
            inpo.textGroup[3].text = itemInpos.limitLv.ToString();
        inpo.textGroup[4].text = itemInpos.wear;
        inpo.textGroup[5].text = itemInpos.explanation;
        inpo.textGroup[6].text = (itemInpos.price * 0.25f).ToString("0");
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

    float currentTimeClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemInpos.spritePath == 1601)
            return;

        if(eventData.button == btn1)
        {
            if (eventData.clickCount == 2)
            {
                if (storeWindow.activeSelf)
                {
                    UIManager.Instance.PopUp_Sell_Active(itemInpos.itemName, itemInpos.itemClass, gameObject);
                }
                else
                {
                    if (itemBody == null)
                    {
                        // 인벤토리에서 아이템 쓰기
                        if (isEquipment == false)
                        {
                            if (itemInpos.type == ItemType.Consumption)
                            {
                                itemInpos.itemCount--;
                                Inventory.Instance.UsePotion(this, false);
                                Debug.Log("1");
                                if (itemInpos.itemCount == 0)
                                    Destroy(gameObject);
                            }
                            else if (itemInpos.type == ItemType.Etc)
                            {
                                return;
                            }
                            else if (itemInpos.type == ItemType.Equipment)
                            {
                                Equipment();
                            }
                        }
                        else
                        {
                            if (itemInpos.type == ItemType.Equipment)
                            {
                                Dismount();
                            }
                            else
                            {
                                itemInpos.itemCount--;
                                itemCopy.GetComponent<Item>().ItemCount--;
                                itemQuick.GetComponent<Item>().ItemCount--;
                                Inventory.Instance.UsePotion(this, false);
                                Debug.Log("2");
                                if (itemInpos.itemCount == 0)
                                    ItemDestroy();
                            }
                        }
                    }
                    else
                    {
                        itemBody.GetComponent<Item>().ItemCount--;
                        if (itemBody.GetComponent<Item>().ItemCount == 0)
                        {
                            ItemDestroy();
                        }
                    }
                }
            }
        }
        else if(eventData.button == btn2)
        {
            if (isEquipment == false)
            {
                if (storeWindow.activeSelf)
                {
                    UIManager.Instance.PopUp_Sell_Active(itemInpos.itemName, itemInpos.itemClass, gameObject);
                }
                else
                {
                    if (itemInpos.type == ItemType.Equipment)
                    {
                            Equipment();
                    }
                    else if (itemInpos.type == ItemType.Consumption)
                    {
                        ConsumptionEquip();
                    }
                }
            }
            else
            {
                if (itemInpos.type == ItemType.Equipment)
                {
                    Dismount();
                }
                else if (itemInpos.type == ItemType.Consumption)
                {
                    ConsumptionDismount();
                }
            }

            InventoryOrganize();

        }
    }

    public void InventoryOrganize()
    {
        Inventory inventory = inventoryWindow.GetComponentInChildren<Inventory>(true);
        inventory.ItemArrSort();
    }

    public void Equipment()
    {
        if (itemInpos.limitLv > Inventory.Instance.playerStat.Level)
            return;

        AudioManager.Instance.PlaySound("ItemEquip", Camera.main.transform.position);

        switch (itemInpos.kinds)
        {
            case ItemKinds.Sword:
                ItemType_Equip(0);
                break;
            case ItemKinds.Helmet:
                ItemType_Equip(1);
                break;
            case ItemKinds.Chest:
                ItemType_Equip(2);
                break;
            case ItemKinds.Pants:
                ItemType_Equip(3);
                break;
            case ItemKinds.Shoes:
                ItemType_Equip(4);
                break;
            case ItemKinds.Gloves:
                ItemType_Equip(5);
                break;
            case ItemKinds.Necklace:
                ItemType_Equip(6);
                break;
            case ItemKinds.Ring:
                ItemType_Equip(7);
                break;
        }
    }

    void ItemType_Equip(int type)
    {
        if (euipmentGroup.equipmentSlot[type].transform.GetChild(0).gameObject.activeSelf == false)
        {
            euipmentGroup.equipmentSlot[type].transform.GetChild(0).gameObject.SetActive(true);
            gameObject.transform.SetParent(euipmentGroup.equipmentSlot[type].transform.GetChild(0));
            gameObject.transform.position = euipmentGroup.equipmentSlot[type].transform.GetChild(0).transform.position;
            EquipmentCheck equipmentCheck = euipmentGroup.equipmentSlot[type].transform.GetChild(0).GetComponent<EquipmentCheck>();
            equipmentCheck.OnOffEquip();
        }
        else
        {
            GameObject itemChange = euipmentGroup.equipmentSlot[type].transform.GetChild(0).GetChild(0).gameObject;
            EquipmentCheck equipmentCheck = euipmentGroup.equipmentSlot[type].transform.GetChild(0).GetComponent<EquipmentCheck>();
            equipmentCheck.OnOffEquip(false);

            itemChange.transform.SetParent(gameObject.transform.parent);
            itemChange.transform.position = gameObject.transform.position;

            gameObject.transform.SetParent(euipmentGroup.equipmentSlot[type].transform.GetChild(0));
            gameObject.transform.position = euipmentGroup.equipmentSlot[type].transform.GetChild(0).transform.position;
            equipmentCheck = euipmentGroup.equipmentSlot[type].transform.GetChild(0).GetComponent<EquipmentCheck>();
            equipmentCheck.OnOffEquip();
        }
    }

    void Dismount()
    {
        Transform itemContent = inventoryWindow.GetComponentInChildren<ScrollRect>().content.transform;

        AudioManager.Instance.PlaySound("ItemDisEquip", Camera.main.transform.position);

        switch (itemInpos.kinds)
        {
            case ItemKinds.Sword:
                ItemType_Dism(0);
                break;
            case ItemKinds.Helmet:
                ItemType_Dism(1);
                break;
            case ItemKinds.Chest:
                ItemType_Dism(2);
                break;
            case ItemKinds.Pants:
                ItemType_Dism(3);
                break;
            case ItemKinds.Shoes:
                ItemType_Dism(4);
                break;
            case ItemKinds.Gloves:
                ItemType_Dism(5);
                break;
            case ItemKinds.Necklace:
                ItemType_Dism(6);
                break;
            case ItemKinds.Ring:
                ItemType_Dism(7);
                break;
        }
        for (int i = 0; i < itemContent.childCount; i++)
        {
            if (itemContent.GetChild(i).childCount == 1)
            {
                gameObject.transform.SetParent(itemContent.GetChild(i).transform);
                gameObject.transform.position = itemContent.GetChild(i).transform.position;
                break;
            }
        }
    }

    void ItemType_Dism(int type)
    {
        if (euipmentGroup.equipmentSlot[type].transform.GetChild(0).gameObject.activeSelf == true)
        {
            euipmentGroup.equipmentSlot[type].transform.GetChild(0).gameObject.SetActive(false);
        }
        EquipmentCheck equipmentCheck = euipmentGroup.equipmentSlot[type].transform.GetChild(0).GetComponent<EquipmentCheck>();
        equipmentCheck.OnOffEquip(false);
    }
    public void ConsumptionEquip()
    {
        itemInpos.isEquip = true;

        AudioManager.Instance.PlaySound("ItemPotionEquip", Camera.main.transform.position);

        if (itemInpos.kinds == ItemKinds.HPPortion)
        {
            Debug.Log(euipmentGroup);
            Item hpPortion = euipmentGroup.portionSlot[0].GetComponentInChildren<Item>();

            if (hpPortion == null)
            {
                ItemSlotAdd(0);
            }
            else
            {
                Transform itemContent = inventoryWindow.GetComponentInChildren<ScrollRect>().content.transform;
                GameObject itemChange = euipmentGroup.portionSlot[0].GetComponentInChildren<Item>().gameObject;
                GameObject itemChangeQuick = Inventory.Instance.portionGroup.portionSlot[0].GetComponentInChildren<Item>().gameObject;

                if (itemChange == itemCopy)
                    return;

                IsQuick[] quickIcon = itemContent.GetComponentsInChildren<IsQuick>();
                for (int i = 0; i < quickIcon.Length; i++)
                {
                    if (quickIcon[i].name.Contains("Quick_1"))
                    {
                        Debug.Log(quickIcon[i].transform.parent.name);
                        quickIcon[i].transform.parent.GetComponent<Item>().itemInpos.isEquip = false;
                        Destroy(quickIcon[i].gameObject);
                    }
                }

                Destroy(itemChange);
                Destroy(itemChangeQuick);



                ItemSlotAdd(0);
            }
        }
        else if (itemInpos.kinds == ItemKinds.OtherConsum)
        {
            Item otherConsum = euipmentGroup.portionSlot[1].GetComponentInChildren<Item>();

            if (otherConsum == null)
            {
                ItemSlotAdd(1);
            }
            else
            {
                Transform itemContent = inventoryWindow.GetComponentInChildren<ScrollRect>().content.transform;
                GameObject itemChange = euipmentGroup.portionSlot[1].GetComponentInChildren<Item>().gameObject;
                GameObject itemChangeQuick = Inventory.Instance.portionGroup.portionSlot[1].GetComponentInChildren<Item>().gameObject;

                if (itemChange == itemCopy)
                    return;

                Destroy(itemChange);
                Destroy(itemChangeQuick);

                IsQuick[] quickIcon = itemContent.GetComponentsInChildren<IsQuick>();
                for (int i = 0; i < quickIcon.Length; i++)
                {
                    if(quickIcon[i].name.Contains("Quick_2"))
                    {
                        quickIcon[i].transform.parent.GetComponent<Item>().itemInpos.isEquip = false;
                        Destroy(quickIcon[i].gameObject);
                    }
                }
                
                

                ItemSlotAdd(1);
            }
        }
    }

    void ItemSlotAdd(int count)
    {
        itemCopy = Instantiate(gameObject);
        itemQuick = Instantiate(itemCopy.gameObject);

        if (itemCopy != null)
        {
            equipOJ = Instantiate(quickImage.gameObject, transform.position, Quaternion.identity, transform);
            equipOJ.name = "Quick_" + count;
        }
        itemCopy.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        itemQuick.GetComponent<Image>().color = new Color(1, 1, 1, 1);

        itemCopy.GetComponent<Item>().ItemCount = itemInpos.itemCount;
        itemCopy.GetComponent<Item>().itemBody = gameObject;

        itemCopy.transform.SetParent(euipmentGroup.portionSlot[count].transform.GetChild(0));
        itemCopy.transform.position = euipmentGroup.portionSlot[count].transform.GetChild(0).transform.position;

        itemCopy.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        
        itemQuick.GetComponent<Item>().ItemCount = itemInpos.itemCount;
        itemQuick.GetComponent<Item>().itemBody = gameObject;

        itemQuick.transform.SetParent(Inventory.Instance.portionGroup.portionSlot[count].transform.GetChild(0));
        itemQuick.transform.SetAsFirstSibling();
        itemQuick.transform.position = Inventory.Instance.portionGroup.portionSlot[count].transform.GetChild(0).transform.position;

        itemQuick.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }
    void ConsumptionDismount()
    {
        itemInpos.isEquip = false;

        AudioManager.Instance.PlaySound("ItemPotionDisEquip", Camera.main.transform.position);

        if (itemInpos.kinds == ItemKinds.HPPortion)
        {

            GameObject itemChange = euipmentGroup.portionSlot[0].GetComponentInChildren<Item>().gameObject;
            GameObject itemChangeQuick = Inventory.Instance.portionGroup.portionSlot[0].GetComponentInChildren<Item>().gameObject;

            Destroy(itemChange);
            Destroy(itemChangeQuick);
        }
        else if (itemInpos.kinds == ItemKinds.OtherConsum)
        {
            GameObject itemChange = euipmentGroup.portionSlot[1].GetComponentInChildren<Item>().gameObject;
            GameObject itemChangeQuick = Inventory.Instance.portionGroup.portionSlot[1].GetComponentInChildren<Item>().gameObject;

            Destroy(itemChange);
            Destroy(itemChangeQuick);
        }

        if (itemBody == null)
        {
            Destroy(equipOJ);
        }
        else
        {
            Destroy(itemBody.GetComponent<Item>().equipOJ);
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemInpos.spritePath == 1601)
            return;

        AudioManager.Instance.PlaySound("ItemDrag", Camera.main.transform.position);
        isDragEnd = false;
        Debug.Log("드래그 시작");
        orignPos = transform.position;
        GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);

        copyItem = Instantiate(dummy, transform.position, Quaternion.identity, dummyParent.transform);
        copyItem.name = "ItemDummy";
        copyItem.GetComponent<ItemDummy>().item = gameObject.GetComponent<Item>();

        copyItem.GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemInpos.spritePath == 1601)
            return;

        copyItem.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemInpos.spritePath == 1601)
            return;

        Debug.Log("드래그 끝");

        if (isEquipment == false)
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                if (ItemCount == 1)
                    UIManager.Instance.PopUp_Inven_Active(itemInpos.itemName, itemInpos.itemClass, gameObject);
                else
                    UIManager.Instance.PopUp_Inven_Consum_Active(itemInpos.itemName, itemInpos.itemClass, gameObject);

                isDragEnd = false;
                Destroy(copyItem);
            }
            else
            {
                copyItem.GetComponent<BoxCollider2D>().enabled = true;
                isDragEnd = true;
                StartCoroutine(DragEndTime());

            }
        }
        else
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                UIManager.Instance.PopUp_Equip_Active(gameObject);
                isDragEnd = false;
                Destroy(copyItem);
            }
            else
            {
                copyItem.GetComponent<BoxCollider2D>().enabled = true;
                isDragEnd = true;
                StartCoroutine(DragEndTime());
            }
        }
    }

    IEnumerator DragEndTime()
    {
        yield return new WaitForSeconds(0.1f);
        copyItem.GetComponent<BoxCollider2D>().enabled = false;
    }

    IEnumerator ItemReset()
    {
        isDragEnd = false;
        GetComponent<Image>().color = new Color(1, 1, 1, 1f);

        RectTransform copyItemScale = copyItem.GetComponent<RectTransform>();
        
        for (int i = 0; i < 10; i++)
        {
            copyItemScale.transform.localScale = new Vector3(copyItemScale.transform.localScale.x - 0.02f, copyItemScale.transform.localScale.y - 0.02f, copyItemScale.transform.localScale.z - 0.02f);
            yield return new WaitForSeconds(0.01f);
        }
        AudioManager.Instance.PlaySound("ItemDragEnd", Camera.main.transform.position);
        Destroy(copyItem);
    }

    public void ItemDestroy()
    {
        if (itemInpos.kinds == ItemKinds.HPPortion)
        {
            if (euipmentGroup.portionSlot[0].transform.GetChild(0).childCount > 1)
            {
                GameObject itemChange = euipmentGroup.portionSlot[0].GetComponentInChildren<Item>().gameObject;
                GameObject itemChangeQuick = Inventory.Instance.portionGroup.portionSlot[0].GetComponentInChildren<Item>().gameObject;

                Destroy(itemChange);
                Destroy(itemChangeQuick);
            }
        }
        else if (itemInpos.kinds == ItemKinds.OtherConsum)
        {
            if (euipmentGroup.portionSlot[0].transform.GetChild(0).childCount > 1)
            {
                GameObject itemChange = euipmentGroup.portionSlot[1].GetComponentInChildren<Item>().gameObject;
                GameObject itemChangeQuick = Inventory.Instance.portionGroup.portionSlot[1].GetComponentInChildren<Item>().gameObject;

                Destroy(itemChange);
                Destroy(itemChangeQuick);
            }
        }
        inventoryWindow.GetComponentInChildren<Inventory>().Time();
        if(itemBody != null)
            Destroy(itemBody);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
        Destroy(itemCopy);
        Destroy(itemQuick);
    }


    private void OnDisable()
    {
        if (itemInpo != null)
            itemInpo.SetActive(false);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);

        if (copyItem != null)
        {
            GetComponent<Image>().color = new Color(1, 1, 1, 1f);
            isDragEnd = false;
            Destroy(copyItem);
        }
    }

}
