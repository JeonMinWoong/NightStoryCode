using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemBox : MonoBehaviour
{
    bool isTile;
    bool isGet;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("Player") && !isGet)
        {
            isGet = true;

            Item newItem = Instantiate(Resources.Load<Item>("Prefabs/UI/ItemTest"));

            Item myItem = GetComponent<Item>();
            newItem.itemInpos = GetComponent<Item>().itemInpos;
            newItem.name = GetComponent<Item>().itemInpos.itemName;

            string spritePath = "";

            if(myItem.itemInpos.spritePath <= 100)
            {
                spritePath = "Sprite/Item/Consumption/" + myItem.itemInpos.spritePath;
            }
            else if(myItem.itemInpos.spritePath > 100 && myItem.itemInpos.spritePath <= 1000)
            {
                spritePath = "Sprite/Item/Etc/" + myItem.itemInpos.spritePath;
            }
            else
            {
                if(myItem.itemInpos.spritePath > 1000 && myItem.itemInpos.spritePath <= 1100)
                {
                    spritePath = "Sprite/Item/Equipment/00.Weapon/" + myItem.itemInpos.spritePath;
                }
                else if(myItem.itemInpos.spritePath > 1100 && myItem.itemInpos.spritePath <= 1200)
                {
                    spritePath = "Sprite/Item/Equipment/01.Helmet/" + myItem.itemInpos.spritePath;
                }
                else if (myItem.itemInpos.spritePath > 1200 && myItem.itemInpos.spritePath <= 1300)
                {
                    spritePath = "Sprite/Item/Equipment/02.Chest/" + myItem.itemInpos.spritePath;
                }
                else if (myItem.itemInpos.spritePath > 1300 && myItem.itemInpos.spritePath <= 1400)
                {
                    spritePath = "Sprite/Item/Equipment/03.Pants/" + myItem.itemInpos.spritePath;
                }
                else if (myItem.itemInpos.spritePath > 1400 && myItem.itemInpos.spritePath <= 1500)
                {
                    spritePath = "Sprite/Item/Equipment/04.Shoes/" + myItem.itemInpos.spritePath;
                }
                else if (myItem.itemInpos.spritePath > 1500 && myItem.itemInpos.spritePath <= 1600)
                {
                    spritePath = "Sprite/Item/Equipment/05.Gloves/" + myItem.itemInpos.spritePath;
                }
                else if (myItem.itemInpos.spritePath > 1600 && myItem.itemInpos.spritePath <= 1700)
                {
                    spritePath = "Sprite/Item/Equipment/06.Necklace/" + myItem.itemInpos.spritePath;
                }
                else if (myItem.itemInpos.spritePath > 1700 && myItem.itemInpos.spritePath <= 1800)
                {
                    spritePath = "Sprite/Item/Equipment/07.Ring/" + myItem.itemInpos.spritePath;
                }
            }

            newItem.GetComponent<Image>().sprite = Resources.Load<Sprite>(spritePath);

            Debug.Log("획득");


            GameObject itemGetCanvas = null;
            TextMeshProUGUI getText = null;
            if (UIManager.Instance.itemGetTextUI.transform.childCount == 0)
                itemGetCanvas = Instantiate(UIManager.Instance.itemGetCanvas, collision.transform.position, Quaternion.identity, UIManager.Instance.itemGetTextUI.transform);
            else
            {
                itemGetCanvas = UIManager.Instance.itemGetTextUI.transform.GetChild(0).gameObject;
                itemGetCanvas.transform.SetParent(collision.transform.parent);
                itemGetCanvas.transform.position = collision.transform.position;
                itemGetCanvas.SetActive(true);
            }
            itemGetCanvas.GetComponent<Canvas>().sortingLayerName = "Tile";

            getText = itemGetCanvas.GetComponentInChildren<TextMeshProUGUI>();

            if (myItem.itemInpos.type == Item.ItemType.Equipment)
                getText.text = myItem.itemInpos.itemName.ToString();
            else
                getText.text = myItem.itemInpos.itemName.ToString() + " x " + myItem.itemInpos.itemCount;


            if (GetComponent<Item>().itemInpos.type == Item.ItemType.Equipment)
                Inventory.Instance.ItemAdd(newItem);
            else
                Inventory.Instance.ItemBundleAdd(newItem, newItem.ItemCount);

            AudioManager.Instance.PlaySound("Get_Item", transform.position);

            Destroy(gameObject);
        }

        if (collision.tag.Contains("Tile") && !isTile)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            GetComponent<Rigidbody2D>().isKinematic = true;
            isTile = true;
        }
    }
}
