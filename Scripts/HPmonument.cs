using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPmonument : MonoBehaviour
{
    public int HpCount;
    public int recovery;

    private void Start()
    {
        HpCount = 1;
        recovery = 30;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (HpCount == 1)
        {
            if (collision.tag == "Player")
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                AudioManager.Instance.PlaySound("recovery", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                collision.GetComponent<PlayerHealth>().HpRecovery(recovery);
                HpCount--;
            }
        }
        else
            return;
    }


}
