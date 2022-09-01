using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewContent : MonoBehaviour
{
    ScrollRect scrollRect;

    int count;

    private void Awake()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();
    }

    private void OnEnable()
    {
        CheckItem();
    }

    void CheckItem()
    {
        count = 0;

        for (int i = 0; i < scrollRect.content.transform.childCount; i++)
        {
            if (scrollRect.content.transform.GetChild(i).gameObject.activeSelf)
                count++;
        }
    }

    public void SetContentSize()
    {
        int line = Mathf.CeilToInt(count / 2.0f);

        scrollRect.content.sizeDelta = new Vector2(1000, 151 * line);
    }
}
