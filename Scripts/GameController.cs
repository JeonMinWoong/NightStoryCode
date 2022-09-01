using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameController : Singleton<GameController>
{
    private MainCanvas mainCanvas;         // 게임 오버 재생
    private Canvas playerCanvas;
    private Canvas cinemaCanvas;
    private CanvasGroup noticeTextOJ;
    private MiniMapSetting miniMap;
    private Animator skillUi;
    
    private TextMeshProUGUI stageText;
    private TextMeshProUGUI mapNameText;

    public GameObject player;
    public PlayerStat playerStat;
    PlayerController playerController;
    public Transform[] stageGroup;
    [SerializeField]
    public StageClear[] stageClear;
    public GameObject boss;
    public Bosszon bosszon;

    [SerializeField]
    private int mapLv = 1;
    [SerializeField]
    private int stageCount = 1;
    public string stageName;
    public int StageCount
    {
        get { return stageCount; }
        set { stageCount = value; }
    }

    public int lvCount { get { return mapLv; } set { mapLv = value; } }

    public bool playIng = false;
    private bool isGameOver = false;
    public bool isGameClear { get; private set;}
    public bool reset = false;
    public Vector2 originPos { get; set; }

    public IEnumerator Start()
    {
        Transform stageParent = GameObject.Find("Stage").transform;

        stageGroup = new Transform[stageParent.childCount];
        stageClear = new StageClear[stageParent.childCount];

        for (int i = 0; i < stageParent.childCount; i++)
        {
            stageGroup[i] = stageParent.GetChild(i);
            stageClear[i] = stageGroup[i].GetComponent<StageClear>();
        }

        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerStat = player.GetComponent<PlayerStat>();
            playerController = player.GetComponent<PlayerController>();

            yield return null;
        }

        Init();

        if (reset == true)
        {
            lvCount = 1;
            reset = false;
        }

        if (playIng == false)
        {
            playerStat.Init();
            player.GetComponent<PlayerHealth>().Init();
            playerController.ControllerInit();
            playIng = true;
        }
        else if (playIng == true)
        {
            mainCanvas.noticeAni[2].Play("Revive");
            player.GetComponent<PlayerHealth>().Revive();
            AudioManager.Instance.PlaySound("Revive", player.transform.position, 1f + Random.Range(-0.1f, 0.1f));
        }

        MemoryPosLoad();

        ScoreManager.Instance.Init();
        ObjectManager.Instance.StageInit();
        mapNameText.text = ObjectManager.Instance.sceneNick;
        if (playerStat.playerAbility.mapInpo != 0)
        {
            mainCanvas.noticeAni[6].Play("StageIntro");
            //AudioManager.Instance.PlaySound("Intro", player.transform.position, 1f + Random.Range(-0.1f, 0.1f));
            MapReset(0, ObjectManager.Instance.sceneNick + " " + mapLv.ToString() + "-" + stageCount.ToString());
        }
    }

    void Init()
    {
        mainCanvas = GameObject.Find("MainCanvas").GetComponent<MainCanvas>();
        miniMap = mainCanvas.transform.Find("MiniMapUI").GetComponent<MiniMapSetting>();
        skillUi = mainCanvas.transform.Find("SkillUi").GetComponent<Animator>();
        stageText = mainCanvas.transform.Find("NoticeUI").GetChild(5).GetComponentInChildren<TextMeshProUGUI>(true);
        mapNameText = mainCanvas.transform.Find("NoticeUI").GetChild(6).GetComponentInChildren<TextMeshProUGUI>(true);
        playerCanvas = GameObject.Find("PlayerCanvas").GetComponent<Canvas>();
        cinemaCanvas = GameObject.Find("CinemaCanvas").GetComponent<Canvas>();
        noticeTextOJ = playerCanvas.transform.Find("Interface/NoticeText").GetComponent<CanvasGroup>();
    }

    public void MemoryPosLoad()
    {
        if (playerStat.isLoadData)
        {
            if (playerStat.CurrentSaveLvStage == "" || playerStat.CurrentSaveLvStage == null)
                return;

            if (!playerStat.playerAbility.stageInpo[mapLv - 1].stageInpo[stageCount - 1])
                return;

            string[] lv_stage = playerStat.CurrentSaveLvStage.Split('-');

            int.TryParse(lv_stage[0], out int saveLvCount);
            int.TryParse(lv_stage[1], out int saveStageCount);

            StartCoroutine(Next_Stage(saveStageCount, lv_stage[2]));
        }
        else
        {
            Debug.Log("저장 없음");
        }    
    }

    void Update()
    {
        if (ScoreManager.Instance.playerStat == null)
            return;

        if (ScoreManager.Instance.ScoreGroup == 3 + lvCount * 2)
        {
            if(StageCount == ObjectManager.Instance.mapCount[mapLv-1] - 1 && stageClear[stageCount - 1].isStageClear == false)
                OpenBossRoom();
            else if (StageCount != ObjectManager.Instance.mapCount[mapLv - 1] - 1 && stageClear[stageCount-1].isStageClear == false)
                StageClear();
        }
    }

    public void GameOver()
    {
        if (isGameOver)
            return;

        mainCanvas.noticeAni[0].Play("GameOver");             //게임 오버 애니메이션 재생
        AudioManager.Instance.PlaySound("Death", player.transform.position, 1f + Random.Range(-0.1f, 0.1f));
        isGameOver = true; //게임 오버 확인

        StartCoroutine(RestartLevel());      // 재시작
    }

    public IEnumerator LifeEnd()
    {
        mainCanvas.noticeAni[0].Play("GameOver");
        AudioManager.Instance.PlaySound("Death", player.transform.position, 1f + Random.Range(-0.1f, 0.1f));
        yield return new WaitForSeconds(2f);
        GameManager.Instance.ManagerDestroy();
    }

    void OpenBossRoom()
    {
        if (boss == null)
        {
            AudioManager.Instance.PlaySound("BossOpen", player.transform.position, 1f + Random.Range(-0.1f, 0.1f));
            mainCanvas.noticeAni[3].Play("Open");
            stageGroup[stageCount - 1].GetComponent<StageClear>().isStageClear = true;
        }
    }

    public void Boss()
    {
        if (!boss.activeSelf)
        {
            if (isGameClear == true)
                return;

            StartCoroutine(BossZonSetting());
            boss.GetComponent<SpriteRenderer>().flipX = true;
            boss.SetActive(true);
            AudioManager.Instance.PlaySound("warning", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            mainCanvas.noticeAni[4].Play("Warning");
        }
    }

    IEnumerator BossZonSetting()
    {
        Transform stageActive = null;
        
        for (int i = 0; i < stageGroup.Length; i++)
        {
            if (stageGroup[i].gameObject.activeSelf)
                stageActive = stageGroup[i];
        }

        Transform camPos = stageActive.Find("BossCam");
        boss.GetComponent<EnemyController>().enabled = false;
        MainCanvas(true);
        playerController.StageIntro();

        yield return new WaitUntil(() => !player.GetComponent<PlayerController>().isStop);

        GameObject cam = player.GetComponent<CameraShake>().cam.gameObject;
        Vector3 prevPos = boss.transform.position - player.transform.position;
        CinemachineFramingTransposer basicCam = player.GetComponent<CameraShake>().basicVc.GetCinemachineComponent<CinemachineFramingTransposer>();
        float dist = 0;

        CinemaManager.Instance.isBossCinema = true;

        if (!playerStat.playerAbility.stageInpo[lvCount - 1].stageInpo[StageCount - 1])
        {
            CinemaManager.Instance.EventSet(5);
            CinemaManager.Instance.CinemaLog();
        }
        else
            CinemaManager.Instance.isCinemaEnd = true;

        playerController.isCinema = true;
        while (true)
        {
            dist = basicCam.m_TrackedObjectOffset.x - prevPos.x;
            basicCam.m_TrackedObjectOffset = Vector3.Lerp(basicCam.m_TrackedObjectOffset, new Vector3(prevPos.x, basicCam.m_TrackedObjectOffset.y), 0.02f);
            if (Mathf.Abs(dist) <= 2)
                break;
            yield return null;
        }

        CinemaManager.Instance.SkipButtonActive(true);
        yield return new WaitUntil(() => CinemaManager.Instance.isCinemaEnd);

        cam.transform.position = camPos.position;
        cam.GetComponent<PolygonCollider2D>().points = camPos.GetComponent<PolygonCollider2D>().points;
        Destroy(cam.GetComponent<CinemachineConfiner>());
        CinemachineConfiner confiner = cam.AddComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = cam.GetComponent<PolygonCollider2D>();
        confiner.m_ConfineScreenEdges = true;

        prevPos = Vector2.zero;
        while (true)
        {
            dist = basicCam.m_TrackedObjectOffset.x - prevPos.x;
            basicCam.m_TrackedObjectOffset = Vector3.Lerp(basicCam.m_TrackedObjectOffset,
                new Vector3(prevPos.x, basicCam.m_TrackedObjectOffset.y), 0.05f);
            if (Mathf.Abs(dist) <= 2)
                break;
            yield return null;
        }

        boss.GetComponent<EnemyController>().enabled = true;
        playerController.isCinema = false;

        yield return new WaitUntil(() => !player.GetComponent<PlayerController>().isCinema);


        GameObject bossRange = stageActive.Find("BossRange").gameObject;
        bossRange.SetActive(true);
    }

    public void MainCanvas(bool isActive)
    {
        mainCanvas.GetComponent<Canvas>().enabled = isActive;
        playerCanvas.enabled = isActive;
        cinemaCanvas.enabled = !isActive;
    }

    void CamReset()
    {
        Transform stageActive = null;
        for (int i = 0; i < stageGroup.Length; i++)
        {
            if (stageGroup[i].gameObject.activeSelf)
                stageActive = stageGroup[i];
        }

        Transform centerPos = stageActive.Find("Center");
        GameObject cam = player.GetComponent<CameraShake>().cam.gameObject;
        cam.transform.position = centerPos.position;
        cam.GetComponent<PolygonCollider2D>().points = centerPos.GetComponent<PolygonCollider2D>().points;
        Destroy(cam.GetComponent<CinemachineConfiner>());
        CinemachineConfiner confiner = cam.AddComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = cam.GetComponent<PolygonCollider2D>();
        confiner.m_ConfineScreenEdges = true;

        GameObject bossRange = stageActive.Find("BossRange").gameObject;
        bossRange.SetActive(false);
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(3f);
       
        mainCanvas.noticeAni[2].Play("Rivive");
        AudioManager.Instance.PlaySound("Revive", player.transform.position, 1f + Random.Range(-0.1f, 0.1f));
        player.transform.position = originPos;
        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        playerController.enabled = true;
        ph.enabled = true;
        player.SetActive(true);
        isGameOver = false;
        player.GetComponent<PlayerHealth>().Revive();
        ObjectManager.Instance.ResetObject();

        if (boss != null)
        {
            Stat es = boss.GetComponent<Stat>();
            EnemyHealth eh = boss.GetComponent<EnemyHealth>();

            if (eh.isDeath)
                yield break ;
            if (!boss.activeSelf)
                yield break;

            es.CurrentHealth = es.MaxHealth;
            CinemaManager.Instance.BossReEvent();
            boss.SetActive(false);
            boss.transform.position = ObjectManager.Instance.bossPos.position;
            es.GetComponent<Stat>().bossHpBar.transform.GetChild(0).gameObject.SetActive(false);
            bosszon.bossZon = false;
            CamReset();
        }
    }

    public void StageClear()
    {
        AudioManager.Instance.PlaySound("NextStage", player.transform.position, 1f + Random.Range(-0.1f, 0.1f));
        mainCanvas.noticeAni[1].Play("GameClearAni");
        stageGroup[stageCount-1].GetComponent<StageClear>().isStageClear = true;
        playerStat.playerAbility.stageInpo[lvCount - 1].stageInpo[stageCount - 1] = true;
    }

    public void MapClear()
    {
        isGameClear = true;
        playerStat.playerAbility.stageInpo[lvCount - 1].stageInpo[StageCount - 1] = true;
        GameObject endGate = stageGroup[stageCount - 1].Find("Door/EndGate").gameObject;
        endGate.SetActive(true);
        mainCanvas.noticeAni[7].Play("MapClear");
        AudioManager.Instance.PlaySound("BossDie", Camera.main.transform.position);
        CamReset();
    }

    public void SkillUse()
    {
        skillUi.SetTrigger("IsSkillUi");
        StartCoroutine(SkillUseEnd());
    }

    IEnumerator SkillUseEnd()
    {
        yield return new WaitForSeconds(0.7f);
        skillUi.SetTrigger("IsSkillUiEnd");
    }

    public IEnumerator Tutorial_Stage()
    {
        playerController.StageIntro();
        while (stageText == null)
            yield return null;
        mainCanvas.noticeAni[6].Play("Idle");
        mainCanvas.noticeAni[5].Play("NextStage");
        stageText.text = "<size=125%>" + "Tutorial" + "</size>\n";

        stageGroup[stageCount - 1].gameObject.SetActive(false);
        stageGroup[7].gameObject.SetActive(true);

        Transform centerPos = stageGroup[7].Find("Center");
        GameObject cam = player.GetComponent<CameraShake>().cam.gameObject;
        cam.transform.position = centerPos.position;
        cam.GetComponent<PolygonCollider2D>().points = centerPos.GetComponent<PolygonCollider2D>().points;
        Destroy(cam.GetComponent<CinemachineConfiner>());
        CinemachineConfiner confiner = cam.AddComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = cam.GetComponent<PolygonCollider2D>();
        confiner.m_ConfineScreenEdges = true;
        Transform startPos = stageGroup[7].Find("StartPos");
        player.transform.position = startPos.position;

        stageCount = 8;
        //ObjectManager.Instance.StageInit();
        //yield return new WaitForSeconds(0.1f);
        MapReset(7, "Tutorial");
    }

    public void TutorialEnd()
    {
        playerStat.playerAbility.mapInpo++;
        QuestWindow.Instance.MapToggleInit();
        playerStat.SaveButton();
        Debug.Log("저장완료");

    }

    public IEnumerator Next_Stage(int count, string _stageName)
    {
        playerController.StageIntro();
        if(count != 1)
            mainCanvas.noticeAni[5].Play("NextStage");
        else
            mainCanvas.noticeAni[6].Play("StageIntro");

        stageText.text = "<size=125%>" + _stageName + "</size>\n";
        
        yield return new WaitForSeconds(1f);
        stageGroup[stageCount - 1].gameObject.SetActive(false);
        stageGroup[count - 1].gameObject.SetActive(true);

        Transform centerPos = stageGroup[count - 1].Find("Center");
        GameObject cam = player.GetComponent<CameraShake>().cam.gameObject;
        cam.transform.position = centerPos.position;
        cam.GetComponent<PolygonCollider2D>().points = centerPos.GetComponent<PolygonCollider2D>().points;
        Destroy(cam.GetComponent<CinemachineConfiner>());
        CinemachineConfiner confiner = cam.AddComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = cam.GetComponent<PolygonCollider2D>();
        confiner.m_ConfineScreenEdges = true;

        Transform startPos = stageGroup[count - 1].Find("StartPos");

        player.transform.position = startPos.position;

        stageCount = count;

        GameObject bossZonCheck = stageGroup[count - 1].Find("BossZon")?.gameObject;

        if (bossZonCheck != null)
        {
            bosszon = bossZonCheck.GetComponent<Bosszon>();
        }
        ObjectManager.Instance.StageInit();

        MapReset(count - 1, _stageName);
        stageName = _stageName;
    }

    public IEnumerator PlayerReDoor(bool _isNext)
    {
        if(_isNext)
            playerController.PrevStageIntro();
        else
            playerController.StageIntro();

        Physics2D.IgnoreLayerCollision(9, 10, true);

        yield return new WaitUntil(() => playerController.coIntro == null);

        CinemaManager.Instance.PlayerReDoorCinema();
    }

    public void MapReset(int count,string stageName)
    {
        miniMap.MiniMapSet(count,stageName);
    }

    public void InputMiniMap()
    {
        if (miniMap.mapTypeGroup[0].activeSelf)
        {
            miniMap.MapTypeButton(0);
        }
        else if (miniMap.mapTypeGroup[1].activeSelf)
        {
            miniMap.MapTypeButton(2);
        }
        else if(miniMap.mapTypeGroup[2].activeSelf)
        {
            miniMap.MapTypeButton(1);
        }
    }

    public void NoticeSet(int count,string addText)
    {
        noticeTextOJ.GetComponent<NoticeText>().Notice(count, addText);
    }

    public void NoticeSet_Off(int count)
    {
        noticeTextOJ.GetComponent<NoticeText>().NoticeOff(count);
    }

    public IEnumerator Prev_Stage(int count,string _stageName)
    {
        playerController.PrevStageIntro();
        mainCanvas.noticeAni[5].Play("NextStage");
        stageText.text = "<size=125%>" + _stageName + "</size>\n";
        playIng = false;
        yield return new WaitForSeconds(1f);
        stageGroup[stageCount - 1].gameObject.SetActive(false);
        stageGroup[count - 1].gameObject.SetActive(true);

        Transform centerPos = stageGroup[count - 1].Find("Center");
        GameObject cam = player.GetComponent<CameraShake>().cam.gameObject;
        cam.transform.position = centerPos.position;
        cam.GetComponent<PolygonCollider2D>().points = centerPos.GetComponent<PolygonCollider2D>().points;
        Destroy(cam.GetComponent<CinemachineConfiner>());
        CinemachineConfiner confiner = cam.AddComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = cam.GetComponent<PolygonCollider2D>();
        confiner.m_ConfineScreenEdges = true;
        Transform endPos = stageGroup[count - 1].Find("EndPos_" + stageCount);
        player.transform.position = endPos.position;

        originPos = stageGroup[count - 1].Find("RePoint").transform.position;

        stageCount = count;

        ObjectManager.Instance.StageInit();
        MapReset(count - 1, _stageName);
        stageName = _stageName;
    }

    public IEnumerator StageEnd()
    {
        UIManager.Instance.SaveCanvas();
        playIng = false;
        playerStat.SaveButton();
        yield return new WaitForSeconds(2f);
        LoadingManager.Instance.LoadScene("MapSelect");
    }
}
