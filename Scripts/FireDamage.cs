using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDamage : MonoBehaviour
{
    public int fireDamage;
    public float damageDelay = 1f;
    private float nextAttackTime;

    private void Start()
    {
        fireDamage = 25;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if ((Time.time >= nextAttackTime))
        {
            if (collision.tag == "Player")
            {

                AudioManager.Instance.PlaySound("FlameBlastStart", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(gameObject,fireDamage);
                nextAttackTime = Time.time + damageDelay;
            }
            else
                return;
        }

    }
}

