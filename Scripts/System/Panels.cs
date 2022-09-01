using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panels : MonoBehaviour
{
    Coroutine coInven;
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        coInven = StartCoroutine(InventoryCheck());
    }

    public void ButtonSound(int _number)
    {
        UIManager.Instance.ButtonSound(_number);
    }

    IEnumerator InventoryCheck()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < Inventory.Instance.itemGroupArr.Count; i++)
        {
            Inventory.Instance.QuestItemCheck(Inventory.Instance.itemGroupArr[i]);
        }

        coInven = null;

        if(coInven == null)
            coInven = StartCoroutine(InventoryCheck());
    }
}
