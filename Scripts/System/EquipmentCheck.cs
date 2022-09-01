using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentCheck : MonoBehaviour
{
    [SerializeField]
    Enums.EquipType equipType;

    public PlayerStat playerStat;

    public StatTextGroup statTextGroup;

    [SerializeField]
    string[] wearSetGroup = new string[4];

    public void OnOffEquip(bool isOn = true)
    {
        Item item = GetComponentInChildren<Item>();

        if (item.itemInpos.wear.Contains(","))
        {
            wearSetGroup = item.itemInpos.wear.Split(',');
            for (int i = 0; i < wearSetGroup.Length; i++)
            {
                EquipCountCheck(wearSetGroup[i], isOn);
            }
        }
        else
        {
            EquipCountCheck(item.itemInpos.wear, isOn);
        }
    }

    void EquipCountCheck(string wearSet, bool equip)
    {
        string wearValue = "";

        if (equip)
        {
            Item item = GetComponentInChildren<Item>();
            statTextGroup.equipItem[(int)item.itemInpos.kinds - 1] = item;
        }
        else
        {
            Item item = GetComponentInChildren<Item>();
            statTextGroup.equipItem[(int)item.itemInpos.kinds - 1] = null;
        }

        if (wearSet.Contains("공격력"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (equip)
                ItemValueSet(0, wearValue);
            else
                ItemValueSet(0, wearValue, false);

            playerStat.skillList.ResetSkill();
        }
        if (wearSet.Contains("방어력"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (equip)
                ItemValueSet(1, wearValue);
            else
                ItemValueSet(1, wearValue, false);
        }
        if (wearSet.Contains("공격속도"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (equip)
                ItemValueSet(2, wearValue);
            else
                ItemValueSet(2, wearValue, false);
        }
        if (wearSet.Contains("체력"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (equip)
                ItemValueSet(3, wearValue);
            else
                ItemValueSet(3, wearValue, false);
        }
        if (wearSet.Contains("기력"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (equip)
                ItemValueSet(4, wearValue);
            else
                ItemValueSet(4, wearValue, false);
        }
        if (wearSet.Contains("체력 재생"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (equip)
                ItemValueSet(5, wearValue);
            else
                ItemValueSet(5, wearValue, false);
        }
        if (wearSet.Contains("기력 재생"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (equip)
                ItemValueSet(6, wearValue);
            else
                ItemValueSet(6, wearValue, false);
        }
        if (wearSet.Contains("이동속도"))
        {
            wearValue = Utill.Instance.StringCheck(wearSet);
            if (equip)
                ItemValueSet(7, wearValue);
            else
                ItemValueSet(7, wearValue, false);
        }
    }

    void ItemValueSet(int value,string _wearValue, bool active = true)
    {
        int.TryParse(_wearValue, out int itemValue);

        switch (value)
        {
            case 0:
                if(active)
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
            default:
                break;
        }
        
    }
}
