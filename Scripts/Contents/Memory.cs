using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memory : MonoBehaviour
{
    public float maxY;
    public float minY;
    public float moveY;

    bool isUp;
    private void Update()
    {
        if(transform.localPosition.y >= maxY)
            isUp = false;
        else if(transform.localPosition.y <= minY)
            isUp = true;

        if(isUp)
            transform.Translate(0, moveY * Time.deltaTime, 0);
        else
            transform.Translate(0, -moveY * Time.deltaTime, 0);
    }
}
