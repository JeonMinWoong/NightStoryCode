using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelect : MonoBehaviour
{
    PlayerStat playerStat;
    [SerializeField]
    Toggle[] toggles;
    [SerializeField]
    GameObject[] lockOJ;
    [SerializeField]
    Button[] goButtons;
    
    private void Start()
    {
        playerStat = FindObjectOfType<PlayerStat>();

        for (int i = 0; i < goButtons.Length; i++)
        {
            if(i != 0)
                goButtons[i].gameObject.SetActive(false);
        }
        MapInit();
    }

    void MapInit()
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            if (playerStat.MapInpo < i)
            {
                toggles[i].enabled = false;
                lockOJ[i].gameObject.SetActive(true);
            }
            else
            {
                toggles[i].enabled = true;
                lockOJ[i].gameObject.SetActive(false);
            }
        }
    }

    public void SelectMapCheck(int count)
    {
        for (int i = 0; i < goButtons.Length; i++)
        {
            if (i == count)
            {
                int mapNum = i;
                goButtons[i].gameObject.SetActive(true);
                goButtons[i].onClick.RemoveAllListeners();
                goButtons[i].onClick?.AddListener(()=>GoMap(mapNum));
            }
            else
            {
                goButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void GoMap(int count)
    {
        int mapNum = count + 1;
        LoadingManager.Instance.LoadScene("Map_" + mapNum);
    }
}
