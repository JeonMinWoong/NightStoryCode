using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracing : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.parent.GetComponent<EnemyController>().OnTriggerEnter2D(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        transform.parent.GetComponent<EnemyController>().OnTriggerStay2D(collision);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(gameObject.activeSelf)
            transform.parent.GetComponent<EnemyController>().OnTriggerExit2D(collision);
    }

}
