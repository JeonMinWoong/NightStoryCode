using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class CinemaText
{
    public Dictionary<int, LogSet> log = new Dictionary<int, LogSet>();
}
[System.Serializable]
public class LogSet
{
    public string mainLog;
    public string target;
    public int logCount;

    public LogSet(string _mainLog, string _target,int _logCount)
    {
        mainLog = _mainLog;
        target = _target;
        logCount = _logCount;
    }
}

public class CinemaManager : Singleton<CinemaManager>
{
    PlayerStat playerStat;
    [SerializeField]
    Enums.MapStory mapStory;

    [SerializeField]
    CinemaText[] cinemaText = new CinemaText[7];

    [SerializeField]
    List<LogSet> mainLogs = new List<LogSet>();

    [SerializeField]
    string[] deathText = new string[5];
    int deathTextCount;
    int maxDeathTextCount;
    public bool isDeathEvent;

    [SerializeField]
    int currentTextCount;
    [SerializeField]
    int maxTextCount;

    int currentReDoorCount = 0;
    int MaxReDoorCount = 1;


    [SerializeField]
    GameObject player;
    GameObject boss;

    Button cinemaSkipButton;
    Canvas canvasLog;
    Image nextLogClick;

    Text mainText;
    Text subText;

    GameObject clickImage;

    Coroutine coText;

    public int eventCount;

    public bool isCinemaEnd;
    public bool isBossCinema;
    public bool isReDoor;
    private void Start()
    {
        playerStat = FindObjectOfType<PlayerStat>();
        player = playerStat.gameObject;
        nextLogClick = GameObject.Find("CinemaCanvas").transform.GetChild(2).GetComponent<Image>();
        cinemaSkipButton = GameObject.Find("CinemaCanvas").GetComponentInChildren<Button>(true);
        cinemaSkipButton.onClick?.AddListener(OnSkip);
    }

