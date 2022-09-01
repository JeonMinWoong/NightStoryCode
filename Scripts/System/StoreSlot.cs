using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreSlot : MonoBehaviour
{
    public PlayerStat playerStat;
    public GameObject itemInpo;
    [SerializeField]
    ScrollRect storeSlot;
    GameObject slots;
    SlotType[] slotGroup;
    SlotType slotType;

    public Enums.StoreType storeType;
    Enums.StoreButton storeButton;

    ScrollViewContent viewContent;
    [SerializeField]
    ToggleGroup toggleGroup;
    private void Awake()
    {
        GameObject playerCanvase = GameObject.Find("PlayerCanvas").gameObject;
        itemInpo = playerCanvase.transform.Find("ItemInpo").gameObject;
        viewContent = GetComponent<ScrollViewContent>();
        playerStat = FindObjectOfType<PlayerStat>();
        //playerStat.CoinGet += 20000;
    }

    private void OnEnable()
    {
        Debug.Log("Check");
        for (int i = 0; i < toggleGroup.transform.childCount; i++)
        {
            if(i == 0)
                toggleGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = true;
            else
                toggleGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
        }
        
    }

    public void StoreSet()
    {
        slots = storeSlot.content.gameObject;
        SlotSet();
    }

    void SlotSet()
    {
        for (int i = 0; i < slots.transform.childCount; i++)
        {
            slotType = slots.transform.GetChild(i).GetComponent<SlotType>();
            slotType.playerStat = playerStat;
            slotType.itemInpo = itemInpo;
            slotType.ItemSlotSet();
        }
        StoreArr(0);
    }

    public void StoreArr(int _number)
    {
        AudioManager.Instance.PlaySound("ItemSlot", Camera.main.transform.position);
        int value = 0;
        storeButton = (Enums.StoreButton)_number;
        slotGroup = slots.GetComponentsInChildren<SlotType>(true);

        if (storeType == Enums.StoreType.Blacksmith)
        {
            switch (storeButton)
            {
                case Enums.StoreButton.All:
                    value = 0;
                    foreach (SlotType slot in slotGroup)
                    {
                        slot.gameObject.transform.gameObject.SetActive(true);
                    }
                    break;
                case Enums.StoreButton.Weapon:
                    value = 1;
                    foreach (SlotType slot in slotGroup)
                    {
                        if ((int)slot.itemSlot == value)
                            slot.gameObject.transform.gameObject.SetActive(true);
                        else
                            slot.gameObject.transform.gameObject.SetActive(false);
                    }
                    break;
                case Enums.StoreButton.Armor:
                    value = 6;
                    foreach (SlotType slot in slotGroup)
                    {
                        if (1 < (int)slot.itemSlot && (int)slot.itemSlot <= value)
                            slot.gameObject.transform.gameObject.SetActive(true);
                        else
                            slot.gameObject.transform.gameObject.SetActive(false);
                    }
                    break;
                default:
                    break;
            }
        }
        else if (storeType == Enums.StoreType.Trinkets)
        {
            switch (storeButton)
            {
                case Enums.StoreButton.All:
                    value = 0;
                    foreach (SlotType slot in slotGroup)
                    {
                        slot.gameObject.transform.gameObject.SetActive(true);
                    }
                    break;
                case Enums.StoreButton.Necklace_Ring:
                    value = 8;
                    foreach (SlotType slot in slotGroup)
                    {
                        if (6 < (int)slot.itemSlot && (int)slot.itemSlot <= value)
                            slot.gameObject.transform.gameObject.SetActive(true);
                        else
                            slot.gameObject.transform.gameObject.SetActive(false);
                    }
                    break;
                case Enums.StoreButton.Con:
                    value = 10;
                    foreach (SlotType slot in slotGroup)
                    {
                        if (8 < (int)slot.itemSlot && (int)slot.itemSlot <= value)
                            slot.gameObject.transform.gameObject.SetActive(true);
                        else
                            slot.gameObject.transform.gameObject.SetActive(false);
                    }
                    break;
                default:
                    break;
            }
        }
        else if (storeType == Enums.StoreType.Traveling)
        {
            switch (storeButton)
            {
                case Enums.StoreButton.All:
                    value = 0;
                    foreach (SlotType slot in slotGroup)
                    {
                        slot.gameObject.transform.gameObject.SetActive(true);
                    }
                    break;
                case Enums.StoreButton.Con:
                    value = 10;
                    foreach (SlotType slot in slotGroup)
                    {
                        if (8 < (int)slot.itemSlot && (int)slot.itemSlot <= value)
                            slot.gameObject.transform.gameObject.SetActive(true);
                        else
                            slot.gameObject.transform.gameObject.SetActive(false);
                    }
                    break;
                case Enums.StoreButton.Etc:
                    value = 0;
                    foreach (SlotType slot in slotGroup)
                    {
                        if ((int)slot.itemSlot == 0)
                            slot.gameObject.transform.gameObject.SetActive(true);
                        else
                            slot.gameObject.transform.gameObject.SetActive(false);
                    }
                    break;
                default:
                    break;
            }
        }
        viewContent.enabled = false;
        viewContent.enabled = true;
        viewContent.SetContentSize();

    }
}
