using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelect : MonoBehaviour
{
    [SerializeField]
    GameObject stageUI;


    public void EventActive()
    {
        if (stageUI.activeSelf == false)
            stageUI.SetActive(true);
        else
            stageUI.SetActive(false);
    }
}
