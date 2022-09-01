using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MiniMap
{
    public string stageNum;
    public Vector2 minimapPos;
    public float minimapSize;
    public Rect minimapRect;
}

public class MiniMapSetting : MonoBehaviour
{
    public GameObject[] mapTypeGroup = new GameObject[3];

    [SerializeField]
    MiniMap[] miniMaps;
    [SerializeField]
    Camera minimapCam;
    [SerializeField]
    RawImage minimapImage;
    [SerializeField]
    Text[] mapNameText;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            mapTypeGroup[i] = transform.GetChild(i).gameObject;
        }
    }

    public void MiniMapSet(int count,string stageName)
    {
        minimapCam.transform.localPosition = miniMaps[count].minimapPos;
        minimapCam.orthographicSize = miniMaps[count].minimapSize;
        minimapCam.rect = miniMaps[count].minimapRect;
        minimapImage.uvRect = minimapCam.rect;
        //Debug.Log(minimapCam.rect + " / " + minimapImage.uvRect);

        string setStageName = stageName.Replace("\n"," ");
        for (int i = 0; i < mapNameText.Length; i++)
        {
            mapNameText[i].text = setStageName;  
        } 
    }

    public void MapTypeButton(int count)
    {
        AudioManager.Instance.PlaySound("MiniMap", Camera.main.transform.position);
        if (count == 0)
        {
            mapTypeGroup[0].SetActive(false);
            mapTypeGroup[1].SetActive(false);
            mapTypeGroup[2].SetActive(true);
        }
        else if(count == 1)
        {
            mapTypeGroup[0].SetActive(false);
            mapTypeGroup[1].SetActive(true);
            mapTypeGroup[2].SetActive(false);
        }
        else
        {
            mapTypeGroup[0].SetActive(true);
            mapTypeGroup[1].SetActive(false);
            mapTypeGroup[2].SetActive(false);
        }
    }
}

