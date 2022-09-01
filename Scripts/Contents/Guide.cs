using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : MonoBehaviour
{
    GameObject buttonGuide;

    private void Start()
    {
        buttonGuide = transform.GetChild(0).gameObject;
    }

    void ButtonGuide(bool isActive)
    {
        buttonGuide.SetActive(isActive);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("Player"))
        {
            if (gameObject.GetComponent<StoryLog>() != null)
            {
                StoryProgress storyProgress = GetComponent<StoryLog>().storyProgress;

                if (storyProgress.progressCount < storyProgress.maxProgressCount || storyProgress.questProgressCount < storyProgress.maxQuestProgressCount
                    || GetComponent<Store>() != null)
                    ButtonGuide(true);
            }
            else
            {
                ButtonGuide(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Contains("Player"))
        {
            ButtonGuide(false);
        }
    }
}
