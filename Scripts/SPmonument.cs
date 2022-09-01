using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPmonument : MonoBehaviour
{
    public int SpCount;
    public int recovery;

    private void Start()
    {
        SpCount = 1;
        recovery = 50;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (SpCount == 1)
        {
            if (collision.tag == "Player")
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                AudioManager.Instance.PlaySound("recovery", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                collision.GetComponent<PlayerHealth>().SpRecovery(recovery);
                SpCount--;
            }
        }
        else
            return;
    }
}
