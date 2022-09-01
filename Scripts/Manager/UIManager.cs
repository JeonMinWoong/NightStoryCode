using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public Button startButton;
    public Button exitButton;

    public GameObject damageTextUI;
    public GameObject itemGetTextUI;
    public GameObject itemGetCanvas;

    public PlayerStat playerStat;
    public ButtonController buttonController;
    public QuestController questController;
    public QuestWindow questWindow;
    public SelectUI selectUI;
    public Setting settingUI;
    public CanvasGroup playerMemory;

    [SerializeField]
    Animator saveAnima;
    [SerializeField]
    GameObject playerPanels;

    GameObject[] window;

    GameObject currentItem;
    GameObject storeItem;

    GameObject pop_up_Inven;
    GameObject pop_up_Equip;
    GameObject pop_up_Inven_Consum;
    GameObject pop_up_Sell;
    GameObject pop_up_SellGroup;
    GameObject pop_up_Purchase;
    GameObject pop_up_PurchaseGroup;

    StoreController storeController;
    
    int throwCount;
    int sellCount;
    int purCount;

    public NpcUI npcUI { get; private set; }

    void OnEnable()
    {
        string _sceneName = GameManager.Instance.sceneName;

        if (_sceneName.Contains("Main"))
            MainMenuInit();
        else if (_sceneName.Contains("Map_"))
            InGameInit();

    }

    private void Start()
    {
        damageTextUI = transform.Find("DamageTextPool").gameObject;
        itemGetTextUI = transform.Find("ItemGetTextPool").gameObject;
        pop_up_Inven = transform.Find("UICanvas").GetChild(0).gameObject;
        pop_up_Equip = transform.Find("UICanvas").GetChild(1).gameObject;
        pop_up_Inven_Consum = transform.Find("UICanvas").GetChild(2).gameObject;
        pop_up_Sell = transform.Find("UICanvas").GetChild(3).gameObject;
        pop_up_SellGroup = transform.Find("UICanvas").GetChild(4).gameObject;
        pop_up_Purchase = transform.Find("UICanvas").GetChild(5).gameObject;
        pop_up_PurchaseGroup = transform.Find("UICanvas").GetChild(6).gameObject;
        itemGetCanvas = Resources.Load<GameObject>("Prefabs/UI/ItemGetCanvas").gameObject;
        npcUI = transform.Find("NpcUI").GetComponent<NpcUI>();
    }

    void MainMenuInit()
    {
        startButton = GameObject.Find("StartButton").GetComponent<Button>();
        exitButton = GameObject.Find("ExitButton (2)").GetComponent<Button>();

        if (startButton != null)
            startButton.onClick?.AddListener(OnClickStart);
        if (exitButton != null)
            exitButton.onClick?.AddListener(OnClickExit);
    }

    void InGameInit()
    {
        playerPanels = GameObject.Find("Panels")?.gameObject;

        if (playerPanels == null)
            return;

        playerStat = FindObjectOfType<PlayerStat>();

        window = new GameObject[playerPanels.transform.childCount + 1];

        for (int i = 0; i < playerPanels.transform.childCount; i++)
        {
            window[i] = playerPanels.transform.GetChild(i).gameObject;
        }
        window[5] = transform.GetChild(4).GetChild(0).gameObject;
        buttonController = FindObjectOfType<ButtonController>();
        storeController = window[4].GetComponent<StoreController>();
        questWindow = window[3].GetComponent<QuestWindow>();
        selectUI = transform.GetChild(6).GetComponent<SelectUI>();
    }

    public void OnClickStart()
    {
        AudioManager.Instance.PlaySound("MainMenuStart", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        DataManager.Instance.OnSaveUI();
    }

    public void OnClickExit()
    {
        AudioManager.Instance.PlaySound("MainMenuStart", Camera.main.transform.position);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ActiveWindow(int _number, string sNpc = null)
    {
        if (window[_number].activeSelf == false)
        {

            ButtonSound(_number, true);
            window[_number].SetActive(true);
            if (_number == 4)
            {
                window[1].SetActive(true);
                storeController.SetStoreType(sNpc);
            }
        }
        else
        {
            if (_number == 2)
            {
                SkillList skillList = window[_number].GetComponent<SkillList>();
                if (skillList.talents.activeSelf)
                {
                    skillList.SkillListEnd();
                    skillList.talents.SetActive(false);

                    return;
                }
            }
            ButtonSound(_number, false);
            window[_number].SetActive(false);
            if (_number == 4)
            {
                window[1].SetActive(false);
                pop_up_Sell.SetActive(window[_number].activeSelf);
                pop_up_SellGroup.SetActive(window[_number].activeSelf);
                pop_up_Purchase.SetActive(window[_number].activeSelf);
                pop_up_PurchaseGroup.SetActive(window[_number].activeSelf);

            }
        }
        WindowUpdate(_number,false);
    }

    public void ButtonSound(int _number, bool isActive = false)
    {
        if (isActive)
        {
            if (_number == 0)
                AudioManager.Instance.PlaySound("EquipOpen", Camera.main.transform.position);
            else if (_number == 1)
                AudioManager.Instance.PlaySound("InventoryOpen", Camera.main.transform.position);
            else if (_number == 2)
                AudioManager.Instance.PlaySound("SkillOpen", Camera.main.transform.position);
            else if (_number == 3)
                AudioManager.Instance.PlaySound("QuestOpen", Camera.main.transform.position);
            else if (_number == 5)
                AudioManager.Instance.PlaySound("SettingOpen", Camera.main.transform.position);
        }
        else
        {
            if (_number == 0)
                AudioManager.Instance.PlaySound("EquipClose", Camera.main.transform.position);
            else if (_number == 1)
                AudioManager.Instance.PlaySound("InventoryClose", Camera.main.transform.position);
            else if (_number == 2)
                AudioManager.Instance.PlaySound("SkillClose", Camera.main.transform.position);
            else if (_number == 3)
                AudioManager.Instance.PlaySound("QuestClose", Camera.main.transform.position);
            else if (_number == 5)
                AudioManager.Instance.PlaySound("SettingClose", Camera.main.transform.position);
        }
    }

    public void WindowUpdate(int _Num,bool _Active = true)
    {
        if (_Num == 5)
            return;

        buttonController.transform.GetChild(_Num).GetChild(0).gameObject.SetActive(_Active);

        if((_Num == 2) || (_Num == 3))
        {
            if (_Active)
                return;

            if(_Num == 2)
                GameController.Instance.NoticeSet_Off(1);
            else
                GameController.Instance.NoticeSet_Off(0);
        }
    }

    public void AllActiveFalseWindow(bool isKey = false)
    {
        int count = 0;

        for (int i = 0; i < window.Length; i++)
        {
            if (window[i] == null)
                return;

            if (window[i].activeSelf || npcUI.transform.GetChild(0).gameObject.activeSelf)
                count++;

            window[i].SetActive(false);

            for (int child = 0; child < pop_up_Inven.transform.parent.transform.childCount; child++)
            {
                pop_up_Inven.transform.parent.transform.GetChild(child).gameObject.SetActive(false);
            }

        }

        
        if (count == 0 && isKey)
        {
            AudioManager.Instance.PlaySound("SettingOpen", Camera.main.transform.position);
            window[5].SetActive(true);
        }
    }

    public void OutNPC()
    {
        npcUI.transform.GetChild(0).gameObject.SetActive(false);
        if (window[4].activeSelf)
            npcUI.OffStore();
        else
            npcUI.OffNpcUI();

        window[4].SetActive(false);
    }

    public void PopUp_Inven_Active(string _name, Item.ItemClass type, GameObject item)
    {
        AudioManager.Instance.PlaySound("ItemPopup", Camera.main.transform.position);
        pop_up_Inven.SetActive(true);
        Text itemName = pop_up_Inven.transform.GetChild(0).GetComponent<Text>();

        switch (type)
        {
            case Item.ItemClass.Normal:
                itemName.text = "<color=#878787>" + _name + "</color>";
                break;
            case Item.ItemClass.Common:
                itemName.text = "<color=#9FFF4F>" + _name + "</color>";
                break;
            case Item.ItemClass.Rare:
                itemName.text = "<color=#FFFD4E>" + _name + "</color>";
                break;
            case Item.ItemClass.Unique:
                itemName.text = "<color=#D970FF>" + _name + "</color>";
                break;
            case Item.ItemClass.Legend:
                itemName.text = "<color=#FF5437>" + _name + "</color>";
                break;
            default:
                break;
        }
        currentItem = item;
    }

    public void PopUp_Equip_Active(GameObject item)
    {
        AudioManager.Instance.PlaySound("ItemPopup", Camera.main.transform.position);
        pop_up_Equip.SetActive(true);
        currentItem = item;
    }

    public void PopUp_Inven_Consum_Active(string _name, Item.ItemClass type, GameObject item)
    {
        AudioManager.Instance.PlaySound("ItemPopup", Camera.main.transform.position);
        pop_up_Inven_Consum.SetActive(true);
        Text itemName = pop_up_Inven_Consum.transform.GetChild(0).GetComponent<Text>();
        Slider itemSlider = pop_up_Inven_Consum.GetComponentInChildren<Slider>();

        switch (type)
        {
            case Item.ItemClass.Normal:
                itemName.text = "<color=#878787>" + _name + "</color>";
                break;
            case Item.ItemClass.Common:
                itemName.text = "<color=#9FFF4F>" + _name + "</color>";
                break;
            case Item.ItemClass.Rare:
                itemName.text = "<color=#FFFD4E>" + _name + "</color>";
                break;
            case Item.ItemClass.Unique:
                itemName.text = "<color=#D970FF>" + _name + "</color>";
                break;
            case Item.ItemClass.Legend:
                itemName.text = "<color=#FF5437>" + _name + "</color>";
                break;
            default:
                break;
        }

        itemSlider.maxValue = item.GetComponent<Item>().ItemCount;
        Text maxText = itemSlider.transform.Find("MaxText").GetComponent<Text>();
        maxText.text = itemSlider.maxValue.ToString();
        currentItem = item;
    }

    public void PopUp_Inven_Consum_Count()
    {
        AudioManager.Instance.PlaySound("ItemCount", Camera.main.transform.position);
        Slider itemSlider = pop_up_Inven_Consum.GetComponentInChildren<Slider>();
        Text currentText = itemSlider.handleRect.transform.Find("CurrentText").GetComponent<Text>();
        currentText.text = itemSlider.value.ToString();
        throwCount = (int)itemSlider.value;
    }

    public void PopUp_Button_Ok()
    {
        AudioManager.Instance.PlaySound("ItemThrow", Camera.main.transform.position);
        pop_up_Inven.SetActive(false);
        Inventory inventory = currentItem.GetComponent<Item>().inventoryWindow.GetComponentInChildren<Inventory>(true);
        Item throwItem = currentItem.GetComponent<Item>();
        throwItem.itemInpos.itemCount = 0;
        Destroy(currentItem);
        StartCoroutine(InventoryReset(inventory));
    }

    public void PopUp_Button_Consum_Ok()
    {
        AudioManager.Instance.PlaySound("ItemThrow", Camera.main.transform.position);
        pop_up_Inven_Consum.SetActive(false);
        Inventory inventory = currentItem.GetComponent<Item>().inventoryWindow.GetComponentInChildren<Inventory>(true);
        if (throwCount == currentItem.GetComponent<Item>().ItemCount)
        {
            StartCoroutine(InventoryReset(inventory));
            Item throwItem = currentItem.GetComponent<Item>();
            throwItem.itemInpos.itemCount = 0;
            Destroy(currentItem);
        }
        else
        {
            int leftover = currentItem.GetComponent<Item>().ItemCount - throwCount;
            currentItem.GetComponent<Item>().ItemCount = leftover;
            currentItem.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            StartCoroutine(InventoryReset(inventory));
        }
    }
    public void PopUp_Sell_Active(string _name, Item.ItemClass type, GameObject item)
    {
        playerStat = FindObjectOfType<PlayerStat>();
        AudioManager.Instance.PlaySound("ItemPopup", Camera.main.transform.position);
        if (item.GetComponent<Item>().ItemCount > 1)
        {
            pop_up_SellGroup.SetActive(true);
            Text itemName = pop_up_SellGroup.transform.GetChild(0).GetComponent<Text>();
            Text sellPrice = pop_up_SellGroup.transform.Find("Price").GetComponentInChildren<Text>();
            Slider itemSlider = pop_up_SellGroup.GetComponentInChildren<Slider>();
            itemName.text = Utill.Instance.SetItemName(type, _name);
            itemSlider.value = 1;
            itemSlider.maxValue = item.GetComponent<Item>().ItemCount;
            sellPrice.text = (item.GetComponent<Item>().itemInpos.price * 0.25f).ToString("0");
            Text maxText = itemSlider.transform.Find("MaxText").GetComponent<Text>();
            maxText.text = itemSlider.maxValue.ToString();
        }
        else
        {
            pop_up_Sell.SetActive(true);
            Text itemName = pop_up_Sell.transform.GetChild(0).GetComponent<Text>();
            Text sellPrice = pop_up_Sell.transform.Find("Price").GetComponentInChildren<Text>();
            itemName.text = Utill.Instance.SetItemName(type, _name);
            sellPrice.text = (item.GetComponent<Item>().itemInpos.price * 0.25f).ToString("0");
        }
        currentItem = item;
    }

    public void PopUp_Sell_Ok()
    {
        AudioManager.Instance.PlaySound("ItemSell", Camera.main.transform.position);
        pop_up_Sell.SetActive(false);
        Inventory inventory = currentItem.GetComponent<Item>().inventoryWindow.GetComponentInChildren<Inventory>(true);

        PlayerHealth pH = playerStat.gameObject.GetComponent<PlayerHealth>();
        pH.GiveCoin((int)(currentItem.GetComponent<Item>().itemInpos.price * 0.25f));
        Item throwItem = currentItem.GetComponent<Item>();
        throwItem.itemInpos.itemCount = 0;
        Destroy(currentItem);
        StartCoroutine(InventoryReset(inventory));
    }
    public void PopUp_SellGroup_Count()
    {
        AudioManager.Instance.PlaySound("ItemCount", Camera.main.transform.position);
        Slider itemSlider = pop_up_SellGroup.GetComponentInChildren<Slider>();
        Text sellPrice = pop_up_SellGroup.transform.Find("Price").GetComponentInChildren<Text>();
        Text currentText = itemSlider.handleRect.transform.Find("CurrentText").GetComponent<Text>();
        sellCount = (int)itemSlider.value;
        currentText.text = itemSlider.value.ToString();
        if (currentItem != null)
            sellPrice.text = ((currentItem.GetComponent<Item>().itemInpos.price * 0.25f) * sellCount).ToString("0");
    }

    public void PopUp_SellGroup_Ok()
    {
        AudioManager.Instance.PlaySound("ItemGroupSell", Camera.main.transform.position);
        pop_up_SellGroup.SetActive(false);
        Inventory inventory = currentItem.GetComponent<Item>().inventoryWindow.GetComponentInChildren<Inventory>(true);
        PlayerHealth pH = playerStat.gameObject.GetComponent<PlayerHealth>();
        if (sellCount == currentItem.GetComponent<Item>().ItemCount)
        {
            Item sellItem = currentItem.GetComponent<Item>();
            sellItem.itemInpos.itemCount = 0;
            pH.GiveCoin((int)(currentItem.GetComponent<Item>().itemInpos.price * 0.25f) * sellCount);
            Destroy(currentItem);
            StartCoroutine(InventoryReset(inventory));
        }
        else
        {
            int leftover = currentItem.GetComponent<Item>().ItemCount - sellCount;
            pH.GiveCoin((int)(currentItem.GetComponent<Item>().itemInpos.price * 0.25f) * sellCount);
            currentItem.GetComponent<Item>().ItemCount = leftover;
            currentItem.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            StartCoroutine(InventoryReset(inventory));
        }

    }

    public void PopUp_Purchase_Active(string _name, Item.ItemClass type, GameObject slotType)
    {
        AudioManager.Instance.PlaySound("ItemPopup", Camera.main.transform.position);
        playerStat = FindObjectOfType<PlayerStat>();

        if (slotType.GetComponent<SlotType>().itemSlot == SlotType.ItemSlot.Con ||
            slotType.GetComponent<SlotType>().itemSlot == SlotType.ItemSlot.OtherConsum)
        {
            pop_up_PurchaseGroup.SetActive(true);
            Text itemName = pop_up_PurchaseGroup.transform.GetChild(0).GetComponent<Text>();
            Text purPrice = pop_up_PurchaseGroup.transform.Find("Price").GetComponentInChildren<Text>();
            Slider itemSlider = pop_up_PurchaseGroup.GetComponentInChildren<Slider>();
            itemName.text = Utill.Instance.SetItemName(type, _name);
            itemSlider.minValue = 1;
            itemSlider.value = 1;
            itemSlider.maxValue = 100;
            purPrice.text = slotType.GetComponent<SlotType>().itemInpos.price.ToString();
            Text maxText = itemSlider.transform.Find("MaxText").GetComponent<Text>();
            maxText.text = itemSlider.maxValue.ToString();
        }
        else
        {
            pop_up_Purchase.SetActive(true);
            Text itemName = pop_up_Purchase.transform.GetChild(0).GetComponent<Text>();
            Text purPrice = pop_up_Purchase.transform.Find("Price").GetComponentInChildren<Text>();
            itemName.text = Utill.Instance.SetItemName(type, _name);
            purPrice.text = (slotType.GetComponent<SlotType>().itemInpos.price).ToString("0");
        }
        storeItem = slotType;
    }
    public void PopUp_Purchase_Ok()
    {
        AudioManager.Instance.PlaySound("ItemPurchase", Camera.main.transform.position);
        pop_up_Purchase.SetActive(false);
        playerStat.CoinGet -= storeItem.GetComponent<SlotType>().itemInpos.price;
        storeItem.GetComponent<SlotType>().PurchaseItem();
        storeItem = null;
        StartCoroutine(InventoryReset(Inventory.Instance));
    }
    public void PopUp_PurchaseGroup_Count()
    {
        AudioManager.Instance.PlaySound("ItemCount", Camera.main.transform.position);
        Slider itemSlider = pop_up_PurchaseGroup.GetComponentInChildren<Slider>();
        Text purPrice = pop_up_PurchaseGroup.transform.Find("Price").GetComponentInChildren<Text>();
        Text currentText = itemSlider.handleRect.transform.Find("CurrentText").GetComponent<Text>();
        int maxSlider = 1;
        if (storeItem != null)
            maxSlider = playerStat.CoinGet / storeItem.GetComponent<SlotType>().itemInpos.price;
        purCount = (int)itemSlider.value;

        if (purCount >= maxSlider)
            itemSlider.value = maxSlider;

        currentText.text = itemSlider.value.ToString();

        if (storeItem != null)
        {
            purPrice.text = (storeItem.GetComponent<SlotType>().itemInpos.price * purCount).ToString();
        }

    }
    public void PopUp_PurchaseGroup_Ok()
    {
        if (purCount == 0)
            return;

        AudioManager.Instance.PlaySound("ItemGroupPurchase", Camera.main.transform.position);
        pop_up_PurchaseGroup.SetActive(false);
        playerStat.CoinGet -= storeItem.GetComponent<SlotType>().itemInpos.price * purCount;
        storeItem.GetComponent<SlotType>().itemInpos.itemCount = purCount;
        storeItem.GetComponent<SlotType>().PurchaseItem();
        storeItem = null;
        StartCoroutine(InventoryReset(Inventory.Instance));
    }
    public IEnumerator InventoryReset(Inventory inventory)
    {
        yield return new WaitForSeconds(0.01f);
        inventory.ItemArrSort();
    }

    public void PopUpDeActive(GameObject pop_Up_Type)
    {
        if (npcUI.inactiveGroup.alpha == 1)
        {
            AudioManager.Instance.PlaySound("NpcClose", Camera.main.transform.position);
            pop_Up_Type.SetActive(false);
            if (currentItem != null)
                currentItem.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            npcUI.OffNpcUI();
        }
    }
    
    public void QuestArrReset(QuestList _questList,bool isActive = true)
    {
        questWindow.SetPlayerQuest(_questList, isActive);
    }

    public void QuestPlus(QuestList questList,int count,bool isInventory = false)
    {
        if (!questList.isMax)
            questWindow.PlusCurrentCount(questList, count, isInventory);
        else
            questWindow.CountCheck(questList, count, isInventory);
    }

    public void QuestPlusGroup(QuestList questList, int count, int seat, bool isInventory = false)
    {
        questWindow.PlusCurrentCountGroup(questList, count, seat, isInventory);
    }

    public void QuestUIReset(int _questNumber)
    {
        questWindow.SetPlayerQuestUI(_questNumber);
    }

    public void SaveCanvas()
    {
        saveAnima.gameObject.SetActive(true);
        saveAnima.Play("Save");
    }

    public void EndCanvas()
    {
        saveAnima.Play("End");
    }

    public void CancelSound()
    {
        AudioManager.Instance.PlaySound("Cancel", Camera.main.transform.position);
    }

    public IEnumerator SetMemory()
    {
        float alpha = 0;
        bool isActive = true;
        playerMemory.gameObject.SetActive(isActive);
        playerMemory.blocksRaycasts = isActive;
        playerMemory.interactable = isActive;
        while(true)
        {
            

            if (isActive)
                alpha += 0.04f;
            else
            {
                alpha -= 0.01f;
                if (!isActive && alpha <= 0)
                    break;
            }

            playerMemory.alpha = alpha;

            if (playerMemory.alpha >= 1)
            {
                yield return new WaitForSeconds(0.5f);
                isActive = false;
            }
            yield return null;
        }
        playerMemory.blocksRaycasts = isActive;
        playerMemory.interactable = isActive;
        playerMemory.gameObject.SetActive(isActive);
    }
}
