using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextGroup : MonoBehaviour
{
    public List<Text> textGroup = new List<Text>();

    public Image skill_Icon;

    private void Awake()
    {
        Text[] dummyGroup = GetComponentsInChildren<Text>();
        foreach (Text text in dummyGroup)
        {
            if (text.name.Equals("Text"))
                textGroup.Add(text);
        }
    }
}
