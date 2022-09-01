using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreController : MonoBehaviour
{
    GameObject storeGroup;

    private void Awake()
    {
        storeGroup = transform.Find("StoreGroup").gameObject;
        gameObject.SetActive(false);
    }

    public void SetStoreType(string npcName)
    {
        for (int i = 0; i < storeGroup.transform.childCount; i++)
        {
            storeGroup.transform.GetChild(i).gameObject.SetActive(false);
        }

        StoreSlot storeSlot = null;

        switch (npcName)
        {
            case "N_Blacksmith":
                storeGroup.transform.Find(npcName).gameObject.SetActive(true);
                storeSlot = storeGroup.transform.Find(npcName).GetComponent<StoreSlot>();
                storeSlot.StoreSet();
                storeSlot.storeType = Enums.StoreType.Blacksmith;
                break;
            case "N_Trinkets_vendor":
                storeGroup.transform.Find(npcName).gameObject.SetActive(true);
                storeSlot = storeGroup.transform.Find(npcName).GetComponent<StoreSlot>();
                storeSlot.StoreSet();
                storeSlot.storeType = Enums.StoreType.Trinkets;
                break;
            case "N_Traveling_Merchant":
                storeGroup.transform.Find(npcName).gameObject.SetActive(true);
                storeSlot = storeGroup.transform.Find(npcName).GetComponent<StoreSlot>();
                storeSlot.StoreSet();
                storeSlot.storeType = Enums.StoreType.Traveling;
                break;
            default:
                break;
        }
            
    }

    public void StoreExit()
    {
        UIManager.Instance.ActiveWindow(4);
        UIManager.Instance.npcUI.OffStore();
    }
}
