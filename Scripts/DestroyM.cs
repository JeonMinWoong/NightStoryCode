using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyM : MonoBehaviour
{
    public GameObject[] blrook;

    GameObject player;

    private void OnEnable()
    {
        player = GameObject.Find("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            for (int i = 0; i < 6; i++)
            {
                blrook[i].GetComponent<EnemyHealth>().TakeDamage(player,200);
            }
            
        }
    }

}
