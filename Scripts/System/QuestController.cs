using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class QuestList
{
    public int questNubmer;
    public Enums.NpcType npcName;
    public int possibleLv;
    public string titleName;
    public string log;
    public string content;
    public string getItem;
    public string maxCount;
    public string currentCount;
    public string amendsItemCount;
    public string itemSpritePath;
    public string selectItemCount;
    public string selectitemPath;
    public int selectitemNumber;
    public int amendsGold;
    public int amendsExp;
    public bool isAccept;
    public bool isMax;
    public bool isEnd;
    public string order;

    public QuestList()
    {

    }
    public QuestList(Enums.NpcType _npcType)
    {
        npcName = _npcType;
    }

    public QuestList(int _questNubmer, string _npcName, int _possibleLv, string _titleName, string _log, string _content, string _getItem, string _maxCount, string _currentCount,
        string _amendsItemCount, string _itemSpritePath, string _selectItemCount, string _selectitemPath,int _selectitemNumber, int _amendsGold, int _amendsExp, int _isAccept, int _isMax, int _isEnd, string _order)
    {
        questNubmer = _questNubmer;
        Enum.TryParse(_npcName,out npcName);
        possibleLv = _possibleLv;
        titleName = _titleName;
        log = _log;
        content = _content;
        getItem = _getItem;
        maxCount = _maxCount;
        currentCount = _currentCount;
        amendsItemCount = _amendsItemCount;
        itemSpritePath = _itemSpritePath;
        selectItemCount = _selectItemCount;
        selectitemPath = _selectitemPath;
        selectitemNumber = _selectitemNumber;
        amendsGold = _amendsGold;
        amendsExp = _amendsExp;
        isAccept = Convert.ToBoolean(_isAccept);
        isMax = Convert.ToBoolean(_isMax);
        isEnd = Convert.ToBoolean(_isEnd);
        order = _order;
    }
}

public class QuestController : MonoBehaviour
{
    public SelectItem selectItem;
    [SerializeField]
    QuestNpc questNpc;

    [Header("퀘스트 내용")]
    [SerializeField]
    Text titleNameText;
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
    GameObject aftorButtonGroup;
    [SerializeField]
    ScrollViewContentText dialogBox;
    [SerializeField]
    ScrollViewContentText logBox;

    public void OnEnable()
    {
        StartCoroutine(LogInit());
    }

    public void SetQuest(QuestNpc _questNpc)
    {
        ButtonOnOff(false);
        questNpc = _questNpc;
        QuestList playerQuest = new QuestList();

        if (!GetComponent<NpcUI>().QuestButtonFasle(questNpc))
            return;

        for (int i = 0; i < questNpc.playerStat.playerAbility.playerQuests.Count; i++)
        {
            if (questNpc.playerStat.playerAbility.playerQuests[i].questNubmer == questNpc.questList[questNpc.questCount].questNubmer)
            {
                if (questNpc.playerStat.playerAbility.playerQuests[i].isEnd)
                {
                    questNpc.questCount++;
                }
                else
                {
                    playerQuest = questNpc.playerStat.playerAbility.playerQuests[i];
                    ButtonOnOff(true);
                }
            }
            if (questNpc.questList.Count <= questNpc.questCount)
                return;
        }

        titleNameText.text = questNpc.questList[questNpc.questCount].titleName;
        logText.text = questNpc.questList[questNpc.questCount].log;

        for (int i = 0; i < contents.Length; i++)
            contents[i].SetActive(false);
        ContentsCheck(playerQuest);

        amendsGoldText.text = questNpc.questList[questNpc.questCount].amendsGold.ToString() + " G";
        amendsExpText.text = questNpc.questList[questNpc.questCount].amendsExp.ToString() + " exp";

        for (int i = 0; i < amendsItemList.transform.childCount; i++)
            amendsItemList.transform.GetChild(i).gameObject.SetActive(false);
        AmendsItemCheck(questNpc.questList[questNpc.questCount].questNubmer);


        for (int i = 0; i < selectList.transform.childCount; i++)
            selectList.transform.GetChild(i).gameObject.SetActive(false);
        SelectItemCheck(questNpc.questList[questNpc.questCount]);

        selectItem.selectItemCount = questNpc.questList[questNpc.questCount].selectitemNumber;
        selectItem.SelectReset();

        dialogBox.SetContents();

        Button[] button = aftorButtonGroup.GetComponentsInChildren<Button>();

        bool isFull = CountCheck(playerQuest.questNubmer == 0 ? questNpc.questList[questNpc.questCount] : playerQuest); ;

        aftorButtonGroup.transform.GetChild(1).GetComponent<Button>().onClick?.RemoveAllListeners();
        aftorButtonGroup.transform.GetChild(1).GetComponent<Button>().onClick?.AddListener(() => GiveUpButton(questNpc.questList[questNpc.questCount]));
        
        ContentsClearCheck(playerQuest, isFull);
    }

