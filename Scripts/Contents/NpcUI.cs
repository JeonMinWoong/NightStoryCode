using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NpcUI : MonoBehaviour
{
    [SerializeField]
    Enums.NpcType npcType;

    [SerializeField]
    Text npcNameText;
    [SerializeField]
    Text npcBasicLog;

    public CanvasGroup inactiveGroup;

    CanvasGroup dialog;
    [SerializeField]
    GameObject npcOJ;

    CanvasGroup questUI;
    QuestController questController;
    [SerializeField]
    Button storeButton;
    [SerializeField]
    Button questButton;

    private void Start()
    {
        dialog = transform.GetChild(0).GetComponent<CanvasGroup>();
        questUI = dialog.transform.GetChild(2).GetComponent<CanvasGroup>();
        questController = GetComponent<QuestController>();
    }

    public void OnNpcUI(GameObject npc)
    {
        StoryLog storyLog = npc.GetComponentInChildren<StoryLog>();

        List<QuestList> playerQuest = UIManager.Instance.playerStat.playerAbility.playerQuests;
        for (int i = 0; i < playerQuest.Count; i++)
        {
            if(!playerQuest[i].isEnd && playerQuest[i].questNubmer == 102)
            {
                if (!PlayStoryLog(npc, true))
                    return;
            }
            else if(!playerQuest[i].isEnd && playerQuest[i].questNubmer == 103)
            {
                if (!PlayStoryLog(npc, true))
                    return;
            }
        }

        if (!PlayStoryLog(npc, false))
            return;

        if (npc.GetComponent<Store>() == null)
            return;


        if (dialog.gameObject.activeSelf)
        {
            if (!OffNpcUI())
                return;
        }
        else
        {
            AudioManager.Instance.PlaySound("NpcOpen", Camera.main.transform.position);
            dialog.gameObject.SetActive(true);
        }
        
        npcNameText.text = npc.GetComponent<Store>().storeType.ToString();

        npcType = (Enums.NpcType)npc.GetComponent<Store>().storeType;

        if(npc.GetComponent<Store>().isNoStore)
            storeButton.interactable = false;
        else
            storeButton.interactable = true;

        QuestNpc questNpc = npc.GetComponent<QuestNpc>();
        switch (npcType)
        {
            case Enums.NpcType.Blacksmith:
                npcBasicLog.text = "기사친구!\n\n좋은 무기! 좋은 방어구!\n\n나의 작품들이 자네의 목숨을\n\n연장하는데 도움이 될 걸세.\n\n천천히둘러보게.";
                break;
            case Enums.NpcType.Trinkets:
                npcBasicLog.text = "안녕하세요! 기사님!\n\n전투에 도움이 되는 포션과\n\n마법이 깃든 장신구를 팔고 있어요!\n\n구경만 해도 좋으니 한번 보고 가세요.\n\n";
                break;
            case Enums.NpcType.Traveling:
                npcBasicLog.text = "여!\n\n쉽게 구하지 못하는 물건을 팔고 있어.\n\n대신 문제 생기면 난 모르는 거야!!!\n\n";
                break;
            case Enums.NpcType.Gravekeeper:
                npcBasicLog.text = "뭍히는 대상이 너가 될 수도 있다.\n\n도와 줄 거 아니면 방해하지마라.\n\n";
                break;
            default:
                break;
        }
        questController.SetQuest(questNpc);
        npcOJ = npc;
    }

    bool PlayStoryLog(GameObject npc,bool isQuest)
    {
        StoryLog storyLog = npc.GetComponentInChildren<StoryLog>();

        int maxLogStoryProgress = 0;

        if (isQuest)
            maxLogStoryProgress = storyLog.storyProgress.maxQuestProgressCount;
        else
            maxLogStoryProgress = storyLog.storyProgress.maxProgressCount;

        if (maxLogStoryProgress > 0)
        {
            int playerStroyProgress = 0;
            int npcStroyProgress = 0;
            if (isQuest)
            {
                playerStroyProgress = storyLog.playerStat.playerAbility.storyProgressGroup[(int)storyLog.npcType].questProgressCount;
                npcStroyProgress = storyLog.storyProgress.questProgressCount;
            }
            else
            {
                playerStroyProgress = storyLog.playerStat.playerAbility.storyProgressGroup[(int)storyLog.npcType].progressCount;
                npcStroyProgress = storyLog.storyProgress.progressCount;
            }
            if (npcStroyProgress == playerStroyProgress && maxLogStoryProgress != npcStroyProgress)
            {
                if (storyLog.coText == null)
                {
                    if(isQuest)
                        storyLog.coText = StartCoroutine(storyLog.QuestSetText());
                    else
                        storyLog.coText = StartCoroutine(storyLog.SetText());
                }
                return false;
            }
        }
        return true;
    }

    public bool OffNpcUI()
    {
        if (dialog.alpha != 0)
        {
            if (questUI.gameObject.activeSelf)
            {
                QuestButton();
                return false;
            }
            else
            {
                if (inactiveGroup.alpha == 1)
                {
                    if(dialog.gameObject.activeSelf)
                        AudioManager.Instance.PlaySound("NpcClose", Camera.main.transform.position);
                    dialog.gameObject.SetActive(false);
                    //Debug.Log("체크");
                }
                return true;
            }
        }
        else
        {
            UIManager.Instance.ActiveWindow(4, npcOJ.name);
            OffStore();
            return false;
        }
    }

    public void StoreButton()
    {
        dialog.alpha = 0;
        dialog.blocksRaycasts = false;
        dialog.interactable = false;
        UIManager.Instance.ActiveWindow(4, npcOJ.name);
        switch (npcType)
        {
            case Enums.NpcType.Blacksmith:
                AudioManager.Instance.PlaySound("BlacksmithOpen", Camera.main.transform.position);
                break;
            case Enums.NpcType.Trinkets:
                AudioManager.Instance.PlaySound("TrinketsOpen", Camera.main.transform.position);
                break;
            case Enums.NpcType.Traveling:
                AudioManager.Instance.PlaySound("TravelingOpen", Camera.main.transform.position);
                break;
            default:
                break;
        }
    }

    public void OffStore()
    {

        dialog.alpha = 1;
        dialog.blocksRaycasts = true;
        dialog.interactable = true;
        AudioManager.Instance.PlaySound("StoreClose", Camera.main.transform.position);
    }

    public void QuestButton()
    {
        questUI.gameObject.SetActive(!questUI.gameObject.activeSelf);
        OnQuest(questUI.gameObject.activeSelf);
        questController.SetQuest(npcOJ.GetComponent<QuestNpc>());
        questController.LogBoxSet();
    }

    public bool QuestButtonFasle(QuestNpc questNpc)
    {
        if (questNpc.questCount + 1 > questNpc.questList.Count)
        {
            questButton.interactable = false;
            return false;
        }
        else
        {
            if (questNpc.questList[questNpc.questCount].possibleLv > UIManager.Instance.playerStat.Level)
                questButton.interactable = false;
            else
                questButton.interactable = true;
            return true;
        }
    }

    public void LimitQuest()
    {
        QuestNpc questNpc = npcOJ.GetComponent<QuestNpc>();

        if (!QuestButtonFasle(questNpc))
            return;

        if (questNpc.questCount + 1 > questNpc.questList.Count)
        {
            questButton.interactable = false;
            return;
        }
        else
        {
            questButton.interactable = true;
        }
        int playerLv = UIManager.Instance.playerStat.playerAbility.level;

        

        if (playerLv >= questNpc.questList[questNpc.questCount].possibleLv)
        {
            questButton.interactable = true;
            questController.SetQuest(questNpc);
            questController.LogBoxSet();
        }
        else
        {
            questButton.interactable = false;
        }
    }

    void OnQuest(bool isActive)
    {
        float alpha = isActive ? 1 : 0;

        if (isActive)
        {
            inactiveGroup.alpha = 0;
            inactiveGroup.blocksRaycasts = false;
            AudioManager.Instance.PlaySound("NpcQuestOpen", Camera.main.transform.position);
            questUI.alpha = 1;
            questUI.blocksRaycasts = true;
        }
        else
        {
            questUI.alpha = 0;
            questUI.blocksRaycasts = false;
            AudioManager.Instance.PlaySound("NpcQuestClose", Camera.main.transform.position);
            inactiveGroup.alpha = 1;
            inactiveGroup.blocksRaycasts = true;
        }
    }
}
