using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class QuestLine
{
    public string mapName;
    public Toggle mapToggle;
    public Toggle[] questChildToggle;
    public Text[] questChildText;
}

public class QuestWindow : Singleton<QuestWindow>
{
    const int yValue = 40;
    [SerializeField]
    ScrollRect questScroll;
    PlayerStat playerStat;

    QuestList[][] questDoubleLists = new QuestList[7][];

    public QuestLine[] questLines;
    public SelectItem selectItem;

    public List<QuestNpc> questNpcs;

    [Header("퀘스트 내용")]
    [SerializeField]
    Text titleNameText;
    [SerializeField]
    Text npcNameText;
    [SerializeField]
    Text logText;

    [Header("퀘스트 세부내용")]
    [SerializeField]
    GameObject[] contents;

    [Header("보상")]
    [SerializeField]
    GameObject amendsItemList;
    [SerializeField]
    GameObject selectList;
    [SerializeField]
    Text amendsGoldText;
    [SerializeField]
    Text amendsExpText;

    [Header("버튼")]
    [SerializeField]
    GameObject beforeButtonGroup;
    [SerializeField]
    GameObject afterButtonGroup;

    [SerializeField]
    GameObject onUI;

    [SerializeField]
    ScrollViewContentText logBox;

    [SerializeField]
    QuestList selectQuest;

    public IEnumerator Start()
    {

        QuestNpc[] _questNpcs = FindObjectsOfType<QuestNpc>(true);

        for (int i = 0; i < _questNpcs.Length; i++)
        {
            questNpcs.Add(_questNpcs[i]);
        }

        playerStat = FindObjectOfType<PlayerStat>();
        logBox = GetComponent<ScrollViewContentText>();
        QuestInit();
        yield return new WaitUntil(() => playerStat.isLoadData);

        MapToggleInit();
        //PlayerQuestInit();
        gameObject.SetActive(false);
    }

    void QuestInit()
    {
        questDoubleLists[0] = new QuestList[10];
        questDoubleLists[1] = new QuestList[10];
    }

    public void PlayerQuestInit()
    {
        for (int i = 0; i < playerStat.playerAbility.playerQuests.Count; i++)
        {
            if(i != playerStat.playerAbility.playerQuests.Count - 1)
                SetPlayerQuest(playerStat.playerAbility.playerQuests[i],isArr: true);
            else
                SetPlayerQuest(playerStat.playerAbility.playerQuests[i]);
        }
    }

