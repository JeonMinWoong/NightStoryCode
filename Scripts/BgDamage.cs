using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgDamage : MonoBehaviour
{
    public int bgDamage;
    public float damageDelay = 1f;
    private float nextAttackTime;

    private void Start()
    {
        bgDamage = 15;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((Time.time >= nextAttackTime))
        {
            if (collision.tag == "Player")
            {

                AudioManager.Instance.PlaySound("Bgdamage", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(gameObject,bgDamage,isPersent:true);
                nextAttackTime = Time.time + damageDelay;
            }
            else
                return;
        }
    }
}

