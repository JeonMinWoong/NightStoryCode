using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundCam : MonoBehaviour
{
    Transform mainCam;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.transform;
        transform.position = new Vector3(mainCam.position.x, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(mainCam.position.x, transform.position.y, transform.position.z);
    }
}
