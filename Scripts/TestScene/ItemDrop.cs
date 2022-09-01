using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public void GoblinDestroy()
    {
        GameObject go = GameObject.Find("Goblin");
        GameObject newGo = new GameObject();
        newGo.tag = "Player";

        go.GetComponent<EnemyHealth>().TakeDamage(newGo, 1000);
    }

    public void GoblinCreate()
    {
        GameObject go = Resources.Load<GameObject>("Prefabs/Monster/Goblin");

        GameObject point = GameObject.Find("OJ");


        go = Instantiate(go, go.transform.position, Quaternion.identity, point.transform);
        go.name = "Goblin";
        go.transform.position = new Vector3(point.transform.position.x, point.transform.position.y);
    }
}