    public void MapToggleInit()
    {
        for (int i = 0; i < questLines.Length; i++)
        {
            if (playerStat.MapInpo > i)
            {
                questLines[i].mapToggle.interactable = true;
                questLines[i].mapToggle.transform.GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1);
                questLines[i].mapToggle.transform.GetChild(1).GetComponent<Text>().text = questLines[i].mapName;
            }
            else
            {
                questLines[i].mapToggle.interactable = false;
                questLines[i].mapToggle.transform.GetChild(1).GetComponent<Text>().color = new Color(0.3f, 0.3f, 0.3f);
            }
            for (int childCount = 0; childCount < questLines[i].questChildText.Length; childCount++)
            {
                if (questLines[i].questChildText[childCount].GetComponent<Text>().text.Contains("???"))
                    questLines[i].questChildText[childCount].GetComponent<Toggle>().interactable = false;
            }
        }
    }

    public void MainQuestAdd(int newNumber)
    {
        playerStat.playerAbility.playerQuests.Add(DBManager.Instance.questList[newNumber]);
        SetPlayerQuest(DBManager.Instance.questList[newNumber]);
        AudioManager.Instance.PlaySound("QuestAccept", Camera.main.transform.position);
    }

    public void SetPlayerQuest(QuestList _questList, bool isActive = true, bool isArr = false)
    {
        UIManager.Instance.WindowUpdate(3);
        string[] textChild = _questList.order.Split('-');
        int.TryParse(textChild[0], out int lvCount);
        int.TryParse(textChild[1], out int childCount);
        questDoubleLists[lvCount][childCount] = _questList;
        GameObject questLine = questScroll.content.GetChild(lvCount).GetChild(2).gameObject;

        int value = _questList.questNubmer;
        questLines[lvCount].questChildText[childCount].GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        //Debug.Log("토글 이벤트 추가");
        questLines[lvCount].questChildText[childCount].GetComponent<Toggle>().onValueChanged?.AddListener((toggle) => ToggleQuestSelect(lvCount, childCount, value));
        if (isActive)
        {
            if (_questList.questNubmer > 100)
            {
                questLines[lvCount].questChildText[childCount].text = (childCount + 1) + ". [메인]" + _questList.titleName;
                ColorUtility.TryParseHtmlString("#FFC951", out Color color);
                questLines[lvCount].questChildText[childCount].color = color;
            }
            else
            {
                questLines[lvCount].questChildText[childCount].text = (childCount + 1) + ". [서브]" + _questList.titleName;
                ColorUtility.TryParseHtmlString("#FFFFFF", out Color color);
                questLines[lvCount].questChildText[childCount].color = color;
            }


            questLines[lvCount].questChildText[childCount].GetComponent<Toggle>().interactable = true;

            for (int i = 0; i < questLines[lvCount].questChildText.Length; i++)
            {
                questLines[lvCount].questChildText[i].GetComponent<Toggle>().isOn = false;
            }

            if(!isArr)
                questLines[lvCount].questChildText[childCount].GetComponent<Toggle>().isOn = true;


            if(_questList.isEnd)
            {
                QuestComText(_questList);
            }

            if (!questLine.activeSelf && questLines[lvCount].questChildText[childCount].GetComponent<Toggle>().isOn)
            {
                questLines[lvCount].mapToggle.isOn = true;
            }

            onUI.SetActive(true);
            logBox.SetContents();

            SetPlayerQuestUI(value);
        }
        else
        {
            questLines[lvCount].questChildText[childCount].text = (childCount + 1) + ". ???";
            questLines[lvCount].questChildText[childCount].color = new Color(0.4f, 0.4f, 0.4f);
            questLines[lvCount].questChildText[childCount].GetComponent<Toggle>().isOn = false;
            questLines[lvCount].questChildText[childCount].GetComponent<Toggle>().interactable = false;
            ToggleQuestSelect(lvCount, childCount, value);
        }
    }

    public void SetToggle(QuestList _questList)
    {
        string[] textChild = _questList.order.Split('-');
        int.TryParse(textChild[0], out int lvCount);
        int.TryParse(textChild[1], out int childCount);
        GameObject questLine = questScroll.content.GetChild(lvCount).GetChild(2).gameObject;

        for (int i = 0; i < questLines.Length; i++)
        {
            for (int j = 0; j < questLines[i].questChildToggle.Length; j++)
            {
                questLines[i].questChildToggle[j].isOn = false;
            }
        }

        Debug.Log(lvCount + " / " + childCount + " / " + questLines[lvCount].questChildText[childCount].gameObject.name);

        questLines[lvCount].questChildToggle[childCount].isOn = true;

        onUI.SetActive(true);
        logBox.SetContents();

        SetPlayerQuestUI(_questList.questNubmer);
    }

    public void ButtonQuestArr(int nubmer)
    {
        GameObject questLine = null;
        int childCount = 0;
        questLine = questScroll.content.GetChild(nubmer).GetChild(2).gameObject;
        childCount = questLine.transform.childCount; 
        questLine.GetComponent<ToggleGroup>().SetAllTogglesOff();

        Debug.Log(nubmer);
        if (questLine.activeSelf)
        {
            questLine.SetActive(false);
            for (int i = nubmer + 1; i < questScroll.content.childCount; i++)
            {
                questScroll.content.GetChild(i).localPosition = new Vector2(50, questScroll.content.GetChild(i).localPosition.y + childCount * yValue);
            }
            questScroll.content.sizeDelta = new Vector2(0, 460);
        }
        else
        {
            questLine.SetActive(true);
            for (int i = nubmer + 1; i < questScroll.content.childCount; i++)
            {
                questScroll.content.GetChild(i).localPosition = new Vector2(50, questScroll.content.GetChild(i).localPosition.y - childCount * yValue);
            }
            questScroll.content.sizeDelta = new Vector2(0, 460 + childCount * yValue);
        }
        
    }

    public void ToggleQuestSelect(int mapCount, int childCount, int nubmer)
    {
        ToggleGroup toggleGroup = questLines[mapCount].mapToggle.transform.GetChild(2).GetComponent<ToggleGroup>();
        IEnumerable<Toggle> ts = toggleGroup.ActiveToggles();
        Toggle newToggle = null;
        foreach (var item in ts)
        {
            newToggle = item;
        }

        int check = 0;
        if (toggleGroup.AnyTogglesOn())
        {
            Color textColor = newToggle.GetComponent<Text>().color;
            Color disableColor = new Color(0.4f, 0.4f, 0.4f);
            if (textColor == disableColor)
            {
                Debug.Log("off");
                onUI.SetActive(false);
            }
            else
            {
                Debug.Log("on");
                onUI.SetActive(true);
                logBox.SetContents();
                Toggle checkToggle = questLines[mapCount].questChildText[childCount].GetComponent<Toggle>();
                if (checkToggle.isOn && check == 0)
                {
                    check++;
                    if(playerStat.isLoadData)
                        AudioManager.Instance.PlaySound("QuestLine", Camera.main.transform.position);
                    SetPlayerQuestUI(nubmer);
                }
            }
        }
        else
        {
            onUI.SetActive(false);
        }
    }

    public void AllToogleOff()
    {
        onUI.SetActive(false);
    }

    public void SetPlayerQuestUI(int questNumber)
    {
        for (int count = 0; count < playerStat.playerAbility.playerQuests.Count; count++)
        {
            if (playerStat.playerAbility.playerQuests[count].questNubmer == DBManager.Instance.questList[questNumber].questNubmer)
            {
                Debug.Log(questNumber);
                selectQuest = playerStat.playerAbility.playerQuests[count];

                if (selectQuest.npcName == Enums.NpcType.Main)
                {
                    npcNameText.transform.parent.gameObject.SetActive(false);
                    beforeButtonGroup.transform.GetChild(0).GetComponent<Button>().interactable = false;
                    beforeButtonGroup.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = new Color(0.5f, 0.5f, 0.5f);
                }
                else
                {
                    npcNameText.transform.parent.gameObject.SetActive(true);
                    beforeButtonGroup.transform.GetChild(0).GetComponent<Button>().interactable = true;
                    beforeButtonGroup.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = Color.white;
                }

                if (playerStat.playerAbility.playerQuests[count].isEnd)
                {
                    beforeButtonGroup.SetActive(false);
                    afterButtonGroup.SetActive(true);
                }
                else
                {
                    beforeButtonGroup.SetActive(true);
                    afterButtonGroup.SetActive(false);
                    MaxCheck(playerStat.playerAbility.playerQuests[count]);
                }

                npcNameText.text = selectQuest.npcName.ToString();
                titleNameText.text = selectQuest.titleName;
                logText.text = selectQuest.log;

                for (int i = 0; i < contents.Length; i++)
                    contents[i].SetActive(false);
                ContentsCheck(selectQuest);

                amendsGoldText.text = selectQuest.amendsGold.ToString() + " G";
                amendsExpText.text = selectQuest.amendsExp.ToString() + " exp";

                for (int i = 0; i < amendsItemList.transform.childCount; i++)
                    amendsItemList.transform.GetChild(i).gameObject.SetActive(false);
                AmendsItemCheck(questNumber);

                for (int i = 0; i < selectList.transform.childCount; i++)
                    selectList.transform.GetChild(i).gameObject.SetActive(false);
                SelectItemCheck(questNumber);

                selectItem.selectItemCount = selectQuest.selectitemNumber;
                selectItem.SelectReset();

                if (selectQuest.selectitemNumber >= 1)
                {
                    if(selectQuest.isEnd)
                        selectItem.SelectValue(false);
                    else
                        selectItem.SelectValue(true);
                }
            }
        }
    }

    public void CountCheck(QuestList _questList, int count, bool isInvenTory = false)
    {
        int.TryParse(_questList.currentCount, out int currentCount);
        int.TryParse(_questList.maxCount, out int maxCount);

        if (isInvenTory)
            currentCount = count;
        else
            currentCount += count;

        if (currentCount < maxCount)
        {
            _questList.isMax = false;
            for (int i = 0; i < questNpcs.Count; i++)
            {
                if(questNpcs[i].npcType == _questList.npcName)
                {
                    questNpcs[i].questList[questNpcs[i].questCount].isMax = false;
                    break;
                }
            }
        }
    }

    public void PlusCurrentCount(QuestList _questList, int count, bool isInvenTory = false)
    {
        int.TryParse(_questList.currentCount, out int currentCount);
        int.TryParse(_questList.maxCount, out int maxCount);

        if (isInvenTory)
            currentCount = count;
        else
            currentCount += count;

        _questList.currentCount = currentCount.ToString();
        ContentsCheck(selectQuest);

        UIManager.Instance.questController.ContentsCheck(_questList);

        //Debug.Log("1");

        if (currentCount >= maxCount)
        {
            _questList.isMax = true;

            if (_questList.questNubmer != selectQuest.questNubmer)
                SetToggle(_questList);

            UIManager.Instance.WindowUpdate(3);
            GameController.Instance.NoticeSet(0, _questList.titleName);
            AudioManager.Instance.PlaySound("QuestClear", Camera.main.transform.position);
            if (_questList.npcName == Enums.NpcType.Main)
            {

                //Debug.Log(_questList.questNubmer + " " + selectQuest.questNubmer);

                beforeButtonGroup.transform.GetChild(1).gameObject.SetActive(true);
                beforeButtonGroup.transform.GetChild(0).gameObject.SetActive(false);
                beforeButtonGroup.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {


                beforeButtonGroup.transform.GetChild(1).gameObject.SetActive(false);
                beforeButtonGroup.transform.GetChild(0).gameObject.SetActive(false);
                beforeButtonGroup.transform.GetChild(2).gameObject.SetActive(true);
            }


            Debug.Log("체크");
            UIManager.Instance.questController.ContentsClearCheck(_questList, true);
        }
        else
        {
            _questList.isMax = false;

            if (_questList.questNubmer != selectQuest.questNubmer)
                return;

            beforeButtonGroup.transform.GetChild(0).gameObject.SetActive(true);
            beforeButtonGroup.transform.GetChild(1).gameObject.SetActive(false);
            beforeButtonGroup.transform.GetChild(2).gameObject.SetActive(false);

            UIManager.Instance.questController.ContentsClearCheck(_questList, false);
        }
    }

    public void PlusCurrentCountGroup(QuestList _questList, int count, int seat, bool isInvenTory = false)
    {
        string[] maxCheck = _questList.maxCount.Split(',');
        string[] currentCheck = _questList.currentCount.Split(',');

        int.TryParse(currentCheck[seat], out int currentCount);
        int.TryParse(maxCheck[seat], out int maxCount);

        if (isInvenTory)
            currentCount = count;
        else
            currentCount += count;

        currentCheck[seat] = currentCount.ToString();

        int currentValue = 0;
        int maxValue = 0;

        for (int i = 0; i < currentCheck.Length; i++)
        {
            Debug.Log(currentCheck[i]);
            currentValue += int.Parse(currentCheck[i]);
            maxValue += int.Parse(maxCheck[i]);
        }

        string reValue = "";
        for (int i = 0; i < currentCheck.Length; i++)
        {
            if(i+1 == currentCheck.Length)
                reValue += currentCheck[i];
            else
                reValue += currentCheck[i] + ",";
        }
        _questList.currentCount = reValue;
        ContentsCheck(selectQuest);

        if (currentValue >= maxValue)
        {
            _questList.isMax = true;

            UIManager.Instance.WindowUpdate(3);
            GameController.Instance.NoticeSet(0, _questList.titleName);
            AudioManager.Instance.PlaySound("QuestClear", Camera.main.transform.position);
            AudioManager.Instance.PlaySound("QuestClear", Camera.main.transform.position);

            if (_questList.npcName == Enums.NpcType.Main)
            {
                if (_questList.questNubmer != selectQuest.questNubmer)
                    return;

                beforeButtonGroup.transform.GetChild(1).gameObject.SetActive(true);
                beforeButtonGroup.transform.GetChild(0).gameObject.SetActive(false);
                beforeButtonGroup.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                if (_questList.questNubmer != selectQuest.questNubmer)
                    return;

                beforeButtonGroup.transform.GetChild(1).gameObject.SetActive(false);
                beforeButtonGroup.transform.GetChild(0).gameObject.SetActive(false);
                beforeButtonGroup.transform.GetChild(2).gameObject.SetActive(true);
            }
            UIManager.Instance.questController.ContentsClearCheck(_questList, true);
        }
        else
        {
            _questList.isMax = false;

            if (_questList.questNubmer != selectQuest.questNubmer)
                return;

            beforeButtonGroup.transform.GetChild(0).gameObject.SetActive(true);
            beforeButtonGroup.transform.GetChild(1).gameObject.SetActive(false);
            beforeButtonGroup.transform.GetChild(2).gameObject.SetActive(false);

            UIManager.Instance.questController.ContentsClearCheck(_questList, false);
        }
    }

    void MaxCheck(QuestList _questList)
    {
        if (UIManager.Instance.questController.CountCheck(_questList))
        {
            if (_questList.npcName == Enums.NpcType.Main)
            {
                beforeButtonGroup.transform.GetChild(1).gameObject.SetActive(true);
                beforeButtonGroup.transform.GetChild(0).gameObject.SetActive(false);
                beforeButtonGroup.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                beforeButtonGroup.transform.GetChild(1).gameObject.SetActive(false);
                beforeButtonGroup.transform.GetChild(0).gameObject.SetActive(false);
                beforeButtonGroup.transform.GetChild(2).gameObject.SetActive(true);
            }
        }
        else
        {
            if (_questList.npcName == Enums.NpcType.Main)
            {
                beforeButtonGroup.transform.GetChild(1).gameObject.SetActive(false);
                beforeButtonGroup.transform.GetChild(0).gameObject.SetActive(true);
                beforeButtonGroup.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                beforeButtonGroup.transform.GetChild(1).gameObject.SetActive(false);
                beforeButtonGroup.transform.GetChild(0).gameObject.SetActive(true);
                beforeButtonGroup.transform.GetChild(2).gameObject.SetActive(false);
            }
        }
    }

    public void QuestUIGiveup()
    {
        UIManager.Instance.questController.GiveUpButton(selectQuest);
        NpcLink();
    }

    public void NpcLink()
    {
        for (int i = 0; i < questNpcs.Count; i++)
        {
            if (selectQuest.npcName == questNpcs[i].npcType)
            {
                questNpcs[i].NpcQuestGiveUp();
                break;
            }
        }
    }

    public void QuesetUIReword()
    {
        UIManager.Instance.questController.RewordButton(selectQuest);
        Debug.Log(selectQuest.npcName);
    }

    public void QuestComText(QuestList _questList)
    {
        for (int count = 0; count < playerStat.playerAbility.playerQuests.Count; count++)
        {
            if (playerStat.playerAbility.playerQuests[count].questNubmer == _questList.questNubmer)
            {
                string[] textChild = _questList.order.Split('-');
                int.TryParse(textChild[0], out int lvCount);
                int.TryParse(textChild[1], out int childCount);

                if (_questList.questNubmer > 100)
                    questLines[lvCount].questChildText[childCount].text = (childCount + 1) + ". [메인]" + "<color=#B2FF66>" + _questList.titleName + "</color>";
                else
                    questLines[lvCount].questChildText[childCount].text = (childCount + 1) + ". [서브]" + "<color=#B2FF66>" + _questList.titleName + "</color>";
            }
        }
    }

    public void ContentsCheck(QuestList _questList)
    {
        string[] questConCheck = _questList.content.Split(',');
        string[] maxCheck = _questList.maxCount.Split(',');
        string[] currentCheck = _questList.currentCount.Split(',');

        for (int i = 0; i < questConCheck.Length; i++)
        {
            contents[i].SetActive(true);
            contents[i].transform.GetChild(0).GetComponent<Text>().text = questConCheck[i];
            if(_questList.isEnd)
                contents[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "완료";
            else
                contents[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = currentCheck[i] +"/"+ maxCheck[i];
        }
    }

    public void AmendsItemCheck(int questNumber)
    {
        string[] itemCheck = DBManager.Instance.questList[questNumber].amendsItemCount.Split(',');
        string[] pathCheck = DBManager.Instance.questList[questNumber].itemSpritePath.Split(',');

        if (itemCheck[0].Contains("0"))
        {
            amendsItemList.SetActive(false);
            return;
        }
        else
            amendsItemList.SetActive(true);

        for (int i = 0; i < itemCheck.Length; i++)
        {
            int.TryParse(pathCheck[i], out int itemSpriteNumber);
            int.TryParse(itemCheck[i], out int itemCount);
            amendsItemList.transform.GetChild(i).gameObject.SetActive(true);
            amendsItemList.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(Utill.Instance.SpritePath(itemSpriteNumber));

            if (itemCheck[i].Contains("1"))
            {
                amendsItemList.transform.GetChild(i).transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                amendsItemList.transform.GetChild(i).transform.GetChild(2).gameObject.SetActive(true);
                amendsItemList.transform.GetChild(i).transform.Find("AmendsCountBar/AmendsText").GetComponent<Text>().text = itemCheck[i];
            }
            amendsItemList.transform.GetChild(i).GetComponent<QuestItem>().QuestItemInit(itemCount);
            amendsItemList.transform.GetChild(i).GetComponent<QuestItem>().plusitemCount = itemCount;
        }
    }

    public void SelectItemCheck(int questNumber)
    {
        string[] itemCheck = DBManager.Instance.questList[questNumber].selectItemCount.Split(',');
        string[] pathCheck = DBManager.Instance.questList[questNumber].selectitemPath.Split(',');

        if (itemCheck[0].Contains("0"))
        {
            selectList.transform.parent.gameObject.SetActive(false);
            return;
        }
        else
            selectList.transform.parent.gameObject.SetActive(true);

        for (int i = 0; i < itemCheck.Length; i++)
        {
            int.TryParse(pathCheck[i], out int itemSpriteNumber);
            int.TryParse(itemCheck[i], out int itemCount);
            selectList.transform.GetChild(i).gameObject.SetActive(true);
            selectList.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(Utill.Instance.SpritePath(itemSpriteNumber));
            selectList.transform.parent.GetChild(2).GetComponent<Text>().text = "아이템 " + itemCheck[i].ToString() + "개를 선택해주세요.";
            if (itemCheck[i].Contains("1"))
            {
                selectList.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                selectList.transform.GetChild(i).GetChild(2).gameObject.SetActive(true);
                selectList.transform.GetChild(i).Find("AmendsCountBar/AmendsText").GetComponent<Text>().text = itemCheck[i];
            }
            selectList.transform.GetChild(i).GetComponent<QuestItem>().QuestItemInit(itemCount);
            selectList.transform.GetChild(i).GetComponent<QuestItem>().plusitemCount = itemCount;
        }
    }

    public void QuestRewordItem()
    {
        if (logBox.gameObject.activeSelf)
        {
            for (int i = 0; i < amendsItemList.transform.childCount; i++)
            {
                if (amendsItemList.transform.GetChild(i).gameObject.activeSelf)
                {
                    QuestItem questItem = amendsItemList.transform.GetChild(i).GetComponent<QuestItem>();
                    questItem.QuestItemCom();
                }
            }
        }
    }

    public void QuestSelectItem()
    {
        if (logBox.gameObject.activeSelf)
        {
            for (int i = 0; i < selectList.transform.childCount; i++)
            {
                if (selectList.transform.GetChild(i).gameObject.activeSelf)
                {
                    QuestItem questItem = selectList.transform.GetChild(i).GetComponent<QuestItem>();
                    if (questItem.isSelectCheck)
                    {
                        questItem.QuestItemCom();
                    }
                }
            }
        }
    }
}
