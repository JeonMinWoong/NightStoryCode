using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bosszon : MonoBehaviour
{
    public bool bossZon;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if(!bossZon)
                GameController.Instance.Boss();
            bossZon = true;
            
        }
    }
}
