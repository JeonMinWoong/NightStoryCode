using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public void OnButton(int count)
    {
        UIManager.Instance.ActiveWindow(count);
        if(count != 5)
            transform.GetChild(count).GetChild(0).gameObject.SetActive(false);
    }
}
