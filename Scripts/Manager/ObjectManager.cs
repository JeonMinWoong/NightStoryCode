using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectManager : Singleton<ObjectManager>
{
    Enums.MapStory map = Enums.MapStory.None;

    [SerializeField]
    List<GameObject> willGroup = new List<GameObject>();

    List<GameObject> monsterGroup = new List<GameObject>();
    [SerializeField]
    GameObject bossOJ;
    [SerializeField]
    Transform spawnPosGroup;
    [SerializeField]
    public Transform willGroupPos { get; set; }
    [SerializeField] 
    Transform monsterGroupPos;
    [SerializeField]
    public Transform bossPos { get; set; }

    [SerializeField]
    List<GameObject> objectPool = new List<GameObject>();

    int willMaxObject = 0;
    int monsterMaxObject = 0;

    public string sceneNick;

    [SerializeField]
    public int[] mapCount = new int[7];

    public void Start()
    {
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            mapCount[i] = transform.GetChild(i).childCount;
        }
    }

    public void StageInit()
    {
        string _sceneName = GameManager.Instance.sceneName;

        if (_sceneName.Contains("Map_1"))
        {
            map = Enums.MapStory.Map_1;
            sceneNick = "방치된 유적지";
        }
        else if (_sceneName.Contains("Map_2"))
        {
            map = Enums.MapStory.Map_2;
            sceneNick = "고요한 숲";
        }
        else
        {
            map = Enums.MapStory.None;
        }

        int gCstageCount = 0;

        if (GameController.Instance == null)
            return;
        else
            gCstageCount = GameController.Instance.StageCount;


        switch (map)
        {
            case Enums.MapStory.None:
                break;
            case Enums.MapStory.Map_1:

                int mapCount = GameController.Instance.lvCount;
                int stageCount = GameController.Instance.StageCount;

                willGroup = new List<GameObject>() { Resources.Load<GameObject>("Prefabs/Monster/Will") };
                //monsterGroup = new List<GameObject>() { Resources.Load<GameObject>("Prefabs/Monster/Goblin") };
                monsterGroup = new List<GameObject>() { Resources.Load<GameObject>("Prefabs/Monster/NewGoblin") };
                if (gCstageCount >= 4)
                {
                    monsterGroup.Add(Resources.Load<GameObject>("Prefabs/Monster/Worf"));
                }
                else
                {
                    if(monsterGroup.Contains(Resources.Load<GameObject>("Prefabs/Monster/Worf")))
                        monsterGroup.Remove(Resources.Load<GameObject>("Prefabs/Monster/Worf"));
                }

                if (gCstageCount == 5)
                    return;


                //Debug.Log(gCstageCount);
                if (gCstageCount == 7)
                {
                    bossOJ = Resources.Load<GameObject>("Prefabs/Monster/Boss/GoblinBoss");
                    bossPos = transform.Find(mapCount + "/SpawnPos_" + mapCount + "_Boss").GetChild(0);
                    SpawnBoss();
                    return;
                }
                else if (gCstageCount == 8)
                    return;
                else
                {
                    spawnPosGroup = transform.Find(mapCount + "/SpawnPos_" + mapCount + "-" + stageCount);
                    willGroupPos = spawnPosGroup.transform.GetChild(0);
                    monsterGroupPos = spawnPosGroup.transform.GetChild(1);
                }

                willMaxObject = willGroupPos.transform.childCount;
                monsterMaxObject = monsterGroupPos.transform.childCount;
                break;
            case Enums.MapStory.Map_2:
                break;
        }

        if (map != Enums.MapStory.None && GameController.Instance.stageGroup[gCstageCount - 1].Find("Monster") == null)
        {
            objectPool.Clear();
            SpawnObject();
        }
        else if(map != Enums.MapStory.None && GameController.Instance.stageGroup[gCstageCount - 1].Find("Monster") != null)
        {
            GameObject monsterGroup = GameController.Instance.stageGroup[gCstageCount - 1].Find("Monster").gameObject;
            objectPool.Clear();

            for (int i = 0; i < monsterGroup.transform.childCount; i++)
            {
                objectPool.Add(monsterGroup.transform.GetChild(i).gameObject);
            }

            ResetObject();
        }
    }

    void SpawnObject()
    {
        GameObject _monster = new GameObject("Monster");
        _monster.transform.parent = GameController.Instance.stageGroup[GameController.Instance.StageCount - 1].transform;
        GameObject _will = new GameObject("WillGroup");
        _will.transform.parent = GameController.Instance.stageGroup[GameController.Instance.StageCount - 1].transform;

        for (int i = 0; i < monsterMaxObject; i++)
        {
            int rand = Random.Range(0, monsterGroup.Count);

            GameObject go = Instantiate(monsterGroup[rand]);
            objectPool.Add(go);
            go.transform.position = monsterGroupPos.transform.GetChild(i).transform.position;
            go.name = monsterGroup[rand].name;
            go.transform.parent = _monster.transform;
        }

        if ((int)map < GameController.Instance.playerStat.MapInpo)
        {
            WillClearSet();
            return;
        }
        else
        {
            if (SetStageWill())
                return;
        }

        for (int i = 0; i < willMaxObject; i++)
        {
            int rand = Random.Range(0, willGroup.Count);

            GameObject go = Instantiate(willGroup[rand]);
            go.transform.position = willGroupPos.transform.GetChild(i).transform.position;
            go.name = willGroup[rand].name;
            go.transform.parent = _will.transform;
        }
    }
    
    bool SetStageWill()
    {
        int mapLv = GameController.Instance.lvCount - 1;
        int stageCount = GameController.Instance.StageCount - 1;
        if (GameController.Instance.playerStat.playerAbility.stageInpo[mapLv].stageInpo[stageCount])
        {
            GameController.Instance.stageClear[stageCount].isStageClear = true;
            GameController.Instance.stageClear[stageCount].isNoWill = true;
            return true;
        }
        else
            return false;
    }

    public void WillClearSet()
    {
        for (int i = 0; i < GameController.Instance.stageClear.Length; i++)
        {
            GameController.Instance.stageClear[i].isStageClear = true;
            GameController.Instance.stageClear[i].isNoWill = true;
        }
    }

    public void ResetObject()
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            objectPool[i].transform.position = monsterGroupPos.transform.GetChild(i).transform.position;
            objectPool[i].GetComponent<Stat>().CurrentHealth = objectPool[i].GetComponent<Stat>().MaxHealth;
            objectPool[i].GetComponent<EnemyHealth>().isDeath = false;
            objectPool[i].SetActive(true);
        }
    }

    public void SpawnBoss()
    {
        GameObject _boss = new GameObject("Boss");

        if (GameController.Instance.boss != null)
            return;

        GameObject go = Instantiate(bossOJ, bossPos.position,Quaternion.identity,_boss.transform);
        go.name = bossOJ.name;
        GameController.Instance.boss = go;
        go.SetActive(false);
    }

}
