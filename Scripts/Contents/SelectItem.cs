using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectItem : MonoBehaviour
{
    public int selectionItem;
    public int selectItemCount;
    public bool isOK;

    [SerializeField]
    QuestItem[] questItems;

    public bool SelectCheck()
    {
        int value = 0;
        for (int i = 0; i < questItems.Length; i++)
        {
            if (questItems[i].isSelectCheck && questItems[i].gameObject.activeSelf)
                value++;
        }

        if (value == selectItemCount)
        {
            isOK = true;
            for (int i = 0; i < questItems.Length; i++)
            {
                if (!questItems[i].isSelectCheck && questItems[i].gameObject.activeSelf)
                {
                    questItems[i].GetComponent<Toggle>().interactable = false;
                }
            }
        }
        else
        {
            isOK = false;
            for (int i = 0; i < questItems.Length; i++)
            {
                if(questItems[i].gameObject.activeSelf)
                    questItems[i].GetComponent<Toggle>().interactable = true;
            }
        }
        return isOK;
    }

    public void IsSelectOnStop()
    {
        int value = 0;
        for (int i = 0; i < questItems.Length; i++)
        {
            if (questItems[i].isSelectCheck)
                value++;
        }

        if (value == selectItemCount)
        {
            for (int i = 0; i < questItems.Length; i++)
            {
                if (questItems[i].isSelectCheck)
                {
                    questItems[i].GetComponent<Toggle>().interactable = true;
                }
                else
                {
                    questItems[i].GetComponent<Toggle>().interactable = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < questItems.Length; i++)
            {
                questItems[i].GetComponent<Toggle>().interactable = true;
            }
        }
    }

    public void SelectReset()
    {
        for (int i = 0; i < questItems.Length; i++)
        {
            questItems[i].isSelectCheck = false;
            questItems[i].GetComponent<Toggle>().isOn = false;
            questItems[i].GetComponent<Toggle>().interactable = true;
        }
    }

    public void SelectValue(bool isActive)
    {
        for (int i = 0; i < questItems.Length; i++)
        {
            questItems[i].GetComponent<Toggle>().interactable = isActive;
        }
    }
}
