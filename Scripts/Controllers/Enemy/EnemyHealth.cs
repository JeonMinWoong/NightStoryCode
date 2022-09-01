using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : BaseHealth
{
    public ParticleSystem[] enemyParticle;
    Collider2D col;
    [SerializeField]
    private string deathSound;
    BaseController baseController;

    public bool isSkilld;
    public bool isSkillHit;
    GameObject goldEff;
    GameObject gemEff;

    public int[] dropList;

    Coroutine coPosion;

    public bool isStun;

    protected override void Awake()
    {
        base.Awake();

        ParticleCheck[] particleChecks = GetComponentsInChildren<ParticleCheck>();
        enemyParticle = new ParticleSystem[particleChecks.Length];
        for (int i = 0; i < particleChecks.Length; i++)
        {
            enemyParticle[i] = particleChecks[i].GetComponent<ParticleSystem>();
        }

        deathSound = name.Replace("(Clone)", "") + "Death";
        col = GetComponent<Collider2D>();
        baseController = GetComponent<BaseController>();
        goldEff = Resources.Load<ParticleSystem>("Load/Particle/GoldEff").gameObject;
        gemEff = Resources.Load<ParticleSystem>("Load/Particle/GemEff").gameObject;
    }

    private void OnEnable()
    {
        if (stat.MonsterType != Enums.MonsterType.None)
            rigid.isKinematic = false;
    }

    void Update()
    {
        if (isDeath == true)
            return;

        if (!isDeath && (name.Equals("GoblinBoss") || name.Equals("WizardBoss")))
        {
            if (GameController.Instance.bosszon.bossZon == true)
            {
                if (stat.bossHpBar.transform.GetChild(0).gameObject.activeSelf == false && CinemaManager.Instance.isBossCinema)
                    stat.bossHpBar.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    public void TakeKnockback(GameObject obj, float posX, float posY)
    {
        if (isDeath == true)
            return;

        rigid.velocity = Vector2.zero;

        Vector2 attackedVelocity = Vector2.zero;
        if (obj.transform.position.x > transform.position.x)
            attackedVelocity = new Vector2(-posX, posY);
        else
            attackedVelocity = new Vector2(posX, posY);

        rigid.AddForce(attackedVelocity, ForceMode2D.Impulse);
    }

    public IEnumerator Stun(float time)
    {
        Debug.Log("Stun");
        animator.SetBool("EnemyStun", true);
        isStun = true;
        enemyParticle[1].Play();
        yield return new WaitForSeconds(time);
        enemyParticle[1].Stop();
        animator.SetBool("EnemyStun", false);
        isStun = false;
    }

    //데미지 받는 함수
    public override void TakeDamage(GameObject obj, float damage, bool isCritical = false, bool isPersent = false, float trueDamage = 0, float counter = 0, float dash = 0)
    {
        if (isDeath)
            return;

        if (baseController.enabled == false)
        {
            baseController.enabled = true;
            animator.Rebind();
            rigid.gravityScale = 1;
        }

        if (!name.Equals("Storeentrance") && !name.Equals("TrainingDummy") && !name.Equals("TrainingDash"))
        {
            if(stat.IsBoss)
            {
                EnemyBossController ebc = GetComponent<EnemyBossController>();
                if(!ebc.isSkill)
                    animator.SetTrigger("EnemyHit");
            }
            else
                animator.SetTrigger("EnemyHit");
        }
        if (name.Equals("WizardBoss"))
        {
            AudioManager.Instance.PlaySound("WizardHit", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        }
        else if (name.Equals("Worf"))
        {
            AudioManager.Instance.PlaySound("WorfHit", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        }

        ParticleSystem.EmissionModule emission = enemyParticle[0].emission;

        PlayerStat pS = obj.GetComponent<PlayerStat>();

        float damageValue = Mathf.Round(damage - stat.Defense);

        damageValue = damageValue >= 1 ? damageValue : 1;

        if(isStun)
            damageValue *= 1.5f;

        if (pS != null)
            damageValue = damageValue + pS.armorLgnore;

        if (isCritical == true)
            damageValue *= 2;

        if (trueDamage == 0 && counter == 0 && dash == 0)
        {
            isdamage = true;
            emission.enabled = true;
            enemyParticle[0].Play();
            StartCoroutine(IsDamage());
            stat.CurrentHealth -= damageValue;
            base.TakeDamage(obj, damageValue, isCritical);
        }
        else if(trueDamage == 0 && counter != 0 && dash == 0)
        {
            stat.CurrentHealth -= (int)counter;
            base.TakeDamage(obj, 0, counter: (int)counter);
        }
        else if(trueDamage == 0 && counter == 0 && dash != 0)
        {
            dash = dash <= 0 ? 1 : dash;
            stat.CurrentHealth -= (int)dash;
            base.TakeDamage(obj, 0, dash: (int)dash);
        }
        else
        {
            stat.CurrentHealth -= trueDamage;
            base.TakeDamage(obj, 0, trueDamage: trueDamage);
        }

        //체력이 0 이하면 사망
        if (stat.CurrentHealth <= 0)
        {
            isDeath = true;
            CinemaManager.Instance.isBossCinema = false;
            Die(obj);
        }
    }

    public void Posion(GameObject obj, float damage)
    {
        if (coPosion == null)
            coPosion = StartCoroutine(PoisonDamage(obj, damage));
        else
            return;
    }

    public IEnumerator PoisonDamage(GameObject obj,float damage)
    {
        int count = 0;
        while(count < 3)
        {
            TakeDamage(obj, 0, trueDamage: damage);
            count++;
            yield return new WaitForSeconds(1);
        }
        coPosion = null;
    }

    public void SkillHit()
    {
        StartCoroutine(IsSkillHit());
    }

    public IEnumerator IsSkillHit()
    {
        isSkillHit = true;
        yield return new WaitForSeconds(1);
        isSkillHit = false;
    }

    public IEnumerator IsSkilled(int type)
    {
        if (type == 0)
        {
            EnemyController eA = null;
            EnemyBossController eBA = null;
            if (!stat.IsBoss)
                eA = GetComponent<EnemyController>();
            else
            {
                eBA = GetComponent<EnemyBossController>();
                if (eBA.isSkill)
                    yield break;
            }
            
            rigid.isKinematic = true;
            rigid.velocity = Vector2.zero;
            animator.SetTrigger("EnemyDeath");
            isSkilld = true;
            if (stat.MonsterType != Enums.MonsterType.None)
            {
                if (!stat.IsBoss)
                {
                    if (eA.coAttack != null)
                        eA.StopCoroutine(eA.coAttack);
                    eA.isAttack = false;
                }
                else
                {
                    if (eBA.coAttack != null)
                        eBA.StopCoroutine(eBA.coAttack);
                    if (eBA.coSkill != null)
                        eBA.StopCoroutine(eBA.coSkill);
                    if (eBA.coSkill2 != null)
                        eBA.StopCoroutine(eBA.coSkill2);
                    if (eBA.coSkill3 != null)
                        eBA.StopCoroutine(eBA.coSkill3);
                    eBA.isAttack = false;
                    eBA.isSkill = false;

                    for (int i = 0; i < enemyParticle.Length; i++)
                    {
                        enemyParticle[i].Stop();
                    }
                }

                yield return new WaitForSeconds(2);

                if (stat.MonsterType != Enums.MonsterType.None)
                {
                    if (!stat.IsBoss)
                    {
                        if (eA.coAttack != null)
                            eA.StartCoroutine(eA.coAttack);
                        eA.isAttack = false;
                    }
                    else
                    {
                        if (eBA.coAttack != null)
                            eBA.StartCoroutine(eBA.coAttack);
                        if (eBA.coSkill != null)
                            eBA.StartCoroutine(eBA.coSkill);
                        if (eBA.coSkill2 != null)
                            eBA.StartCoroutine(eBA.coSkill2);
                        if (eBA.coSkill3 != null)
                            eBA.StartCoroutine(eBA.coSkill3);
                        eBA.isAttack = false;
                        eBA.isSkill = false;


                    }

                    rigid.isKinematic = false;
                }

                if (isDeath == false)
                    animator.Rebind();
            }

            else if (type == 1)
            {
                yield return new WaitForSeconds(0.5f);
            }
            isSkilld = false;
        }
    }


    IEnumerator IsDamage()
    {
        yield return new WaitForSeconds(1f);
        enemyParticle[0].Stop(); 
        ParticleSystem.EmissionModule emission = enemyParticle[0].emission;
        emission.enabled = false;
        isdamage = false;
    }

    protected override void Die(GameObject obj)
    {
        if (obj.tag == "Player")
            Drop(obj);

        AudioManager.Instance?.PlaySound(deathSound, transform.position, 1f + Random.Range(-0.1f, 0.1f));

        if (!name.Equals("TrainingDummy") && !name.Equals("TrainingDash"))
            animator.SetTrigger("EnemyDeath");

        DieQuestCheck();

        if (stat.IsScore == true)
            ScoreManager.Instance.ScoreGroup++;

        StartCoroutine(CoDie());
    }

    IEnumerator CoDie()
    {
        SpriteRenderer sp = gameObject.GetComponent<SpriteRenderer>();
        rigid.gravityScale = 0;
        rigid.velocity = Vector2.zero;
        col.enabled = false;

        if (stat.enemyHpBar != null)
        {
            CanvasGroup canvasHpBar = stat.enemyHpBar.GetComponent<CanvasGroup>();
            while (true)
            {
                canvasHpBar.alpha -= 0.03f;
                if (canvasHpBar.alpha <= 0)
                    break;
                yield return null;
            }

            for (int i = 0; i < 20; i++)
            {
                sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, sp.color.a - 0.05f);
                yield return new WaitForSeconds(0.05f);
            }

            gameObject.SetActive(false);

        }
        else
            rigid.simulated = false;
        rigid.gravityScale = 1;
        col.enabled = true;
        sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, 1);

        if (stat.bossHpBar != null)
        {
            stat.bossHpBar.SetActive(false);
            EnemyBossController enemyBossController = GetComponent<EnemyBossController>();
            enemyBossController.Speech(5); 
        }
    }

    public void Drop(GameObject obj)
    {
        ItemBox();

        if (obj == null)
            return;

        PlayerHealth playerH = obj.GetComponent<PlayerHealth>();
        PlayerStat playerStat = obj.GetComponent<PlayerStat>();
        if (name.Equals("GoblinBoss"))
        {
            enemyParticle[2].Play();
            GameController.Instance.MapClear();
            transform.Find("MiniMap").gameObject.SetActive(false);
        }
        else if (name.Equals("WizardBoss"))
        {
            GameController.Instance.MapClear();
        }
        if (stat.name.Contains("Will") == false)
        {
            Instantiate(goldEff, transform.position, Quaternion.identity, transform);
            AudioManager.Instance?.PlaySound("Coin_Drop", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            playerH?.GiveCoin(stat.CoinSet);
        }
        else
        {
            Instantiate(gemEff, transform.position, Quaternion.identity, transform);
            AudioManager.Instance?.PlaySound("Gem_Drop", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            playerH?.GiveSkillPoint(stat.Level * 3);
        }

        playerH?.TakeExp(stat.ExpSet);
    }

    void ItemBox()
    {
        GameObject itemBox = Resources.Load<GameObject>("Prefabs/ItemBox/Normal_Box");

        int dropItem = DropItem();

        if (dropItem == 0)
            return;

        itemBox = Instantiate(itemBox, transform.position, Quaternion.identity);

        itemBox.GetComponent<Item>().itemInpos.itemName = DBManager.Instance.itemInpos[dropItem].itemName;
        itemBox.GetComponent<Item>().itemInpos.itemClass = DBManager.Instance.itemInpos[dropItem].itemClass;
        itemBox.GetComponent<Item>().itemInpos.itemCount = DBManager.Instance.itemInpos[dropItem].itemCount;
        itemBox.GetComponent<Item>().itemInpos.type = DBManager.Instance.itemInpos[dropItem].type;
        itemBox.GetComponent<Item>().itemInpos.wear = DBManager.Instance.itemInpos[dropItem].wear;
        itemBox.GetComponent<Item>().itemInpos.explanation = DBManager.Instance.itemInpos[dropItem].explanation;
        itemBox.GetComponent<Item>().itemInpos.limitLv = DBManager.Instance.itemInpos[dropItem].limitLv;
        itemBox.GetComponent<Item>().itemInpos.kinds = DBManager.Instance.itemInpos[dropItem].kinds;
        itemBox.GetComponent<Item>().itemInpos.spritePath = DBManager.Instance.itemInpos[dropItem].spritePath;
        itemBox.GetComponent<Item>().itemInpos.price = DBManager.Instance.itemInpos[dropItem].price;

        int classType = (int)itemBox.GetComponent<Item>().itemInpos.itemClass;
        Sprite classSprite = null;

        if (classType == 1)
            classSprite = Resources.Load<Sprite>("Sprite/Item_Box/Box_02");
        else if (classType == 2)
            classSprite = Resources.Load<Sprite>("Sprite/Item_Box/Box_03");
        else if (classType == 3)
            classSprite = Resources.Load<Sprite>("Sprite/Item_Box/Box_04");
        else if (classType == 4)
            classSprite = Resources.Load<Sprite>("Sprite/Item_Box/Box_05");
        else
            classSprite = Resources.Load<Sprite>("Sprite/Item_Box/Box_01");

        itemBox.GetComponent<SpriteRenderer>().sprite = classSprite;
        itemBox.GetComponent<Item>().isDropItem = true;
    }

    private int DropItem()
    {
        int rand = Random.Range(0, 101);
        int itemNumber = 0;
        if(rand <= 5)
        {
            List<int> equipItemGroup = new List<int>();
            for (int i = 0; i < dropList.Length; i++)
            {
                if(dropList[i] > 1000)
                {
                    equipItemGroup.Add(dropList[i]);
                }
            }

            int equipItemRange = Random.Range(0, equipItemGroup.Count);

            if (equipItemGroup.Count > 0)
                itemNumber = equipItemGroup[equipItemRange];
            else
                itemNumber = 0;
        }
        else if(rand > 5 && rand <= 20)
        {
            List<int> comsumItemGroup = new List<int>();
            for (int i = 0; i < dropList.Length; i++)
            {
                if (dropList[i] <= 100)
                {
                    comsumItemGroup.Add(dropList[i]);
                }
            }

            int etcItemRange = Random.Range(0, comsumItemGroup.Count);

            if (comsumItemGroup.Count > 0)
                itemNumber = comsumItemGroup[etcItemRange];
            else
                itemNumber = 0;

        }
        else
        {
            List<int> etcItemGroup = new List<int>();
            for (int i = 0; i < dropList.Length; i++)
            {
                if (dropList[i] > 100 && dropList[i] < 1000)
                {
                    etcItemGroup.Add(dropList[i]);
                }
            }

            int etcItemRange = Random.Range(0, etcItemGroup.Count);

            if (etcItemGroup.Count > 0)
                itemNumber = etcItemGroup[etcItemRange];
            else
                itemNumber = 0;
        }

        return itemNumber;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Finish")
        {
            TakeDamage(gameObject, 10000);
        }
    }

    void DieQuestCheck()
    {
        if(name.Contains("Goblin"))
        {
            QuestMonster(11);
           
        }
        else if(name.Contains("Worf"))
        {
            QuestMonster(12);
        }
    }

    void QuestMonster(int value)
    {
        QuestList currnetQuest = null;
        for (int i = 0; i < UIManager.Instance.playerStat.playerAbility.playerQuests.Count; i++)
        {
            if (UIManager.Instance.playerStat.playerAbility.playerQuests[i].questNubmer == value && !UIManager.Instance.playerStat.playerAbility.playerQuests[i].isEnd)
            {
                currnetQuest = UIManager.Instance.playerStat.playerAbility.playerQuests[i];
                break;
            }
        }
        if (currnetQuest == null)
            return;
        if (currnetQuest.isEnd)
            return;

        UIManager.Instance.QuestPlus(currnetQuest, 1);
    }

    private void OnDisable()
    {
        isSkillHit = false;

        if (stat.enemyHpBar != null)
            stat.enemyHpBar.GetComponent<CanvasGroup>().alpha = 1;
    }
}
