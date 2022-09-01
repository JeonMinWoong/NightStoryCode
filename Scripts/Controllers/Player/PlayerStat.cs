using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class PlayerAbility : Ability
{
    public float reGen;
    public int maxStamina;
    public float currentStamina;
    public float staminaReGen;
    public int maxSkillGage;
    public float currentSkillGage;
    public int maxLife;
    public int currentLife;
    public int coinGet;
    public int maxExp;
    public int currentExp;
    public int maxStage;
    public Vector2 pos;
    public int skillLevel;
    public float criticalPercent;
    public float criticalDamage;
    public float evasion;
    public int luck;
    public int skillPoint;
    public int mapInpo;
    public List<StageInpo> stageInpo = new List<StageInpo>(new StageInpo[System.Enum.GetValues(typeof(Enums.MapStory)).Length - 1]);
    public string currentSaveLvStage;
    public int cinemaCount;
    public bool isDeathEvent;

    public List<QuestList> playerQuests = new List<QuestList>();

    public string date;
    public float playTime;
    public string saveName;
    public int saveCount;

    public List<ItemInpo> ownItem = new List<ItemInpo>();
    public List<ItemInpo> equipItem = new List<ItemInpo>();
    public Skill[] skillInpo = new Skill[4];
    public StoryProgress[] storyProgressGroup = new StoryProgress[System.Enum.GetValues(typeof(Enums.NpcType)).Length];
}

[System.Serializable]
public class StageInpo
{
    public bool[] stageInpo;
}

[System.Serializable]
public class Skill
{
    public List<SkillStat> skillStats = new List<SkillStat>();
}

public class PlayerStat : MonoBehaviour
{
    const int defultMaxHealth = 100;
    const int defultMaxSp = 75;
    const float defultReGen = 0.5f;
    const float defultStaminaReGen = 1f;

    const int defultAttack = 10;
    const float defultAttackSpeed = 1.5f;
    const float defultAttackAni = 1;
    const int defultDefense = 0;
    const float defultCritical = 1;

    public StatTextGroup statTextGroup;
    public SkillList skillList;

    [SerializeField]
    Slider healthSlider;     //체력 표시 슬라이더
    [SerializeField]
    Slider staminaSlider;    //스테미나 표시 슬라이더
    [SerializeField]
    Slider skillSlider;
    [SerializeField]
    Slider expSlider;
    [SerializeField]
    Slider lifeSlider;
    Text lvText;

    public Enums.SkillGage skillGage = Enums.SkillGage.None;

    public PlayerHealth playerH;

    public PlayerAbility playerAbility;

    public SkillStat[] equipSkills = new SkillStat[7];

    public bool isLoadData;

    public byte trapResistance;
    public byte armorLgnore;
    public float poisonDamage;
    public byte goldenRule;
    public byte soul;
    public byte potionPlus;
    public byte staminaPlus;
    public byte counter;
    public byte blessing;
    public byte dashTime;

    public float CurrentHealth
    {
        get
        {
            if (playerAbility.currentHealth > playerAbility.maxHealth)
                playerAbility.currentHealth = playerAbility.maxHealth;

            return playerAbility.currentHealth;
        }
        set
        {
            playerAbility.currentHealth = value;

            //값 보정
            if (playerAbility.currentHealth < 0)
                playerAbility.currentHealth = 0;
            else if (playerAbility.currentHealth > playerAbility.maxHealth)
                playerAbility.currentHealth = playerAbility.maxHealth;
        }
    }
    public float CurrentStamina
    {
        get
        {
            if (playerAbility.currentStamina > playerAbility.maxStamina)
                playerAbility.currentStamina = playerAbility.maxStamina;

            return playerAbility.currentStamina; 
        }
        set
        {
            playerAbility.currentStamina = value;

            //값 보정
            if (playerAbility.currentStamina < 0)
                playerAbility.currentStamina = 0;
            else if (playerAbility.currentStamina > playerAbility.maxStamina)
                playerAbility.currentStamina = playerAbility.maxStamina;
        }
    }
    public float CurrentSkillGage
    {
        get
        {
            return playerAbility.currentSkillGage;
        }
        set
        {
            playerAbility.currentSkillGage = value;

            if (playerAbility.currentSkillGage >= playerAbility.maxSkillGage)
            {
                playerAbility.currentSkillGage = playerAbility.maxSkillGage;
                if (skillGage != Enums.SkillGage.MaxIng)
                    skillGage = Enums.SkillGage.Max;

                if (skillGage == Enums.SkillGage.Max)
                    playerH.MaxSkill();
            }
        }
    }
    public int CurrentExp
    {
        get
        {
            return playerAbility.currentExp;
        }
        set
        {
            playerAbility.currentExp = value;
        }
    }

    public int CurrentLife
    {
        get
        {
            return playerAbility.currentLife;
        }
        set
        {
            playerAbility.currentLife = value;

            if (playerAbility.currentLife < 0)
                playerAbility.currentLife = 0;
            else if (playerAbility.currentLife > playerAbility.maxLife)
                playerAbility.currentLife = playerAbility.maxLife;
        }
    }
    public int Level { get { return playerAbility.level; } set { playerAbility.level = value; } }
    public string ObName { get { return playerAbility.obName; } set { playerAbility.obName = value; } }

