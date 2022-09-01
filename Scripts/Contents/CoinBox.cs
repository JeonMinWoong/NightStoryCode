using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBox : MonoBehaviour
{
    public int coinCount = 1;
    public int coindrop;
    public Animator animator;

    private void Start()
    {
        coindrop = Random.Range(100, 201);
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (coinCount == 1)
        {
            if (collision.tag == "Player")
            {
                PlayerHealth ph = collision.GetComponent<PlayerHealth>();
                ph.GiveCoinEff();
                animator.SetTrigger("Open");
                AudioManager.Instance.PlaySound("CoinDrop", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                ph.GiveCoin(coindrop);
                coinCount--;
            }
        }
        else
            return;
    }


}
