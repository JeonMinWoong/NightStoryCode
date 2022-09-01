using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Contains("Player"))
        {
            collision.GetComponent<PlayerStat>().QuestCheck(101);
        }
    }
}
