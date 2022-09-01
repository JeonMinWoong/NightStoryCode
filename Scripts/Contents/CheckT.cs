using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckT : MonoBehaviour
{

    void OnTuto(bool isActive)
    {
        gameObject.GetComponent<Canvas>().enabled = isActive;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Contains("Player"))
        {
            OnTuto(true);
        }
    }
}
