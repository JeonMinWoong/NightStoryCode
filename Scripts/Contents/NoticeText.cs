using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeText : MonoBehaviour
{
    CanvasGroup canvasGroup;
    Coroutine coNotice;
    [SerializeField]
    Text[] noticeTextGroup = new Text[2];

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Notice(int count,string addText = "")
    {
        if (count == 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            noticeTextGroup[0].text = "'"+ addText +"'" + " 퀘스트가 완료 되었습니다.";
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(true);
            noticeTextGroup[1].text = "새로운 '"+ addText + "'" + "을 장착할 수 있습니다.";
        }
        if (coNotice != null)
            return;

        coNotice = StartCoroutine(CoNotice());
    }

    public void NoticeOff(int count)
    {
        if (count == 0)
            transform.GetChild(0).gameObject.SetActive(false);
        else
            transform.GetChild(1).gameObject.SetActive(false);

        if (coNotice != null)
            coNotice = null;

    }

    IEnumerator CoNotice()
    {
        bool isActive = false;

        while(noticeTextGroup[0].gameObject.activeSelf || noticeTextGroup[1].gameObject.activeSelf)
        {
            //Debug.Log("체크");

            if (canvasGroup.alpha <= 0)
                isActive = true;
            else if (canvasGroup.alpha >= 1)
                isActive = false;

            if (isActive)
                canvasGroup.alpha += 0.02f;
            else
                canvasGroup.alpha -= 0.02f;

            yield return null;
        }

        coNotice = null;
    }
}
