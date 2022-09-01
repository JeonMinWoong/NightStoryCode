using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Utill : Singleton<Utill>
{
    public string SetItemName(Item.ItemClass itemClass,string _name)
    {
        string itemName = "";

        switch (itemClass)
        {
            case Item.ItemClass.Normal:
                itemName = "<color=#878787>" + _name + "</color>";
                break;
            case Item.ItemClass.Common:
                itemName = "<color=#9FFF4F>" + _name + "</color>";
                break;
            case Item.ItemClass.Rare:
                itemName = "<color=#FFFD4E>" + _name + "</color>";
                break;
            case Item.ItemClass.Unique:
                itemName = "<color=#D970FF>" + _name + "</color>";
                break;
            case Item.ItemClass.Legend:
                itemName = "<color=#FF5437>" + _name + "</color>";
                break;
            default:
                break;
        }

        return itemName;
    }

    public string StringCheck(string value)
    {
        string conversion = Regex.Replace(value, @"\D", "");
        return conversion;
    }

    public string SpritePath(int path)
    {
        string spritePath = "";

        if (path <= 100)
        {
            spritePath = "Sprite/Item/Consumption/" + path;
        }
        else if (path > 100 && path <= 1000)
        {
            spritePath = "Sprite/Item/Etc/" + path;
        }
        else
        {
            if (path > 1000 && path <= 1100)
            {
                spritePath = "Sprite/Item/Equipment/00.Weapon/" + path;
            }
            else if (path > 1100 && path <= 1200)
            {
                spritePath = "Sprite/Item/Equipment/01.Helmet/" + path;
            }
            else if (path > 1200 && path <= 1300)
            {
                spritePath = "Sprite/Item/Equipment/02.Chest/" + path;
            }
            else if (path > 1300 && path <= 1400)
            {
                spritePath = "Sprite/Item/Equipment/03.Pants/" + path;
            }
            else if (path > 1400 && path <= 1500)
            {
                spritePath = "Sprite/Item/Equipment/04.Shoes/" + path;
            }
            else if (path > 1500 && path <= 1600)
            {
                spritePath = "Sprite/Item/Equipment/05.Gloves/" + path;
            }
            else if (path > 1600 && path <= 1700)
            {
                spritePath = "Sprite/Item/Equipment/06.Necklace/" + path;
            }
            else if (path > 1700 && path <= 1800)
            {
                spritePath = "Sprite/Item/Equipment/07.Ring/" + path;
            }
        }

        return spritePath;
    }
}
