using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageCanvasUp : MonoBehaviour
{
    Vector3 upPos;
    TextMeshProUGUI text;
    Coroutine coFade;
    Coroutine coSize;
    public bool isCritical;
    public bool isItem;

    private void OnEnable()
    {
        upPos = new Vector3(transform.localPosition.x, transform.localPosition.y + 2.25f, transform.localPosition.z);
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        if (isCritical == true)
        {
            coSize = StartCoroutine(FontSize());
            isCritical = false;
        }
        gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, upPos, 0.015f);

        if(transform.localPosition.y < upPos.y)
        {
            if (coFade == null && isItem == false)
                coFade = StartCoroutine(FadeOut());
            else if (coFade == null && isItem == true)
                coFade = StartCoroutine(ItemFadeOut());
        }
    }

    IEnumerator FontSize()
    {
        for (int i = 0; i < 20; i++)
        {
            text.fontSize += 2;
            yield return new WaitForSeconds(0.01f);
        }
        coSize = null;
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 21; i++)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - 0.05f);
            yield return new WaitForSeconds(0.03f);
        }
        gameObject.SetActive(false);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        text.text = "";
        gameObject.transform.SetParent(UIManager.Instance.transform.GetChild(0).transform);
        coFade = null;
    }

    IEnumerator ItemFadeOut()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 21; i++)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - 0.05f);
            yield return new WaitForSeconds(0.03f);
        }
        gameObject.SetActive(false);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        text.text = "";
        gameObject.transform.SetParent(UIManager.Instance.transform.GetChild(1).transform);
        coFade = null;
    }

}
