using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillLevelButton : MonoBehaviour
{
    [SerializeField]
    Sprite[] buttonImage = new Sprite[2];
    Image currentImage;

    private void Awake()
    {
        currentImage = GetComponent<Image>();
    }

    public void ButtonActive()
    {
        currentImage.sprite = buttonImage[0];
    }

    public void ButtonDeActive()
    {
        currentImage.sprite = buttonImage[1];
    }
}