    public void LogBoxSet()
    {
        logBox.SetContents();
    }

    public void SelectItemInit(QuestList playerQuest)
    {
        if (questNpc == null)
            return;

        QuestList questList = new QuestList();

        if (CountCheck(questNpc.questList[questNpc.questCount]))
        {
            Debug.Log(questList.questNubmer);
            questList = playerQuest.questNubmer == 0 ? questNpc.questList[questNpc.questCount] : playerQuest;
            aftorButtonGroup.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            aftorButtonGroup.transform.GetChild(0).GetComponent<Button>().onClick?.AddListener(() => RewordButton(questList));
            selectItem.SelectValue(true);
        }
        else
        {
            aftorButtonGroup.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            selectItem.SelectValue(false);
        }
    }


    public bool CountCheck(QuestList _questList)
    {

        string[] questConCheck = _questList.content.Split(',');
        string[] maxCheck = _questList.maxCount.Split(',');
        string[] currentCheck = _questList.currentCount.Split(',');

        bool[] sameCount = new bool[questConCheck.Length];
        int maxEndCount = questConCheck.Length;
        int currentEndCount = 0;
        bool endValue = false;
        for (int i = 0; i < questConCheck.Length; i++)
        {
            int.TryParse(currentCheck[i], out int currentCount);
            int.TryParse(maxCheck[i], out int maxCount);

            if (currentCount >= maxCount)
                sameCount[i] = true;
            else
                sameCount[i] = false;
        }

        for (int i = 0; i < sameCount.Length; i++)
        {
            if (sameCount[i])
                currentEndCount++;
        }

        if (currentEndCount >= maxEndCount)
            endValue = true;

        return endValue;
    }

