using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fullmonument : MonoBehaviour
{
    public int fullCount;
    public int recovery;

    private void Start()
    {
        fullCount = 1;
        recovery = 100;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (fullCount == 1)
        {
            if (collision.tag == "Player")
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                AudioManager.Instance.PlaySound("recovery", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                collision.GetComponent<PlayerHealth>().HpRecovery(recovery);
                collision.GetComponent<PlayerHealth>().SpRecovery(recovery);
                fullCount--;
            }
        }
        else
            return;
    }


}
