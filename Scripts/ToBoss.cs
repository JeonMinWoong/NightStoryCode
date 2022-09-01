using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToBoss : MonoBehaviour
{
    public int currentCount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerController pA = collision.GetComponent<PlayerController>();

            if (pA.coIntro == null)
            {
                GameController.Instance.StartCoroutine(GameController.Instance.Next_Stage(currentCount, "BossZon"));
            }
            
            AudioManager.Instance.PlaySound("Toboss", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        } 
    }

}
