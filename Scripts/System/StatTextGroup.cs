using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatTextGroup : MonoBehaviour
{
    public GameObject generalGroup;
    public GameObject damageGroup;
    public GameObject armorGroup;

    public PlayerStat playerStat;

    public List<GameObject> equipmentSlot = new List<GameObject>();

    public List<GameObject> portionSlot = new List<GameObject>();

    public List<Item> equipItem = new List<Item>();

    public List<EquipmentCheck> equipmentChecks = new List<EquipmentCheck>();

    public void Awake()
    {
        playerStat = FindObjectOfType<PlayerStat>();
        playerStat.statTextGroup = this;
        equipItem = new List<Item>(new Item[8]);
        for (int i = 0; i < equipmentSlot.Count; i++)
        {
            equipmentSlot[i].transform.GetChild(0).GetComponent<EquipmentCheck>().playerStat = playerStat;
            equipmentSlot[i].transform.GetChild(0).GetComponent<EquipmentCheck>().statTextGroup = this;
            equipmentChecks.Add(equipmentSlot[i].transform.GetChild(0).GetComponent<EquipmentCheck>());
        }
        
        gameObject.SetActive(false);
    }
    public void SaveEquip()
    {
        playerStat.playerAbility.equipItem.Clear();

        playerStat.playerAbility.equipItem = new List<ItemInpo>(new ItemInpo[equipItem.Count]);

        for (int i = 0; i < equipItem.Count; i++)
        {
            if (equipItem[i] != null)
                playerStat.playerAbility.equipItem[i] = equipItem[i].itemInpos;
        }
    }

    public void LoadEquip()
    {
        for (int i = 0; i < playerStat.playerAbility.equipItem.Count; i++)
        {
            if (playerStat.playerAbility.equipItem[i].spritePath <= 0)
                continue;

            Item loadItem = Instantiate(Resources.Load<Item>("Prefabs/UI/ItemTest"));

            loadItem.itemInpos = playerStat.playerAbility.equipItem[i];
            equipItem[i] = loadItem;
            loadItem.name = loadItem.itemInpos.itemName;
            loadItem.transform.SetParent(equipmentSlot[i].transform.GetChild(0));
            loadItem.transform.localPosition = Vector2.zero;
            loadItem.transform.localScale = new Vector2(1, 1);
            loadItem.GetComponent<Image>().sprite = Resources.Load<Sprite>(Utill.Instance.SpritePath(loadItem.itemInpos.spritePath));
            equipmentSlot[i].transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void EqiupLevelUp()
    {
        for (int i = 0; i < equipmentChecks.Count; i++)
        {
            if(equipmentChecks[i].transform.childCount != 0)
            {
                equipmentChecks[i].OnOffEquip();
            }
        }
    }

}
