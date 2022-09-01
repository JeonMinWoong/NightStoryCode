using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public GameObject noticeUI;
    public Animator[] noticeAni;


    private void Awake()
    {
        noticeAni = new Animator[noticeUI.transform.childCount];

        for (int i = 0; i < noticeUI.transform.childCount; i++)
        {
            noticeAni[i] = noticeUI.transform.GetChild(i).GetComponent<Animator>();
        }
    }

}
