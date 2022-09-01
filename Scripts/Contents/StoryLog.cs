using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


[System.Serializable]
public struct StoryProgress
{
    public int progressCount;
    public int maxProgressCount;
    public int questProgressCount;
    public int maxQuestProgressCount;
}

[System.Serializable]
public struct LogGroup
{
    public List<string> log;
}

public class StoryLog : MonoBehaviour, IPointerClickHandler
{
    public Enums.NpcType npcType;
    [SerializeField]
    Canvas logCanvas;
    [SerializeField]
    Text mainText;
    [SerializeField]
    Text subText;
    [SerializeField]
    GameObject clickImage;

    [SerializeField]
    LogGroup[] textStruct = new LogGroup[3];
    [SerializeField]
    int textCount;
    [SerializeField]
    int maxCount;
    [SerializeField]
    LogGroup[] questStruct = new LogGroup[3];
    int questTextCount;
    int questMaxCount;


    GameObject player;
    public PlayerStat playerStat;
    public Coroutine coText;
    public StoryProgress storyProgress;
    public bool isQuest;
    private void Awake()
    {
        LogInit();
        logCanvas = transform.Find("LogCanvas").GetComponent<Canvas>();
        playerStat = FindObjectOfType<PlayerStat>();
        player = playerStat.gameObject;
        
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => playerStat.isLoadData);
        SetStoryLog();
    }

    void SetStoryLog()
    {
        storyProgress.progressCount = playerStat.playerAbility.storyProgressGroup[(int)npcType].progressCount;
        storyProgress.questProgressCount = playerStat.playerAbility.storyProgressGroup[(int)npcType].questProgressCount;

        if (textStruct.Length > storyProgress.progressCount)
            maxCount = textStruct[storyProgress.progressCount].log.Count;
        else
            maxCount = 0;

        if (questStruct.Length > storyProgress.questProgressCount)
            questMaxCount = questStruct[storyProgress.questProgressCount].log.Count;
        else
            questMaxCount = 0;
    }

    void TextGroupInit(int _maxProgress, int _maxQuestProgress)
    {
        textStruct = new LogGroup[_maxProgress];
        for (int i = 0; i < textStruct.Length; i++)
        {
            textStruct[i].log = new List<string>();
        }
        
        questStruct = new LogGroup[_maxQuestProgress];
        for (int i = 0; i < questStruct.Length; i++)
        {
            questStruct[i].log = new List<string>();
        }
    }

    void LogInit()
    {
        if (transform.name.Contains("Blacksmith"))
            npcType = Enums.NpcType.Blacksmith;
        else if (transform.name.Contains("Trinkets"))
            npcType = Enums.NpcType.Trinkets;
        else if (transform.name.Contains("Traveling"))
            npcType = Enums.NpcType.Traveling;
        else if (transform.name.Contains("Gravedigger"))
            npcType = Enums.NpcType.Gravekeeper;
        else if(transform.name.Contains("WizardHouse"))
            npcType = Enums.NpcType.WizardHouse;
        else if (transform.name.Contains("Ashes"))
            npcType = Enums.NpcType.Ashes;

        TextGroupInit(storyProgress.maxProgressCount,storyProgress.maxQuestProgressCount);

        switch (npcType)
        {
            case Enums.NpcType.Blacksmith:
                textStruct[0].log.Add("허허.. 자네를 보니 예전 생각이 많이나는 구먼..");
                textStruct[0].log.Add("나도 젊었을 때는 자네처럼\n여행을 했었지..");
                textStruct[0].log.Add("아 미안하다네.. 기억을 찾고 있는 거 였었지");
                textStruct[0].log.Add("그래도 너무 걱정하며 살지 말게");
                textStruct[0].log.Add("몸만 건강하면 언젠간 찾을 수 있지 않겠는가?");
                textStruct[0].log.Add("몸 안다치게 조심하고\n필요한 거 있으면 언제든 들리게");

                questStruct[0].log.Add("자네...?\n처음 보는 군?");
                questStruct[0].log.Add("어쩌다 여기까지 오게 된 건가?");
                questStruct[0].log.Add("기억을 잃었다고?\n안타깝구만...");
                questStruct[0].log.Add("자네한텐 미안하지만...\n내가 도와 줄 수 있는 건 없다네...");
                questStruct[0].log.Add("하지만 장비가 필요하면 언제든 들리게");
                questStruct[0].log.Add("기억을 찾길 바라");
                break;
            case Enums.NpcType.Trinkets:
                textStruct[0].log.Add("안녕하세요 기사님!");
                textStruct[0].log.Add("요즘 마물들이 극성이라..\n안 다치게 조심히 다녀야해요.");
                textStruct[0].log.Add("아 그리고 마법사님은\n유적들이 보이는 곳 근처에 있답니다.");
                textStruct[0].log.Add("가보진 않았지만 들리는 소문으로는\n깊은 낭떨어지에 산다고 들었어요.");
                textStruct[0].log.Add("꼭 도움이 되었으면 좋겠네요.");

                questStruct[0].log.Add("앗 깜짝이야...");
                questStruct[0].log.Add("죄송해요. 이 근방 사람이 아닌 것 같아서 놀랬어요.");
                questStruct[0].log.Add("그렇군요...\n기억을 잃으셨군요...");
                questStruct[0].log.Add("제가 도움을 드릴 수 있으면 좋을 텐데..");
                questStruct[0].log.Add("아! 깊은 숲에 사는 대마법사가 있어요!");
                questStruct[0].log.Add("그 분에게 가시면 도움이 될 수도 있을 것 같아요!");
                break;
            case Enums.NpcType.Traveling:
                textStruct[0].log.Add("헤이 친구!");
                textStruct[0].log.Add("나도 여행자여서 이런말 하기 그렇지만..");
                textStruct[0].log.Add("주민들하고 친해지는 게 좋아\n전투에 필요한 물품이나 정보를 받게 될 수도 있어");
                textStruct[0].log.Add("물론 나하고도 친해지는 게 좋고!");
                textStruct[0].log.Add("왜냐고?! 나는 한 곳에만 머물러 있지 않아서\n다른 곳에서 도움을 받을 수도 있거든");
                textStruct[0].log.Add("뭐 결국 너의 선택이지만 말이야");

                questStruct[0].log.Add("안녕!");
                questStruct[0].log.Add("나는 떠돌이 상인이라서 너를 도와줄 수 없을 것 같아.");
                questStruct[0].log.Add("또 보자고!");
                break;
            case Enums.NpcType.Gravekeeper:
                textStruct[0].log.Add("죽음은...");
                textStruct[0].log.Add("슬픈 것이지만\n그것이 내가 먹고 살 수 있는 이유라니..");
                textStruct[0].log.Add("너는 조심히 다녀라..\n죽음 만큼 허무한 것도 없어..");

                questStruct[0].log.Add("뭐??");
                questStruct[0].log.Add("너가 누군지 내가 어떻게 알어!!!!");
                questStruct[0].log.Add("지금 바쁜거 안보여?");
                questStruct[0].log.Add("도와 줄 거 아니면 저리가라");
                break;
            case Enums.NpcType.WizardHouse:
                textStruct[0].log.Add("너무 잘 풀린다고 생각해지만\n역시 생각보다 쉽지 않을 것 같다.");
                textStruct[0].log.Add("고요의 숲..이라..");

                questStruct[0].log.Add("잡화상인이 말한 곳 같지만 마법사는 없다.");
                questStruct[0].log.Add("마법사의 연구 자료에서\n고요의 숲으로 간다는 이야기가 있었다.");
                questStruct[0].log.Add("더 이상 찾아볼 건 없으니\n고요의 숲으로 가서 찾아야봐야 할 것 같다.");
                break;
            case Enums.NpcType.Ashes:
                textStruct[0].log.Add(".......");
                textStruct[0].log.Add("누군가의 유골이다..");
                textStruct[0].log.Add("크기가 작은 것으로 보아 어린아이로\n추정된다..");
                textStruct[0].log.Add("혹시 누군가가 찾고 있을 지 모른다.\n가져갈까?");

                questStruct[0].log.Add("무덤지기가 말한 유골이다.");
                questStruct[0].log.Add("기다리고 있을 유족들을 위해\n서둘러 가져다 주기로 하자.");
                break;
            default:
                break;
        }
        maxCount = textStruct[storyProgress.progressCount].log.Count;
        questMaxCount = questStruct[storyProgress.questProgressCount].log.Count;
    }


    public IEnumerator SetText()
    {
        if (textCount >= maxCount)
        {
            storyProgress.progressCount++;
            player.GetComponent<PlayerStat>().playerAbility.storyProgressGroup[(int)npcType].progressCount = storyProgress.progressCount;
            //isFirstLog = true;
            coText = null;

            if (textStruct.Length > storyProgress.progressCount)
            {
                textCount = 0;
                maxCount = textStruct[storyProgress.progressCount].log.Count;
            }
            if (npcType == Enums.NpcType.Ashes)
            {
                string[] textGroup = new string[2];
                textGroup[0] = "가져간다.";
                textGroup[1] = "안 가져간다.";
                SelectMission(textGroup);

                yield return new WaitUntil(() => UIManager.Instance.selectUI.currentCount != 0);

                AshesSelect(UIManager.Instance.selectUI.currentCount);
            }

            player.GetComponent<PlayerController>().isStop = false;
            if(GetComponent<Store>() != null)
                transform.GetChild(0).gameObject.SetActive(true);

            logCanvas.gameObject.SetActive(false);
            yield break;
        }
        else
        {
            player.GetComponent<PlayerController>().isStop = true;
            transform.GetChild(0).gameObject.SetActive(false);
            logCanvas.gameObject.SetActive(true);
        }
        int count = 0;
        subText.text = "";
        mainText.text = textStruct[0].log[textCount];
        clickImage.SetActive(false);
        while (textStruct[0].log[textCount] != subText.text)
        {
            subText.text += textStruct[0].log[textCount][count];
            count++;
            //Debug.Log(subText.text);
            AudioManager.Instance.PlaySound("LogSound", gameObject.transform.position);
            yield return new WaitForSeconds(0.06f);
        }
        clickImage.SetActive(true);

        textCount++;
        coText = null;
    }

    public IEnumerator QuestSetText()
    {
        if (questTextCount >= questMaxCount)
        {
            storyProgress.questProgressCount++;
            player.GetComponent<PlayerStat>().playerAbility.storyProgressGroup[(int)npcType].questProgressCount = storyProgress.questProgressCount;

            if (questStruct.Length > storyProgress.questProgressCount)
            {
                questTextCount = 0;
                questMaxCount = questStruct[storyProgress.questProgressCount].log.Count;
            }
            coText = null;
            player.GetComponent<PlayerController>().isStop = false;
            if (GetComponent<Store>() != null)
                transform.GetChild(0).gameObject.SetActive(true);

            logCanvas.gameObject.SetActive(false);
            QuestCheck();
            isQuest = false;
            yield break;
        }
        else
        {
            player.GetComponent<PlayerController>().isStop = true;
            transform.GetChild(0).gameObject.SetActive(false);
            logCanvas.gameObject.SetActive(true);
            isQuest = true;
        }
        int count = 0;
        subText.text = "";
        mainText.text = questStruct[0].log[questTextCount];
        clickImage.SetActive(false);
        while (questStruct[0].log[questTextCount] != subText.text)
        {
            subText.text += questStruct[0].log[questTextCount][count];
            count++;
            //Debug.Log(subText.text);
            AudioManager.Instance.PlaySound("LogSound", gameObject.transform.position);
            yield return new WaitForSeconds(0.06f);
        }
        clickImage.SetActive(true);

        questTextCount++;
        coText = null;
    }

    public void OnPointerClick(PointerEventData eventData = null)
    {
        if (player == null)
            return;

        if (coText != null)
            return;

        Debug.Log("다음으로");
        if (isQuest)
            coText = StartCoroutine(QuestSetText());
        else
            coText = StartCoroutine(SetText());
    }

    void QuestCheck()
    {
        List<QuestList> playerQuest = player.GetComponent<PlayerStat>().playerAbility.playerQuests;
        for (int i = 0; i < playerQuest.Count; i++)
        {
            Debug.Log("유골-F");
            if (playerQuest[i].questNubmer == 102 && !playerQuest[i].isEnd)
            {
                switch (npcType)
                {
                    case Enums.NpcType.Blacksmith:
                        UIManager.Instance.QuestPlusGroup(playerQuest[i],1,0);
                        QuestWindow.Instance.SetToggle(playerQuest[i]);
                        break;
                    case Enums.NpcType.Trinkets:
                        UIManager.Instance.QuestPlusGroup(playerQuest[i], 1, 1);
                        QuestWindow.Instance.SetToggle(playerQuest[i]);
                        break;
                    case Enums.NpcType.Traveling:
                        UIManager.Instance.QuestPlusGroup(playerQuest[i], 1, 2);
                        QuestWindow.Instance.SetToggle(playerQuest[i]);
                        break;
                    case Enums.NpcType.Gravekeeper:
                        UIManager.Instance.QuestPlusGroup(playerQuest[i], 1, 3);
                        QuestWindow.Instance.SetToggle(playerQuest[i]);
                        break;
                    default:
                        break;
                }
            }
            else if (playerQuest[i].questNubmer == 103 && !playerQuest[i].isEnd)
            {

                switch (npcType)
                {
                    case Enums.NpcType.WizardHouse:
                        UIManager.Instance.QuestPlus(playerQuest[i], 1);
                        UIManager.Instance.ActiveWindow(3);
                        QuestWindow.Instance.SetToggle(playerQuest[i]);
                        break;
                    default:
                        break;
                }
            }
            else if(playerQuest[i].questNubmer == 26 && !playerQuest[i].isEnd)
            {
                Debug.Log("유골");
                if(npcType == Enums.NpcType.Ashes)
                {
                    UIManager.Instance.QuestPlus(playerQuest[i], 1);
                    AshesSelect(1);
                    return;
                }
            }
        }
    }

    void SelectMission(string[] textGroup)
    {
        UIManager.Instance.selectUI.SelectSet(textGroup);
    }

    void AshesSelect(int count)
    {
        if(count == 1)
        {
            Debug.Log("유골2");
            Inventory.Instance.NewItem("106",1);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("안 가져감");
        }
    }
}
