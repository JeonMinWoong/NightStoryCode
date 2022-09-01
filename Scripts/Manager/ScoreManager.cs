using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : Singleton<ScoreManager>
{
    public PlayerStat playerStat;

    [SerializeField]
    GameObject playerPanels;

    [SerializeField]
    GameObject statWindow;
    [SerializeField]
    GameObject InpoUI;

    // General
    Text level;
    Text life;
    Text health;
    Text healthRegen;
    Text stamina;
    Text staminaRegen;

    // Damage
    Text attack;
    Text attackSpeed;
    Text criticalChance;
    Text criticalDamage;

    // Armor
    Text armor;
    Text evasion;
    Text luck;

    [SerializeField]
    List<Text> titleGroup = new List<Text>();

    // Coin
    Text coinText;

    TextMeshProUGUI count;
    [SerializeField]
    private int[] scoreGroup;
    public int ScoreGroup
    {
        get { return scoreGroup[GameController.Instance.StageCount]; }
        set { scoreGroup[GameController.Instance.StageCount] = value; }
    }

    public void Init()
    {
        playerStat = GameController.Instance.player.GetComponent<PlayerStat>();
        playerPanels = GameObject.Find("Panels");
        statWindow = playerPanels.transform.Find("Equipment").gameObject;
        InpoUI = GameObject.Find("InpoUI");

        Transform stageParent = statWindow.transform;

        scoreGroup = new int[stageParent.childCount];

        StatsInit();
        LanguageSetting();
    }

    void StatsInit()
    {
        level = statWindow.GetComponent<StatTextGroup>().generalGroup.transform.Find("LV").GetChild(1).GetComponent<Text>();
        life = statWindow.GetComponent<StatTextGroup>().generalGroup.transform.Find("Life").GetChild(1).GetComponent<Text>();
        health = statWindow.GetComponent<StatTextGroup>().generalGroup.transform.Find("HP").GetChild(1).GetComponent<Text>();
        healthRegen = statWindow.GetComponent<StatTextGroup>().generalGroup.transform.Find("HPreg").GetChild(1).GetComponent<Text>();
        stamina = statWindow.GetComponent<StatTextGroup>().generalGroup.transform.Find("SP").GetChild(1).GetComponent<Text>();
        staminaRegen = statWindow.GetComponent<StatTextGroup>().generalGroup.transform.Find("SPreg").GetChild(1).GetComponent<Text>();

        attack = statWindow.GetComponent<StatTextGroup>().damageGroup.transform.Find("SwordDamage").GetChild(1).GetComponent<Text>();
        attackSpeed = statWindow.GetComponent<StatTextGroup>().damageGroup.transform.Find("AttackSpeed").GetChild(1).GetComponent<Text>();
        criticalChance = statWindow.GetComponent<StatTextGroup>().damageGroup.transform.Find("CriticalHit").GetChild(1).GetComponent<Text>();
        criticalDamage = statWindow.GetComponent<StatTextGroup>().damageGroup.transform.Find("CriticalDamage").GetChild(1).GetComponent<Text>();

        armor = statWindow.GetComponent<StatTextGroup>().armorGroup.transform.Find("Armor").GetChild(1).GetComponent<Text>();
        evasion = statWindow.GetComponent<StatTextGroup>().armorGroup.transform.Find("Evasion").GetChild(1).GetComponent<Text>();
        luck = statWindow.GetComponent<StatTextGroup>().armorGroup.transform.Find("Luck").GetChild(1).GetComponent<Text>();

        coinText = playerPanels.transform.GetChild(1).Find("Gold").GetComponentInChildren<Text>();
        count = InpoUI.transform.GetComponentInChildren<TextMeshProUGUI>();
    }

    void LanguageSetting()
    {
        Text[] allText = statWindow.GetComponentsInChildren<Text>();
        foreach(Text title in allText)
        {
            if(title.name.Equals("Title"))
            {
                titleGroup.Add(title);
            }
        }

        if (SettingManager.Instance.languageSetting == SettingManager.Language.Korea)
        {
            titleGroup[0].text = "기본 관련";
            titleGroup[1].text = "레벨";
            titleGroup[2].text = "생명력";
            titleGroup[3].text = "체력";
            titleGroup[4].text = "체력 회복률";
            titleGroup[5].text = "내구력";
            titleGroup[6].text = "내구력 회복률";
            titleGroup[7].text = "공격 관련";
            titleGroup[8].text = "공격력";
            titleGroup[9].text = "공격 속도";
            titleGroup[10].text = "치명률";
            titleGroup[11].text = "치명타 피해량";
            titleGroup[12].text = "방어 관련";
            titleGroup[13].text = "방어력";
            titleGroup[14].text = "회피율";
            titleGroup[15].text = "운";
        }
        else if(SettingManager.Instance.languageSetting == SettingManager.Language.English)
        {
            titleGroup[0].text = "GENERAL";
            titleGroup[1].text = "Level";
            titleGroup[2].text = "Life";
            titleGroup[3].text = "Maximum Health";
            titleGroup[4].text = "Regeneration";
            titleGroup[5].text = "Maximum Stamina";
            titleGroup[6].text = "ST Regeneration";
            titleGroup[7].text = "DAMAGE";
            titleGroup[8].text = "Sword Damage";
            titleGroup[9].text = "Attack Speed";
            titleGroup[10].text = "Critical Hit Chance";
            titleGroup[11].text = "Critical Damage";
            titleGroup[12].text = "ARMOR";
            titleGroup[13].text = "Armor";
            titleGroup[14].text = "Evasion";
            titleGroup[15].text = "Luck";
        }
    }

    private void FixedUpdate()
    {
        if(playerStat != null)
            StatsUpdate();
    }

    void StatsUpdate()
    {
        level.text = playerStat.Level.ToString();
        life.text = playerStat.CurrentLife + "/" + playerStat.MaxLife;
        health.text = playerStat.CurrentHealth.ToString("0.0") + "/" + playerStat.MaxHealth;
        healthRegen.text = playerStat.ReGen.ToString("0.0") + " per sec";
        stamina.text = playerStat.CurrentStamina.ToString("0.0") + "/" + playerStat.MaxStamina;
        staminaRegen.text = playerStat.StaminaReGen.ToString("0.0") + " per sec";

        attack.text = playerStat.Attack.ToString();
        attackSpeed.text = playerStat.AttackSpeed.ToString("0.0");
        criticalChance.text = (playerStat.CriticalPercent).ToString() + "%";
        criticalDamage.text = (100 + playerStat.CriticalDamage).ToString() + "%";

        armor.text = playerStat.Defense.ToString();
        evasion.text = playerStat.Evasion.ToString("0.0") + "%";
        luck.text = playerStat.Luck.ToString();

        coinText.text = (playerStat.CoinGet.ToString() + "G");
        if (GameController.Instance.stageClear[GameController.Instance.StageCount - 1].isNoWill == false)
        {
            count.gameObject.SetActive(true);
            count.text = (ScoreGroup.ToString() + " / " + (3 + GameController.Instance.lvCount * 2));
        }
        else
            count.gameObject.SetActive(false);
    }
}
