using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : Singleton<Inventory>
{
    public enum SlotType
    {
        All,
        Equipment,
        Consumption,
        Etc,
    }

    Dictionary<string, string> itemDic;

    public SlotType slotType = SlotType.All;

    [SerializeField]
    Toggle[] selectItemType;
    [SerializeField]
    GameObject itemContent;
    
    public List<Item> itemGroupArr = new List<Item>();
    
    public PlayerStat playerStat;
    public PlayerHealth playerHealth;
    public Group portionGroup;
    private IEnumerator Start()
    {
        itemDic = new Dictionary<string, string>();

        Transform selectGroup = transform.GetChild(1).transform;
        selectItemType = new Toggle[selectGroup.childCount];

        for (int i = 0; i < selectGroup.childCount; i++)
        {
            selectItemType[i] = selectGroup.GetChild(i).GetComponent<Toggle>();
        }

        //itemGroupArr.Clear();

        for (int i = 0; i < itemContent.transform.childCount; i++)
        {
            if (itemContent.transform.GetChild(i).GetComponentInChildren<Item>() != null)
                itemGroupArr.Add(itemContent.transform.GetChild(i).GetComponentInChildren<Item>());
        }

        for (int i = 0; i < itemGroupArr.Count; i++)
        {
            itemGroupArr[i].transform.parent.SetSiblingIndex(i);
        }

        while (true)
        {
            yield return null;
            if (playerStat?.MapInpo == 0)
            {
                NewItem("1601", 1);
                NewItem("20", 5);
                transform.parent.gameObject.SetActive(false);
                break;
            }
        }
    }

    public void OnSelectSlotType(int _number)
    {
        slotType = (SlotType)_number;

        Item[] itemGroup = itemContent.GetComponentsInChildren<Item>(true);
        AudioManager.Instance.PlaySound("ItemSlot", Camera.main.transform.position);
        switch (slotType)
        {
            case SlotType.All:
                foreach(Item item in itemGroup)
                {
                    item.gameObject.transform.parent.gameObject.SetActive(true);
                }
                break;
            case SlotType.Equipment:
                foreach (Item item in itemGroup)
                {
                    if (item.itemInpos.type == Item.ItemType.Equipment)
                        item.gameObject.transform.parent.gameObject.SetActive(true);
                    else
                        item.gameObject.transform.parent.gameObject.SetActive(false);
                }
                break;
            case SlotType.Consumption:
                foreach (Item item in itemGroup)
                {
                    if (item.itemInpos.type == Item.ItemType.Consumption)
                        item.gameObject.transform.parent.gameObject.SetActive(true);
                    else
                        item.gameObject.transform.parent.gameObject.SetActive(false);
                }
                break;
            case SlotType.Etc:
                foreach (Item item in itemGroup)
                {
                    if (item.itemInpos.type == Item.ItemType.Etc)
                        item.gameObject.transform.parent.gameObject.SetActive(true);
                    else
                        item.gameObject.transform.parent.gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }

        ItemArrSort();
    }

    public void Time()
    {
        ItemArrSort();
    }

    public void ItemArrSort()
    {
        itemGroupArr.Clear();

        for (int i = 0; i < itemContent.transform.childCount; i++)
        {
            if (itemContent.transform.GetChild(i).GetComponentInChildren<Item>(true) != null)
            {
                itemGroupArr.Add(itemContent.transform.GetChild(i).GetComponentInChildren<Item>());
            }
        }

        for (int i = 0; i < itemGroupArr.Count; i++)
        {
            itemGroupArr[i].transform.parent.SetSiblingIndex(i);
        }
    }

    public void QuestItemCheck(Item item)
    {
        if (item.itemInpos.spritePath == 101)
        {
            ItemValue(item,1);
        }
        else if(item.itemInpos.spritePath == 103)
        {
            ItemValue(item, 2);
        }
        else if(item.itemInpos.spritePath == 102)
        {
            ItemValue(item, 21);
        }
        else if(item.itemInpos.spritePath == 106)
        {
            ItemValue(item, 26);
        }
    }

    public void QuestItemRemove(int spritePath, int count)
    {
        for (int i = 0; i < itemGroupArr.Count; i++)
        {
            if (itemGroupArr[i].itemInpos.spritePath == spritePath)
            {
                int itemRemoveCount = itemGroupArr[i].itemInpos.itemCount - count;
                itemGroupArr[i].itemInpos.itemCount = itemRemoveCount;
                if (itemGroupArr[i].itemInpos.itemCount <= 0)
                {
                    Destroy(itemGroupArr[i].gameObject);
                }
                break;
            }
        }
    }

    void ItemValue(Item item, int questNumber)
    {
        QuestList currnetQuest = null;
        int itemCount = 0;
        itemCount = item.itemInpos.itemCount;
        
        for (int i = 0; i < UIManager.Instance.playerStat.playerAbility.playerQuests.Count; i++)
        {
            if (UIManager.Instance.playerStat.playerAbility.playerQuests[i].questNubmer == questNumber && !UIManager.Instance.playerStat.playerAbility.playerQuests[i].isEnd)
            {
                currnetQuest = UIManager.Instance.playerStat.playerAbility.playerQuests[i];
                break;
            }
        }
        if (currnetQuest == null)
            return;

        UIManager.Instance.QuestPlus(currnetQuest, itemCount, true);
    }

    public void ItemTypeArrSort()
    {
        AudioManager.Instance.PlaySound("ItemSlot", Camera.main.transform.position);
        Debug.Log("Sort");
        itemGroupArr = itemGroupArr.OrderBy(x => ((int)x.itemInpos.type)).ThenBy(x => ((int)x.itemInpos.kinds)).ThenByDescending(x =>x.itemInpos.itemClass).ToList();
        for (int i = 0; i < itemGroupArr.Count; i++)
        {
            itemGroupArr[i].transform.parent.SetSiblingIndex(i);
        }
    }

    public void NewItem(string spritePath,int count)
    {
        Item newItem = Instantiate(Resources.Load<Item>("Prefabs/UI/ItemTest"));
        newItem.itemInpos = newItem.ItemInit(spritePath, count);
        newItem.GetComponent<Image>().sprite = Resources.Load<Sprite>(Utill.Instance.SpritePath(int.Parse(spritePath)));
        newItem.name = newItem.itemInpos.itemName;

        if (newItem.GetComponent<Item>().itemInpos.type == Item.ItemType.Equipment)
            ItemAdd(newItem);
        else
            ItemBundleAdd(newItem, count);

    }

    public void ItemAdd(Item addItem)
    {
        for (int i = 0; i < itemContent.transform.childCount; i++)
        {
            if (itemContent.transform.GetChild(i).childCount == 1)
            {
                addItem.transform.SetParent(itemContent.transform.GetChild(i).transform);
                addItem.transform.localScale = new Vector3(1, 1, 1);
                addItem.transform.position = itemContent.transform.GetChild(i).transform.position;
                break;
            }
        }
        UIManager.Instance.StartCoroutine(UIManager.Instance.InventoryReset(this));
        UIManager.Instance.WindowUpdate(1);
    }

    public void ItemBundleAdd(Item addItem, int count)
    {
        bool isGet = false;

        for (int i = 0; i < itemContent.transform.childCount; i++)
        {
            if (itemContent.transform.GetChild(i).childCount == 1)
                continue;

            if (addItem.itemInpos.itemName == itemContent.transform.GetChild(i).GetComponentInChildren<Item>(true).itemInpos.itemName)
            {
                Item plusItem = itemContent.transform.GetChild(i).GetComponentInChildren<Item>(true);
                plusItem.ItemCount += count;
                if(plusItem.itemInpos.isEquip)
                {
                    plusItem.itemCopy.GetComponent<Item>().ItemCount += count;
                    plusItem.itemQuick.GetComponent<Item>().ItemCount += count;
                }
                isGet = true;
                Destroy(addItem.gameObject);
                break;
            }
        }

        if(!isGet)
            ItemAdd(addItem);

        UIManager.Instance.StartCoroutine(UIManager.Instance.InventoryReset(this));
    }

    public void ItemUse(int slot)
    {
        Item itemSlot = portionGroup.portionSlot[slot].GetComponentInChildren<Item>(true);

        if (itemSlot == null)
            return;

        
        Item realCount = itemSlot.itemBody.GetComponent<Item>();
        Item copyCount = itemSlot.itemBody.GetComponent<Item>().itemCopy.GetComponent<Item>();
        copyCount.ItemCount--;
        realCount.ItemCount--;
        
        itemSlot.itemInpos.itemCount = realCount.itemInpos.itemCount;
        UsePotion(realCount);
        if (realCount.ItemCount == 0)
        {
            realCount.ItemDestroy();
        }

        UIManager.Instance.StartCoroutine(UIManager.Instance.InventoryReset(this));
    }

    string[] wearSetGroup = new string[4];

    public void UsePotion(Item _targetItem , bool isTime = true)
    {
        AudioManager.Instance.PlaySound("EatPotion", Camera.main.transform.position);

        if (_targetItem.itemInpos.wear.Contains(","))
        {
            wearSetGroup = _targetItem.itemInpos.wear.Split(',');
            for (int i = 0; i < wearSetGroup.Length; i++)
            {
                PotionCountCheck(wearSetGroup[i], isTime);
            }
        }
        else
        {
            PotionCountCheck(_targetItem.itemInpos.wear, isTime);
        }
    }
    void PotionCountCheck(string wearSet, bool isTime)
    {
        string wearValue = "";

        if (wearSet.Contains("공격력"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (isTime)
                ItemValueSet(0, wearValue);
            else
                ItemValueSet(0, wearValue, false);
        }
        if (wearSet.Contains("방어력"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (isTime)
                ItemValueSet(1, wearValue);
            else
                ItemValueSet(1, wearValue, false);
        }
        if (wearSet.Contains("공격속도"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (isTime)
                ItemValueSet(2, wearValue);
            else
                ItemValueSet(2, wearValue, false);
        }
        if (wearSet.Contains("체력"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (isTime)
                ItemValueSet(3, wearValue);
            else
                ItemValueSet(3, wearValue, false);
        }
        if (wearSet.Contains("기력"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (isTime)
                ItemValueSet(4, wearValue);
            else
                ItemValueSet(4, wearValue, false);
        }
        if (wearSet.Contains("체력 재생"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (isTime)
                ItemValueSet(5, wearValue);
            else
                ItemValueSet(5, wearValue, false);
        }
        if (wearSet.Contains("기력 재생"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (isTime)
                ItemValueSet(6, wearValue);
            else
                ItemValueSet(6, wearValue, false);
        }
        if (wearSet.Contains("이동속도"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (isTime)
                ItemValueSet(7, wearValue);
            else
                ItemValueSet(7, wearValue, false);
        }
        if (wearSet.Contains("LIFE"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (isTime)
                ItemValueSet(8, wearValue);
            else
                ItemValueSet(8, wearValue, false);
        }
        if (wearSet.Contains("HP"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (isTime)
                ItemValueSet(9, wearValue);
            else
                ItemValueSet(9, wearValue, false);
        }
        if (wearSet.Contains("SP"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (isTime)
                ItemValueSet(10, wearValue);
            else
                ItemValueSet(10, wearValue, false);
        }
    }
    void ItemValueSet(int value, string _wearValue, bool active = true)
    {
        int.TryParse(_wearValue, out int itemValue);

        switch (value)
        {
            case 0:
                if (active)
                    playerStat.Attack += itemValue;
                else
                    playerStat.Attack -= itemValue;
                break;
            case 1:
                if (active)
                    playerStat.Defense += itemValue;
                else
                    playerStat.Defense -= itemValue;
                break;
            case 2:
                if (active)
                    playerStat.AttackSpeed += itemValue * 0.1f;
                else
                    playerStat.AttackSpeed -= itemValue * 0.1f;
                break;
            case 3:
                if (active)
                    playerStat.MaxHealth += itemValue;
                else
                    playerStat.MaxHealth -= itemValue;
                break;
            case 4:
                if (active)
                    playerStat.MaxStamina += itemValue;
                else
                    playerStat.MaxStamina -= itemValue;
                break;
            case 5:
                if (active)
                    playerStat.ReGen += itemValue * 0.1f;
                else
                    playerStat.ReGen -= itemValue * 0.1f;
                break;
            case 6:
                if (active)
                    playerStat.StaminaReGen += itemValue * 0.1f;
                else
                    playerStat.StaminaReGen -= itemValue * 0.1f;
                break;
            case 7:
                if (active)
                    playerStat.MoveSpeed += itemValue;
                else
                    playerStat.MoveSpeed -= itemValue;
                break;
            case 8:
                if (active)
                    playerStat.CurrentLife += itemValue;
                else
                    playerStat.CurrentLife += itemValue;
                break;
            case 9:
                if (active)
                    playerHealth.GiveHp(itemValue);
                else
                    playerHealth.GiveHp(itemValue);
                break;
            case 10:
                if (active)
                    playerHealth.GiveStaMina(itemValue);
                else
                    playerHealth.GiveStaMina(itemValue);
                break;
            default:
                break;
        }

    }

    public void SaveInventory()
    {
        playerStat.playerAbility.ownItem.Clear();
        for (int i = 0; i < itemGroupArr.Count; i++)
        {
            playerStat.playerAbility.ownItem.Add(itemGroupArr[i].itemInpos);
        }
    }

    public IEnumerator LoadInventory()
    { 
        for (int i = 0; i < playerStat.playerAbility.ownItem.Count; i++)
        {
            Item loadItem = Instantiate(Resources.Load<Item>("Prefabs/UI/ItemTest"));

            loadItem.itemInpos = playerStat.playerAbility.ownItem[i];
            itemGroupArr.Add(loadItem);
            loadItem.name = loadItem.itemInpos.itemName;
            loadItem.transform.SetParent(itemContent.transform.GetChild(i));
            loadItem.transform.localPosition = Vector2.zero;
            loadItem.transform.localScale = new Vector2(1, 1);
            loadItem.GetComponent<Image>().sprite = Resources.Load<Sprite>(Utill.Instance.SpritePath(loadItem.itemInpos.spritePath));
            if(loadItem.itemInpos.isEquip)
            {
                while(true)
                {
                    if (loadItem.inventoryWindow != null)
                    {
                        loadItem.ConsumptionEquip();
                        break;
                    }
                    yield return null;
                }
            }
        }
        ItemArrSort();
        transform.parent.gameObject.SetActive(false);
    }
}
