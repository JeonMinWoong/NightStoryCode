using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    public GameObject intro;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameController.Instance == null)
            return;

        if (collision.tag == "Player" && GameController.Instance.playIng == false)
        {
            intro.SetActive(true);
        }
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        if (GameController.Instance == null)
            return;

        if (collision.tag == "Player" && GameController.Instance.playIng == false)
        {
            intro.SetActive(false);
        }
    }
}