    public void ContentsCheck(QuestList playerQuest)
    {
        if (questNpc == null)
            return;

        if (questNpc.questCount >= questNpc.questList.Count)
            return;

        if (playerQuest.questNubmer != questNpc.questList[questNpc.questCount].questNubmer)
        {
            playerQuest = questNpc.questList[questNpc.questCount];
        }

        string[] questConCheck = playerQuest.content.Split(','); 
        string[] maxCheck = playerQuest.maxCount.Split(',');
        string[] currentCheck = playerQuest.currentCount.Split(',');

        for (int i = 0; i < questConCheck.Length; i++)
        {
            contents[i].SetActive(true);
            contents[i].transform.GetChild(0).GetComponent<Text>().text = questConCheck[i];
            contents[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = currentCheck[i] + "/" + maxCheck[i];
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

    public void SelectItemCheck(QuestList playerQuest)
    {
        string[] itemCheck = playerQuest.selectItemCount.Split(',');
        string[] pathCheck = playerQuest.selectitemPath.Split(',');

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

        //SelectItemInit(playerQuest);
    }

    IEnumerator LogInit()
    {
        logText.gameObject.SetActive(false);
        yield return null;
        logText.gameObject.SetActive(true);
    }

    public void AcceptButton()
    {
        AudioManager.Instance.PlaySound("QuestAccept", Camera.main.transform.position);
        questNpc.questList[questNpc.questCount].isAccept = true;
        UIManager.Instance.playerStat.playerAbility.playerQuests.Add(questNpc.questList[questNpc.questCount]);
        UIManager.Instance.QuestArrReset(questNpc.questList[questNpc.questCount]);
        ButtonOnOff(true);
    }

    public void FefuseButton()
    {
        UIManager.Instance.npcUI.QuestButton();
    }

    public void GiveUpButton(QuestList selectQuest)
    {
        AudioManager.Instance.PlaySound("QuestGiveUp", Camera.main.transform.position);
        selectQuest.currentCount = "0";
        selectQuest.isAccept = false;
        selectQuest.isMax = false;
        ContentsCheck(selectQuest);
        for (int i = 0; i < UIManager.Instance.playerStat.playerAbility.playerQuests.Count; i++)
        {
            if(UIManager.Instance.playerStat.playerAbility.playerQuests[i].questNubmer == selectQuest.questNubmer)
                UIManager.Instance.playerStat.playerAbility.playerQuests.RemoveAt(i);
        }
        UIManager.Instance.QuestArrReset(selectQuest, false);
        ButtonOnOff(false);
    }

    public void ContentsClearCheck(QuestList _questList, bool isActive)
    {
        if (questNpc == null)
            return;

        if (!GetComponent<NpcUI>().QuestButtonFasle(questNpc))
            return;

        if (isActive)
        {
            if (_questList.questNubmer == questNpc.questList[questNpc.questCount].questNubmer)
            {
                if (_questList.selectitemNumber >= 1)
                    selectItem.SelectValue(true);
                aftorButtonGroup.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
                aftorButtonGroup.transform.GetChild(0).GetComponent<Button>().onClick?.AddListener(() => RewordButton(_questList));
                aftorButtonGroup.transform.GetChild(0).gameObject.SetActive(true);
                aftorButtonGroup.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                if (_questList.selectitemNumber >= 1)
                    selectItem.SelectValue(false);
                aftorButtonGroup.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
                aftorButtonGroup.transform.GetChild(0).gameObject.SetActive(false);
                aftorButtonGroup.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        else
        {
            if (_questList.questNubmer == questNpc.questList[questNpc.questCount].questNubmer)
            {
                if (_questList.selectitemNumber >= 1)
                    selectItem.SelectValue(false);
                aftorButtonGroup.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
                aftorButtonGroup.transform.GetChild(0).gameObject.SetActive(false);
                aftorButtonGroup.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                if (_questList.selectitemNumber >= 1)
                    selectItem.SelectValue(false);
                aftorButtonGroup.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
                aftorButtonGroup.transform.GetChild(0).gameObject.SetActive(false);
                aftorButtonGroup.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    public void RewordButton(QuestList selectQuest)
    {
        if(transform.GetChild(0).gameObject.activeSelf)
        {
            if (selectItem.SelectCheck() && selectQuest.selectitemNumber >= 1)
            {
                for (int i = 0; i < selectList.transform.childCount; i++)
                {
                    if (selectList.transform.GetChild(i).gameObject.activeSelf)
                    {
                        QuestItem questItem = selectList.transform.GetChild(i).GetComponent<QuestItem>();
                        if (questItem.isSelectCheck)
                            questItem.QuestItemCom();
                    }
                }
            }
            else if (!selectItem.SelectCheck() && selectQuest.selectitemNumber >= 1)
                return;
        }
        else
        {
            if (UIManager.Instance.questWindow.selectItem.SelectCheck())
            {
                UIManager.Instance.questWindow.QuestSelectItem();
            }
        }

        if (selectQuest.getItem != "0")
        {
            string[] getitems = selectQuest.getItem.Split(',');
            string[] itemCount = selectQuest.maxCount.Split(',');
            for (int i = 0; i < getitems.Length; i++)
            {
                Inventory.Instance.QuestItemRemove(int.Parse(getitems[i]), int.Parse(itemCount[i]));
            }
        }

        AudioManager.Instance.PlaySound("QuestClear2", Camera.main.transform.position);
        PlayerHealth ph = UIManager.Instance.playerStat.GetComponent<PlayerHealth>();
        ph.GiveCoin(selectQuest.amendsGold);
        ph.TakeExp(selectQuest.amendsExp);

        QuestWindow.Instance.QuestComText(selectQuest);

        if (selectQuest.npcName == Enums.NpcType.Main)
        {
            selectQuest.isEnd = true;
            UIManager.Instance.questWindow.QuestRewordItem();
            int newQuest = selectQuest.questNubmer + 1;
            UIManager.Instance.questWindow.MainQuestAdd(newQuest);
            UIManager.Instance.QuestUIReset(newQuest);
        }
        else
        {
            FefuseButton();
            selectQuest.isEnd = true;
            QuestRewordItem();
            questNpc.questList[questNpc.questCount].isEnd = true;
            questNpc.QusetClear();
            UIManager.Instance.QuestUIReset(selectQuest.questNubmer);
            UIManager.Instance.npcUI.LimitQuest();
            ContentsClearCheck(selectQuest, false);
            ButtonOnOff(false);
        }
    }

    void QuestRewordItem()
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
        else
        {
            UIManager.Instance.questWindow.QuestRewordItem();
        }
    }

    void ButtonOnOff(bool active)
    {
        aftorButtonGroup.SetActive(active);
        beforeButtonGroup.SetActive(!active);
    }
}
