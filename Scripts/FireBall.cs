using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    private const float moveSpeed = 10f;
    public bool x;
    public GameObject fireballEx;

    void Start()
    {

    }
    void Update()
    {
        if (x == true)
        {
            float moveX = moveSpeed * Time.deltaTime;
            transform.Translate(moveX, 0, 0);
        }
        else
        {
            float moveX = moveSpeed * Time.deltaTime;
            transform.Translate(-moveX, 0, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" || collision.tag == "Tile" )
        {
            if(collision.tag == "Player")
            {
                collision.GetComponent<PlayerHealth>().TakeDamage(gameObject,50);
                Instantiate(fireballEx, this.transform.position, Quaternion.identity);
            }
            Destroy(this.gameObject);
        }
    }
    void OnBecameInvisible()
    {
        Destroy(this.gameObject);// 자기 자신을 지웁니다.
    }


}