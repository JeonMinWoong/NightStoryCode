using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Enums;

[System.Serializable]
public class Ability
{
    public int level;
    public string obName;
    public float currentHealth;
    public int maxHealth;
    public int attack;
    public float moveSpeed;
    public float defense;
    public float attackSpeed;
    public int expSet;
    public int coinSet;
    public bool isScore;
    public bool isBoss;
    public MonsterType monsterType;
    public int attackClass;
    public float attackRange;

}

public class Stat : MonoBehaviour
{
    private Image nowHpbar;
    private Image priHpbar;
    public GameObject bossHpBar;
    public GameObject enemyHpBar;

    [SerializeField]
    private Ability ability;

    public int Level { get { return ability.level; } set { ability.level = value; } }
    public string ObName { get { return ability.obName; } set { ability.obName = value; } }
    public virtual float CurrentHealth
    {
        get { return ability.currentHealth; }
        set
        {
            ability.currentHealth = value;
        }
    }

    public int MaxHealth { get { return ability.maxHealth; } set { ability.maxHealth = value; } }
    public int Attack { get { return ability.attack; } set { ability.attack = value; } }
    public float MoveSpeed { get { return ability.moveSpeed; } set { ability.moveSpeed = value; } }
    public float Defense { get { return ability.defense; } set { ability.defense = value; } }
    public float AttackSpeed { get { return ability.attackSpeed; } set { ability.attackSpeed = value; } }
    public int ExpSet { get { return ability.expSet; } set { ability.expSet = value; } }
    public int CoinSet { get { return ability.coinSet; } set { ability.coinSet = value; } }
    public bool IsScore { get { return ability.isScore; } set { ability.isScore = value; } }
    public bool IsBoss { get { return ability.isBoss; } set { ability.isBoss = value; } }
    public MonsterType MonsterType { get { return ability.monsterType; } set { ability.monsterType = value; } }
    public int AttackClass { get { return ability.attackClass; } set { ability.attackClass = value; } }
    public float AttackRange { get { return ability.attackRange; } set { ability.attackRange = value; } }
    protected virtual void Awake()
    {
        if (enemyHpBar != null)
        {
            nowHpbar = enemyHpBar.transform.GetChild(1).GetComponent<Image>();
            priHpbar = enemyHpBar.transform.GetChild(0).GetComponent<Image>();
        }
    }

    private void Start()
    {
        MonsterName();
    }

    private void Update()
    {
        if (enemyHpBar != null)
        {
            nowHpbar.fillAmount = Mathf.Lerp(nowHpbar.fillAmount, ability.currentHealth / ability.maxHealth, Time.deltaTime * 8f);
            priHpbar.fillAmount = Mathf.Lerp(priHpbar.fillAmount, ability.currentHealth / ability.maxHealth, Time.deltaTime * 2f);
        }
        if (bossHpBar == null)
            return;
        else
        {
            nowHpbar.fillAmount = Mathf.Lerp(nowHpbar.fillAmount, ability.currentHealth / ability.maxHealth, Time.deltaTime * 8f);
            priHpbar.fillAmount = Mathf.Lerp(priHpbar.fillAmount, ability.currentHealth / ability.maxHealth, Time.deltaTime * 2f);
        }
    }

    void MonsterName()
    {
        if (name.Equals("Will"))
        {
            MonsterStatSet(1, "Will", 30 + (GameController.Instance.lvCount - 1) * 30,
                0, 0, 5, 0, 20, Random.Range(25 * GameController.Instance.lvCount, 51 * GameController.Instance.lvCount), true, false, Enums.MonsterType.None,0,0);
        }
        else if(name.Equals("TrainingDummy"))
        {
            MonsterStatSet(0, "TrainingDummy", 30,
               0, 0, 0, 0, 0, Random.Range(10, 31), false, false, Enums.MonsterType.None, 0, 0);
        }
        else if (name.Equals("TrainingAttack"))
        {
            MonsterStatSet(1, "TrainingAttack", 100, 0, 0, 0, 2, 0, Random.Range(50, 101), false, false, MonsterType.Small, 1, 2.8f);
        }
        //else if(name.Equals("Goblin"))
        else if (name.Equals("NewGoblin"))
        {
            MonsterStatSet(1, "Goblin", 80, 17,2, 2, 2, 15, Random.Range(21, 31), false, false, MonsterType.Small,1,2.5f);
        }
        else if(name.Equals("Worf"))
        {
            MonsterStatSet(3, "Worf", 120, 22, 4.5f , 4, 1.5f, 25, Random.Range(51, 81), false, false, MonsterType.Normal, 2, 2);
        }
        else if (name.Equals("GoblinBoss"))
        {
            BossHP();
            MonsterStatSet(5, "GoblinBoss", 800, 32, 5, 10, 2.2f, 220, Random.Range(1000, 2001), false, true,MonsterType.Normal,2,7f);
        }
        else if (name.Equals("Storeentrance"))
        {
            MonsterStatSet(0, "Storeentrance", 1,0, 0, 0, 0, 0, 0, false, false, MonsterType.None,0,0);
        }
        else if (name.Equals("Bat"))
        {
            MonsterStatSet(5, "Bat", 200, 30, 3, 10, 3, 25, Random.Range(29, 51), false, false, MonsterType.Small,1,7);
        }
        else if (name.Equals("Mushroom"))
        {
            MonsterStatSet(6, "Mushroom", 270, 50, 1, 15, 1.5f, 40, Random.Range(50, 101), false, false, MonsterType.Normal,2,7);
        }
        else if (name.Equals("RedGoblin"))
        {
            MonsterStatSet(8, "RedGoblin", 400, 30, 2.5f, 15, 3.2f, 60, Random.Range(100, 101), false, false, MonsterType.Normal,2,10);
        }
        else if (name.Equals("WizardBoss"))
        {
            BossHP();
            MonsterStatSet(10, "WizardBoss", 750, 40,3, 20, 2.5f, 80, Random.Range(150, 201), false, true, MonsterType.Big,1,12);
        }
    }

    void BossHP()
    {
        bossHpBar = GameObject.Find("BossHpbar").gameObject;
        if (bossHpBar != null)
        {
            nowHpbar = bossHpBar.transform.GetChild(0).GetChild(1).GetComponent<Image>();
            priHpbar = bossHpBar.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            TextMeshProUGUI bossNameText = bossHpBar.transform.GetChild(0).GetChild(2).GetComponent< TextMeshProUGUI>();
            string korName = "";
            if (name.Contains("GoblinBoss"))
                korName = "유적의 수호자";

            bossNameText.text = korName;
        }
    }

    public virtual void MonsterStatSet(int _lv,string _name, int _maxHp, int _attack, float _moveSpeed, int _defense,
        float _attackSpeed , int _exeSet, int _coinSet, bool _isScore,bool _isBoss , MonsterType _monsterType ,
        int _attackClass , float _attackRange )
    {
        Level = _lv;
        ObName = _name;
        MaxHealth = _maxHp;
        CurrentHealth = MaxHealth;
        Attack = _attack;
        MoveSpeed = _moveSpeed;
        Defense = _defense;
        AttackSpeed = _attackSpeed;
        ExpSet = _exeSet;
        CoinSet = _coinSet;
        IsScore = _isScore;
        IsBoss = _isBoss;
        MonsterType = _monsterType;
        AttackClass = _attackClass;
        AttackRange = _attackRange;
    }
}
