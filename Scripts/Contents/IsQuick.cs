using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IsQuick : MonoBehaviour
{
    private void Start()
    {
        Item itemBody = transform.parent.GetComponent<Item>();
        Text number = GetComponentInChildren<Text>();
        if(itemBody.itemInpos.kinds == Item.ItemKinds.HPPortion)
        {
            number.text = "1";
        }
        else if(itemBody.itemInpos.kinds == Item.ItemKinds.OtherConsum)
        {
            number.text = "2";
        }
    }
}
