using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectUI : MonoBehaviour
{
    public GameObject selectUI;
    public Button[] selectButton;

    public int currentCount;

    private void Start()
    {
        for (int i = 0; i < selectButton.Length; i++)
        {
            selectButton[i].gameObject.SetActive(false);
        }
    }

    private void SelectEnd()
    {
        for (int i = 0; i < selectButton.Length; i++)
        {
            selectButton[i].gameObject.SetActive(false);
        }
        Time.timeScale = 1;
    }

    public void SelectSet(string[] textGroup)
    {
        Time.timeScale = 0;
        selectUI.SetActive(true);
        AudioManager.Instance.PlaySound("SelectWindow", Camera.main.transform.position);
        for (int i = 0; i < textGroup.Length; i++)
        {
            int count = i;
            selectButton[i].gameObject.SetActive(true);
            selectButton[i].onClick.RemoveAllListeners();
            selectButton[i].onClick.AddListener(() => OnSelectButton(count));
            Text selectText = selectButton[i].GetComponentInChildren<Text>();
            selectText.text = (i + 1).ToString() + "." + textGroup[i];
        }
        
    }

    public void OnSelectButton(int count)
    {
        AudioManager.Instance.PlaySound("SelectRoot", Camera.main.transform.position);
        currentCount = count + 1;
        Debug.Log(count);
        selectUI.SetActive(false);
        SelectEnd();
    }
}
