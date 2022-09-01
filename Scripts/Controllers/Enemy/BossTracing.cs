using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTracing : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.parent.GetComponent<EnemyBossController>().OnTriggerEnter2D(collision);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        transform.parent.GetComponent<EnemyBossController>().OnTriggerExit2D(collision);
    }

}