using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDummy : MonoBehaviour
{
    public Item item;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);

        if (collision.name.Equals(item.itemInpos.kinds.ToString()))
        {
            Debug.Log(item.isDragEnd);

            if (item.isDragEnd == false)
                return;

            Debug.Log("ÀåÂø");

            if (item.itemInpos.type == Item.ItemType.Equipment)
                item.Equipment();
            else if (item.itemInpos.type == Item.ItemType.Consumption)
                item.ConsumptionEquip();
            item.InventoryOrganize();
        }
        else if(collision.tag.Equals("Sell"))
        {
            if (item.isDragEnd == false)
                return;
            UIManager.Instance.PopUp_Sell_Active(item.itemInpos.itemName,item.itemInpos.itemClass,item.gameObject);
        }
    }
}
