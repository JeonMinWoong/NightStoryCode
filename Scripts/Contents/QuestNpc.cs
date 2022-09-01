using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNpc : MonoBehaviour
{
    [SerializeField]
    GameObject questNotice;
    public List<QuestList> questList;
    public int questCount;
    public Enums.NpcType npcType;
    public PlayerStat playerStat;
    
    private void Awake()
    {
        questNotice = transform.Find("QuestNotice").gameObject;
        playerStat = FindObjectOfType<PlayerStat>();
    }

    private void OnEnable()
    {
        StartCoroutine(QuestLink());
    }

    IEnumerator QuestLink()
    {
        yield return new WaitUntil(() => playerStat.isLoadData);
        if(questList.Count <= 0)
            GetQuestList();
        PlayerQuestList();
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (questCount >= questList.Count)
            {
                questNotice.SetActive(false);
                return;
            }
            else
                questNotice.SetActive(true);

            if (questList[questCount].possibleLv <= playerStat.Level && !questList[questCount].isAccept)
            {
                questNotice.transform.GetChild(0).gameObject.SetActive(true);
                questNotice.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
                questNotice.transform.GetChild(0).gameObject.SetActive(false);

            if(questList[questCount].isAccept && questList[questCount].isMax && !questList[questCount].isEnd)
            {
                questNotice.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                questNotice.transform.GetChild(1).gameObject.SetActive(false);
            }
        } 
    }

    void GetQuestList()
    {
        foreach (var item in DBManager.Instance.questList)
        {
            if (item.Value.npcName == npcType)
                questList.Add(item.Value);
        }
    }

    public void PlayerQuestList()
    {
        int count = 0;
        for (int i = 0; i < playerStat.playerAbility.playerQuests.Count; i++)
        {
            if (playerStat.playerAbility.playerQuests[i].npcName == npcType)
            {
                if (questCount >= questList.Count)
                    return;
                if (playerStat.playerAbility.playerQuests[i].questNubmer == questList[questCount].questNubmer)
                {
                    count++;
                    questList[questCount].currentCount = playerStat.playerAbility.playerQuests[i].currentCount;
                    questList[questCount].isAccept = true;
                    questList[questCount].isMax = playerStat.playerAbility.playerQuests[i].isMax;
                    if (playerStat.playerAbility.playerQuests[i].isEnd)
                    {
                        questList[questCount].isEnd = true;
                        questCount++;
                    }
                }
            }
        }

        if (count == 0)
        {
            questList[questCount].isAccept = false;
            questList[questCount].isMax = false;
        }
    }

    public void NpcQuestGiveUp()
    {
        PlayerQuestList();
    }

    public void QusetClear()
    {
        if (questList[questCount].isEnd)
            questCount++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Contains("Player"))
        {
            playerStat = collision.GetComponent<PlayerStat>();
        }
    }
}
