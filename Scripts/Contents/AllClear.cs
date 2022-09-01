using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllClear : MonoBehaviour
{
    public List<GameObject> will = new List<GameObject>();

    private void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player").gameObject;


            GameObject gos = GameObject.Find("WillGroup");
            foreach (Transform go in gos.transform)
            {
                if (go.name.Equals("Will"))
                    will.Add(go.gameObject);
            }


            for (int i = 0; i < will.Count; i++)
            {
                will[i].GetComponent<EnemyHealth>().TakeDamage(player, 100);
            }
        }
    }
}
