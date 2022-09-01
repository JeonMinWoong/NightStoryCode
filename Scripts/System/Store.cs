using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Store : MonoBehaviour
{
    public StoreType storeType;
    public bool isNoStore;

    private void Start()
    {
        StoreInit();
    }

    void StoreInit()
    {
        int typeCount = System.Enum.GetValues(typeof(StoreType)).Length;
        for (int i = 0; i < typeCount; i++)
        {
            if (name.Contains(((StoreType)i).ToString()))
            {
                storeType = (StoreType)i;
            }
        }
    }
}