    public void LogInit()
    {
        for (int i = 0; i < cinemaText.Length; i++)
        {
            cinemaText[i] = new CinemaText();
        }
        //Debug.Log(playerStat.playerAbility.mapInpo);
        if (playerStat.playerAbility.mapInpo <= 1)
            mapStory = Enums.MapStory.Map_1;
        else
            mapStory = Enums.MapStory.None;

        switch (mapStory)
        {
            case Enums.MapStory.Map_1:
                int count = 0;
                int length = 1;

                cinemaText[0].log.Add(length++, new LogSet("으..으윽..\n여..여기가 어디지?", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("... 내 이름이 뭐지..?", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("기억이 없다...", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("가지고 있는 거라곤 목걸이 하나 밖에 없다.", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("일단 정신 차리고 마을부터 찾아보자.", "Player", count));
                count++;

                cinemaText[0].log.Add(length++, new LogSet("강한 에너지가 뿜어나오고 있다.", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("아마도 저것을 부셔야\n앞으로 나갈 수 있을 것 같다.", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("기억하고 있자.", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("<color=#FFFF00>" + "결계들을 모두 처리해야만 다음 맵으로\n이전 맵으로 이동할 수 있습니다." + "</color>", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("<color=#FFFF00>" + "그리고 스테이지를 클리어 하게 되면\n결계들은 재소환 되지 않습니다." + "</color>", "Player", count));
                count++;

                cinemaText[0].log.Add(length++, new LogSet("마을로 가는 길들에 마물들이 있다.\n쉽지 않을 것 같다.", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("기억을 잃기 전에 뭘 했는 지는 모르겠지만,\n긴장도 안되고 몸이 가볍다.", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("죽을 지도 모를 상황인데...", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("하지만 멈출 수는 없다.", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("<color=#FFFF00>" + "인벤토리를 열어 물약을 장착하시고,\n전투를 진행해주세요." + "</color>", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("<color=#FFFF00>" + "또한, 비 전투시 기력이 빠르게 회복됩니다.\n그럼 행운을 빕니다." + "</color>", "Player", count));
                count++;

                cinemaText[0].log.Add(length++, new LogSet("마을에 도착한 것 같다.", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("이름이 없는 것은 이상하지만...\n가만히 있을 수는 없다.", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("마을 주민분들이 정보를 가지고 있어야 할텐데..", "Player", count));
                count++;

                cinemaText[0].log.Add(length++, new LogSet("돌이 빛을 내며 띄워져있다.", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("조심해서 나쁠 건 없지만,\n마을 한 가운데 있는 것으로 보아", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("안전한 돌인 것 같다.\n다가가 보자.", "Player", count));
                count++;

                cinemaText[0].log.Add(length++, new LogSet("오랜만에 온 손님이군...", "Boss", count));
                cinemaText[0].log.Add(length++, new LogSet("나는 유적의 수호자 알렌카..\n그대는 누구인가?", "Boss", count));
                cinemaText[0].log.Add(length++, new LogSet("이름이 없는 것인가?", "Boss", count));
                cinemaText[0].log.Add(length++, new LogSet("그럼...", "Boss", count));
                cinemaText[0].log.Add(length++, new LogSet("그대는 나약한가?\n아니면 강한가?", "Boss", count));
                cinemaText[0].log.Add(length++, new LogSet("나약한 자는\n내 앞을 지나갈 수 없다.", "Boss", count));
                cinemaText[0].log.Add(length++, new LogSet("지나가려면 나를 넘어서봐라!!!", "Boss", count));
                count++;

                cinemaText[0].log.Add(length++, new LogSet("다른 곳으로 연결되어 있는 포탈이다.", "Player", count));
                cinemaText[0].log.Add(length++, new LogSet("다음은 고요의 숲인가..", "Player", count));
                break;
            case Enums.MapStory.Map_2:
                break;
            case Enums.MapStory.Map_3:
                break;
            case Enums.MapStory.Map_4:
                break;
            case Enums.MapStory.Map_5:
                break;
            case Enums.MapStory.Map_6:
                break;
            case Enums.MapStory.Map_7:
                break;
            default:
                break;
        }
        deathText = new string[5];
        deathText[0] = "분명히 죽었는데 살아났다..";
        deathText[1] = "영혼이 빠져나가는 기분 빼고는\n상처도 없고, 몸도 멀쩡하다.";
        deathText[2] = "뭐지..?";
        deathText[3] = "<color=#FFFF00>" + "플레이어가 죽으면 중앙의 영혼 게이지가 감소합니다.\n5번의 부활이 있습니다." + "</color>";
        deathText[4] = "<color=#FFFF00>" + "그 이상으로 죽게 될 시 게임이 종료 되고,\n마지막으로 저장한 기억부터 다시 시작할 수 있습니다." + "</color>";
        deathTextCount = 0;


        maxDeathTextCount = deathText.Length;
    }

    public void EventSet(int _eventCount)
    {
        eventCount = _eventCount;
        playerStat.playerAbility.cinemaCount++;
        CinemaTextCheck((int)mapStory - 1);
    }

    void CinemaTextCheck(int count)
    {
        mainLogs.Clear();

        foreach (var item in cinemaText[count].log)
        {
            if (item.Value.logCount == eventCount)
            {
                mainLogs.Add(item.Value);
            }
        }

        maxTextCount = mainLogs.Count;
    }

    public void CinemaLog()
    {
        currentTextCount = 0;
        coText = StartCoroutine(CoCinemaLog());
    }

    public void PlayerReDoorCinema()
    {
        if (coText != null)
            return;

        coText = StartCoroutine(CoPlayerReDoorCinema());
    }

    public void PlayerDeathEvent()
    {
        if (coText != null)
            return;

        coText = StartCoroutine(CoPlayerDeathEvent());
    }

    IEnumerator CoPlayerDeathEvent()
    {
        if (deathTextCount < maxDeathTextCount)
        {
            isDeathEvent = true;
            canvasLog = player.transform.Find("LogCanvas").GetComponent<Canvas>();
            mainText = canvasLog.transform.GetChild(0).GetComponent<Text>();
            subText = canvasLog.transform.GetChild(0).GetChild(1).GetComponent<Text>();
            mainText.text = deathText[deathTextCount];
            EventTrigger();
            clickImage = mainText.transform.GetChild(2).gameObject;
            clickImage.SetActive(false);
            GameController.Instance.MainCanvas(false);
            player.GetComponent<PlayerController>().isCinema = true;
            canvasLog.gameObject.SetActive(true);
            Physics2D.IgnoreLayerCollision(9, 10, true);
        }
        else
        {
            clickImage.SetActive(true);
            GameController.Instance.MainCanvas(true);
            player.GetComponent<PlayerController>().isCinema = false;
            canvasLog.gameObject.SetActive(false);
            playerStat.playerAbility.isDeathEvent = true;
            isDeathEvent = false;
            //SkipButtonActive(false);
            Physics2D.IgnoreLayerCollision(9, 10, false);
            yield break;
        }

        string currentLog = "";

        subText.text = "";

        currentLog = mainText.text;

        int count = 0;

        while (currentLog != subText.text)
        {
            if (currentLog[count] == '<')
                subText.text = currentLog;
            else
            {
                subText.text += currentLog[count];
                count++;
            }
            AudioManager.Instance.PlaySound("LogSound", gameObject.transform.position);
            yield return new WaitForSeconds(0.06f);
        }
        nextLogClick.raycastTarget = true;
        //SkipButtonActive(true);
        deathTextCount++;
        coText = null;
    }

    public IEnumerator CoPlayerReDoorCinema()
    {
        if (currentReDoorCount < MaxReDoorCount)
        {
            isReDoor = true;
            canvasLog = player.transform.Find("LogCanvas").GetComponent<Canvas>();
            mainText = canvasLog.transform.GetChild(0).GetComponent<Text>();
            subText = canvasLog.transform.GetChild(0).GetChild(1).GetComponent<Text>();
            mainText.text = "아직은 갈 수 없다.\n할 일이 남아있다.";
            EventTrigger();
            clickImage = mainText.transform.GetChild(2).gameObject;
            clickImage.SetActive(false);
            GameController.Instance.MainCanvas(false);
            player.GetComponent<PlayerController>().isCinema = true;
            canvasLog.gameObject.SetActive(true);
        }
        else
        {
            clickImage.SetActive(true);
            GameController.Instance.MainCanvas(true);
            player.GetComponent<PlayerController>().isCinema = false;
            canvasLog.gameObject.SetActive(false);
            isReDoor = false;
            //SkipButtonActive(false);
            currentReDoorCount = 0;
            Physics2D.IgnoreLayerCollision(9, 10, false);
            coText = null;
            yield break;
        }

        string currentLog = "";

        subText.text = "";

        currentLog = mainText.text;

        int count = 0;

        while (currentLog != subText.text)
        {
            subText.text += currentLog[count];
            count++;
            AudioManager.Instance.PlaySound("LogSound", gameObject.transform.position);
            yield return new WaitForSeconds(0.06f);
        }
        nextLogClick.raycastTarget = true;
        //SkipButtonActive(true);
        currentReDoorCount++;
        coText = null;
    }

    public IEnumerator CoCinemaLog()
    {
        if (mapStory == Enums.MapStory.None)
        {
            player.GetComponent<PlayerController>().isCinema = true;
            isCinemaEnd = true;
            yield break;
        }

        if (currentTextCount < maxTextCount)
        {
            if (mainLogs[currentTextCount].target.Contains("Player"))
            {
                canvasLog = player.transform.Find("LogCanvas").GetComponent<Canvas>();
                mainText = canvasLog.transform.GetChild(0).GetComponent<Text>();
                subText = canvasLog.transform.GetChild(0).GetChild(1).GetComponent<Text>();
                mainText.text = mainLogs[currentTextCount].mainLog;
                EventTrigger();

            }
            else if (mainLogs[currentTextCount].target.Contains("Boss"))
            {
                boss = GameController.Instance.boss;

                if (boss == null)
                    yield break;

                canvasLog = boss.transform.Find("LogCanvas").GetComponent<Canvas>();
                mainText = canvasLog.transform.GetChild(0).GetComponent<Text>();
                subText = canvasLog.transform.GetChild(0).GetChild(1).GetComponent<Text>();
                mainText.text = mainLogs[currentTextCount].mainLog;
                EventTrigger();
            }
        }

        if (currentTextCount >= maxTextCount)
        {
            canvasLog.gameObject.SetActive(false);
            GameController.Instance.MainCanvas(true);
            coText = null;
            if(!isBossCinema)
                player.GetComponent<PlayerController>().isCinema = false;
            SkipButtonActive(false);
            currentTextCount = 0;
            isCinemaEnd = true;
            yield break;
        }
        else
        {
            GameController.Instance.MainCanvas(false);
            player.GetComponent<PlayerController>().isCinema = true;
            canvasLog.gameObject.SetActive(true);
            isCinemaEnd = false;
            //Debug.Log("시네마");
        }

        clickImage = mainText.transform.GetChild(2).gameObject;
        clickImage.SetActive(false);
        string currentLog = "";

        subText.text = "";

        currentLog = mainText.text;

        int count = 0;
        while (currentLog != subText.text)
        {
            if (currentLog[count] == '<')
                subText.text = currentLog;
            else
            {
                subText.text += currentLog[count];
                count++;
            }
            AudioManager.Instance.PlaySound("LogSound", gameObject.transform.position);
            yield return new WaitForSeconds(0.06f);
        }
        if(!isBossCinema)
            SkipButtonActive(true);

        clickImage.SetActive(true);

        currentTextCount++;
        coText = null;
    }

    void EventTrigger()
    {
        EventTrigger.Entry entry_PointerClick = new EventTrigger.Entry();
        entry_PointerClick.eventID = EventTriggerType.PointerClick;
        entry_PointerClick.callback.AddListener((data) => { OnClick(); });
        nextLogClick.GetComponent<EventTrigger>().triggers.Clear();
        nextLogClick.GetComponent<EventTrigger>().triggers.Add(entry_PointerClick);
    }

    public void OnClick()
    {
        Debug.Log("클릭");

        if (coText != null)
            return;

        if (!isReDoor && !isDeathEvent)
            coText = StartCoroutine(CoCinemaLog());
        else if (!isReDoor && isDeathEvent)
            coText = StartCoroutine(CoPlayerDeathEvent());
        else if(isReDoor && !isDeathEvent)
            coText = StartCoroutine(CoPlayerReDoorCinema());
    }

    public void BossReEvent()
    {
        playerStat.playerAbility.cinemaCount--;
        isCinemaEnd = false;
        isBossCinema = false;
    }
    
    public void SkipButtonActive(bool isActive)
    {
        nextLogClick.raycastTarget = isActive;
        cinemaSkipButton.gameObject.SetActive(isActive);
    }

    public void OnSkip()
    {
        Debug.Log("스킵");
        AudioManager.Instance.PlaySound("Cancel", Camera.main.transform.position);
        currentTextCount = maxTextCount;
        coText = StartCoroutine(CoCinemaLog());
    }
}