    public int MaxHealth { get { return playerAbility.maxHealth; } set { playerAbility.maxHealth = value; } }
    public int Attack { get { return playerAbility.attack; } set { playerAbility.attack = value; } }
    public float MoveSpeed { get { return playerAbility.moveSpeed; } set { playerAbility.moveSpeed = value; } }
    public float Defense { get { return playerAbility.defense; } set { playerAbility.defense = value; } }
    public float AttackSpeed { get { return playerAbility.attackSpeed; } set { playerAbility.attackSpeed = value; } }
    public float ReGen { get { return playerAbility.reGen; } set { playerAbility.reGen = value; } }
    public int MaxStamina { get { return playerAbility.maxStamina; } set { playerAbility.maxStamina = value; } }
    public float StaminaReGen { get { return playerAbility.staminaReGen; } set { playerAbility.staminaReGen = value; } }
    public int MaxSkillGage { get { return playerAbility.maxSkillGage; } set { playerAbility.maxSkillGage = value; } }
    public int MaxLife { get { return playerAbility.maxLife; } set { playerAbility.maxLife = value; } }
    public int CoinGet { get { return playerAbility.coinGet; } set { playerAbility.coinGet = value; } }
    public int MaxExp { get { return playerAbility.maxExp; } set { playerAbility.maxExp = value; } }
    public int ExpGet { get { return playerAbility.currentExp; } set { playerAbility.currentExp = value; } }
    public int MaxStage { get { return playerAbility.maxStage; } set { playerAbility.maxStage = value; } }
    public Vector2 Pos { get { return playerAbility.pos; } set { playerAbility.pos = value; } }
    public int SkillLevel { get { return playerAbility.skillLevel; } set { playerAbility.skillLevel = value; } }
    public float CriticalPercent { get { return playerAbility.criticalPercent; } set { playerAbility.criticalPercent = value; } }
    public float CriticalDamage { get { return playerAbility.criticalDamage; } set { playerAbility.criticalDamage = value; } }
    public float Evasion { get { return playerAbility.evasion; } set { playerAbility.evasion = value; } }
    public int Luck { get { return playerAbility.luck; } set { playerAbility.luck = value; } }
    public int SkillPoint { get { return playerAbility.skillPoint; } set { playerAbility.skillPoint = value; } }
    public int MapInpo { get { return playerAbility.mapInpo; } set { playerAbility.mapInpo = value; } }
    public string CurrentSaveLvStage { get { return playerAbility.currentSaveLvStage; } set { playerAbility.currentSaveLvStage = value; } }

