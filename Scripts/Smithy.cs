using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smithy : Singleton<Smithy>

{
    public int smithyCount = 1;
    public int smithy;

    private void Start()
    {
        smithy = Random.Range(10, 21);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (smithyCount == 1)
        {
            if (collision.tag == "Player")
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                AudioManager.Instance.PlaySound("SmithySound", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                collision.GetComponent<PlayerHealth>().StartCoroutine("SmithyAttack");
                smithyCount--;
            }
        }
        else
            return;
    }


}
