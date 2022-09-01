using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewContentText : MonoBehaviour
{
    [SerializeField]
    ScrollRect scrollRect;
    [SerializeField]
    VerticalLayoutGroup content;
    [SerializeField]
    RectTransform logTextSize;

    public void SetContents()
    {
        scrollRect.content.anchoredPosition = new Vector2(0, 0);
        if (gameObject.activeSelf)
            StartCoroutine(coSetContents());
    }

    private void OnEnable()
    {
        StartCoroutine(coSetContents());
    }

    IEnumerator coSetContents()
    {
        content.enabled = false;
        content.GetComponent<ContentSizeFitter>().enabled = false;
        yield return null;
        
        Canvas.ForceUpdateCanvases();

        logTextSize.GetComponent<ContentSizeFitter>().enabled = true;

        yield return null;

        logTextSize.GetComponent<ContentSizeFitter>().enabled = false;

        yield return null;

        content.enabled = true;
        content.GetComponent<ContentSizeFitter>().enabled = true;
    }

}