    private void OnEnable()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Contains("MapSelect"))
        {
            DataManager.Instance.isLoad = true;
            DataApply();
        }
    }

    public void Init()
    {
        playerH = GetComponent<PlayerHealth>();

        if (GameObject.Find("Hp_ampule") != null)
        { 
            healthSlider = GameObject.Find("Hp_ampule").GetComponent<Slider>();
            staminaSlider = GameObject.Find("ST_ampule").GetComponent<Slider>();
            skillSlider = GameObject.Find("SP_ampule").GetComponent<Slider>();
            lifeSlider = GameObject.Find("Life_ampule").GetComponent<Slider>();
            expSlider = GameObject.Find("Exp_ampule").GetComponent<Slider>();
            lvText = GameObject.Find("LevelText").GetComponent<Text>();
            Inventory.Instance.playerStat = this;
            Inventory.Instance.playerHealth = gameObject.GetComponent<PlayerHealth>();
        }
        SetPlayerStatus();

        DataApply();
    }

    private void Update()
    {
        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, CurrentHealth / MaxHealth, Time.deltaTime * 10f);
            staminaSlider.value = Mathf.Lerp(staminaSlider.value, CurrentStamina /MaxStamina, Time.deltaTime * 10f);
            skillSlider.value = Mathf.Lerp(skillSlider.value, CurrentSkillGage / MaxSkillGage, Time.deltaTime * 10f);
            lifeSlider.value = Mathf.Lerp(lifeSlider.value, (float)CurrentLife / MaxLife, Time.deltaTime * 2.5f);
            lvText.text = playerAbility.level.ToString();
            expSlider.value = Mathf.Lerp(expSlider.value, (float)CurrentExp / MaxExp,Time.deltaTime * 5f);
        }

        playerAbility.playTime += Time.deltaTime;
    }

    public void PlayerStatSet(int _lv, string _name, int _hp, int _maxHp, int _attack, int _defense,
       float _speed, float _regen, int _maxStamina, int _currentStamina, float _staminaReGen, int _coinGet, int _maxExp , int _expGet, int _maxStage, int _currentStage,
       Vector2 _pos , int _skillCount , float _criticalPercent, int _maxLife,int _currentLife)
    {
        playerAbility.level = _lv;
        playerAbility.obName = _name;
        CurrentHealth = _hp;
        playerAbility.maxHealth = _maxHp;
        playerAbility.attack = _attack;
        playerAbility.defense = _defense;
        playerAbility.attackSpeed = _speed;
        ReGen = _regen;
        MaxStamina = _maxStamina;
        CurrentStamina = _currentStage;
        StaminaReGen = _staminaReGen;
        CoinGet = _coinGet;
        MaxExp = _maxExp;
        ExpGet = _expGet;
        MaxStage = _maxStage;
        Pos = _pos;
        SkillLevel = _skillCount;
        CriticalPercent = _criticalPercent;
        MaxLife = _maxLife;
        CurrentLife = _currentLife;
    }

    public void PlayerSkillUpdate()
    {

    }

    public void SetPlayerStatus()
    {
        MaxExp =                    100 + (playerAbility.level - 1 ) * 50 + (playerAbility.level - 1 ) * 25;
        playerAbility.maxHealth =                 defultMaxHealth + (playerAbility.level -1) * 5 + (playerAbility.level - 1);
        CurrentHealth =             playerAbility.maxHealth;
        MaxStamina =                defultMaxSp + (playerAbility.level -1) * 3 + staminaPlus;
        CurrentStamina =            MaxStamina;
        MaxSkillGage =              100;
        ReGen =                     defultReGen + (playerAbility.level - 1) * 0.1f;
        StaminaReGen =              defultStaminaReGen + (playerAbility.level - 1) * 0.2f + staminaPlus * 0.05f;
        playerAbility.defense =                   defultDefense + (playerAbility.level - 1) * 0.5f;
        playerAbility.attack =                    defultAttack + (playerAbility.level - 1) * 1;
        playerAbility.attackSpeed =               defultAttackSpeed - (playerAbility.level - 1) * 0.03f;
        SetAttackAnimSpeed();
        CriticalPercent =           defultCritical + (playerAbility.level - 1) * 0.2f;
        statTextGroup.EqiupLevelUp();
        skillList.ResetSkill();
    }

    public void SetAttackAnimSpeed(float plusSpeed = 0)
    {
        float aniSpeed = defultAttackAni +  (playerAbility.level - 1) * 0.03f + plusSpeed;
        GetComponent<Animator>().SetFloat("AttackSpeed", aniSpeed);
    }

    public void PlusStamina()
    {
        MaxStamina = defultMaxSp + (playerAbility.level - 1) * 3 + staminaPlus;
        StaminaReGen = defultStaminaReGen + (playerAbility.level - 1) * 0.1f + staminaPlus * 0.05f;
    }

    void StageSet()
    {
        playerAbility.stageInpo[0].stageInpo = new bool[7];
        playerAbility.stageInpo[1].stageInpo = new bool[7];
        playerAbility.stageInpo[2].stageInpo = new bool[7];
        playerAbility.stageInpo[3].stageInpo = new bool[7];
        playerAbility.stageInpo[4].stageInpo = new bool[7];
        playerAbility.stageInpo[5].stageInpo = new bool[7];
        playerAbility.stageInpo[6].stageInpo = new bool[7];
    }

    void DataApply()
    {
        int count = DataManager.Instance.loadFiles.selectCount - 1;
        if (!DataManager.Instance.isLoad)
        {
            playerAbility.date = DataManager.Instance.playerAbilitys[count].date;
            playerAbility.saveName = DataManager.Instance.playerAbilitys[count].saveName;
            playerAbility.saveCount = DataManager.Instance.playerAbilitys[count].saveCount;
            StageSet();
            DataManager.Instance.SaveFile();
        }
        else
        {
            playerAbility = DataManager.Instance.playerAbilitys[count];
            if (Inventory.Instance != null)
            {
                StartCoroutine(Inventory.Instance.LoadInventory());
                QuestWindow.Instance.PlayerQuestInit();
                statTextGroup.LoadEquip();
                skillList.LoadSkill();
            }
        }
        CinemaManager.Instance.LogInit();
        isLoadData = true;
    }

    public void SaveButton()
    {
        Debug.Log("체크");
        Inventory.Instance.SaveInventory();
        
        statTextGroup.SaveEquip();
        skillList.SaveSkill();
        if (playerAbility.stageInpo[0].stageInpo[0])
        {
            AudioManager.Instance.PlaySound("Save", Camera.main.transform.position);
            CurrentSaveLvStage = GameController.Instance.lvCount + "-" + GameController.Instance.StageCount + "-" + GameController.Instance.stageName;
            StartCoroutine(UIManager.Instance.SetMemory());
        }
        DataManager.Instance.SaveFile();
        
    }

    public void QuestCheck(int questNumber)
    {
        for (int i = 0; i < playerAbility.playerQuests.Count; i++)
        {
            if (playerAbility.playerQuests[i].questNubmer == questNumber && !playerAbility.playerQuests[i].isEnd)
                UIManager.Instance.QuestPlus(playerAbility.playerQuests[i],1);
        }
    }
}
