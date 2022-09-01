using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerHealth : BaseHealth
{
    Text[] textUi;
    public bool maxSkilld = false;

    public ParticleSystem[] playerParticle;

    public bool isStun;
    Coroutine coHp;
    Coroutine coSt;
    Coroutine coSk;
    Coroutine costL;
    public Coroutine coBlessing;

    public void Revive()
    {
        Init();
        if (!playerStat.playerAbility.isDeathEvent)
            CinemaManager.Instance.PlayerDeathEvent();
    }

    protected override void Awake()
    {
        base.Awake();
        playerStat = GetComponent<PlayerStat>();
        textUi = GetComponentsInChildren<Text>();
        playerStat.MaxLife = 5;
        playerStat.CurrentLife = playerStat.MaxLife;
    }

    public void Init()
    {
        isDeath = false;
        playerStat.CurrentHealth = playerStat.playerAbility.maxHealth;
        playerStat.CurrentStamina = playerStat.MaxStamina;
        playerStat.CurrentSkillGage = 0;
        coHp = StartCoroutine(Hp());
        coSt = StartCoroutine(Stamina());
        coSk = StartCoroutine(Skill());
    }

    IEnumerator Hp()
    {
        while (isDeath == false)
        {
            if (playerStat.CurrentHealth <= playerStat.playerAbility.maxHealth)
                playerStat.CurrentHealth += playerStat.ReGen;

            yield return new WaitForSeconds(1f);
        }
    }
    IEnumerator Stamina()
    {
        while (isDeath == false)
        {
            if (playerStat.CurrentStamina <= playerStat.MaxStamina)
            {
                playerStat.CurrentStamina += playerStat.StaminaReGen * 0.2f;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void Blessing()
    {
        if (coBlessing == null && playerStat.blessing != 0)
            coBlessing = StartCoroutine(CoBlessing());
        else
            return;
    }

    IEnumerator CoBlessing()
    {
        yield return new WaitForSeconds(30f);
        Debug.Log("축복");
        int rand = Random.Range(0, 101);
        if (rand < playerStat.blessing)
        {
            float plusHp = (playerStat.MaxHealth * (playerStat.blessing * 0.01f));
            GiveHp((int)plusHp);
        }

        coBlessing = null;
        Blessing();
    }

    public void IdleStamina()
    {
        if (playerStat.CurrentStamina >= playerStat.MaxStamina)
            return;

        playerStat.CurrentStamina += playerStat.MaxStamina * 0.01f;
    }

    IEnumerator Skill()
    {
        while (isDeath == false)
        {
            if (playerStat.CurrentSkillGage < playerStat.MaxSkillGage)
            {
                playerStat.CurrentSkillGage += 0.1f;
                playerStat.skillGage = Enums.SkillGage.None;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void MaxSkill()
    {
        playerStat.skillGage = Enums.SkillGage.MaxIng;
        AudioManager.Instance.PlaySound("MaxSkill", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        playerParticle[3].gameObject.SetActive(true);
        playerParticle[3].Play();
    }

    public void TakeStaMina(float stamina)
    {
        playerStat.CurrentStamina -= stamina;
    }

    public void TakeSkill(int skill)
    {
        playerStat.CurrentSkillGage -= skill;
    }


    public void GiveHp(int hp)
    {
        playerStat.CurrentHealth += hp + (int)(hp * playerStat.potionPlus * 0.01f);
    }

    public void GiveStaMina(float stamina)
    {
        playerStat.CurrentStamina += stamina + (int)(stamina * playerStat.potionPlus * 0.01f);
    }

    public void GiveSkill(int skill)
    {
        playerStat.CurrentSkillGage += skill + (int)(skill * playerStat.soul * 0.01f);
    }

    public void GiveCoin(int coin)
    {
        playerStat.CoinGet += coin + (int)(coin * playerStat.goldenRule * 0.01f);
    }
    public void GiveSkillPoint(int poin)
    {
        playerStat.SkillPoint += poin;
    }

    public void TakeCoin(int coin)
    {
        playerStat.CoinGet -= coin;
    }

    public IEnumerator Stun(float time)
    {
        Debug.Log("Stun");
        animator.SetBool("Stun",true);
        isStun = true;
        yield return new WaitForSeconds(time);
        animator.SetBool("Stun", false);
        isStun = false;
        animator.Rebind();
        PlayerController playerController = GetComponent<PlayerController>();
        playerController.skillUse = false;
        playerController.isAttack = false;
        playerController.isAttack_Strong = false;
    }

    public void LevelUp()
    {
        playerStat.playerAbility.level++;
        playerStat.CurrentExp = 0;
        playerStat.SetPlayerStatus();
        AudioManager.Instance.PlaySound("LevelUp", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        playerParticle[8].gameObject.SetActive(true);
        playerParticle[8].Play();
        StartCoroutine(CoLevelUpText());
        UIManager.Instance.WindowUpdate(0);
        switch (playerStat.Level)
        {
            case 5:
            case 13:
            case 21:
                UIManager.Instance.WindowUpdate(2);
                GameController.Instance.NoticeSet(1,"패시브 스킬");
                break;
            case 2:
            case 6:
            case 12:
                UIManager.Instance.WindowUpdate(2);
                GameController.Instance.NoticeSet(1, "액티브 스킬");
                break;
            default:
                break;
        }
    }

    IEnumerator CoLevelUpText()
    {
        float alpha = 0;
        bool isMax = false;
        GameObject canvasOj = textUi[0].transform.parent.gameObject;
        
        yield return new WaitForSeconds(0.5f);

        while(true)
        {
            if (!isMax)
            {
                alpha += 0.02f;
                if (alpha >= 1)
                    isMax = true;
            }
            else
            {
                alpha -= 0.02f;
                if (alpha <= 0)
                    break;
            }
            textUi[0].color = new Color(1, 1, 0, alpha);

            if (transform.localScale.x > 0)
                canvasOj.transform.localScale = new Vector2(Mathf.Abs(canvasOj.transform.localScale.x),canvasOj.transform.localScale.y);
            else
                canvasOj.transform.localScale = new Vector2(-Mathf.Abs(canvasOj.transform.localScale.x), canvasOj.transform.localScale.y);
            yield return null;
        }
    }

    public void StminaLack()
    {
        if (costL != null)
        {
            StopCoroutine(costL);
            costL = null;
        }
        costL = StartCoroutine(CoStminaLack());
    }

    public IEnumerator CoStminaLack()
    {
        float alpha = 1;
        GameObject canvasOj = textUi[0].transform.parent.gameObject;
        AudioManager.Instance.PlaySound("StminaLack", Camera.main.transform.position);
        while (true)
        {
            alpha -= 0.01f;
            textUi[1].color = new Color(1, 0, 0, alpha);

            if (alpha <= 0)
                break;

            

            if (transform.localScale.x > 0)
                canvasOj.transform.localScale = new Vector2(Mathf.Abs(canvasOj.transform.localScale.x), canvasOj.transform.localScale.y);
            else
                canvasOj.transform.localScale = new Vector2(-Mathf.Abs(canvasOj.transform.localScale.x), canvasOj.transform.localScale.y);
            yield return null;
        }
        costL = null;
    }

    public void TakeExp(int exp)
    {
        playerStat.CurrentExp += exp;

        if (playerStat.CurrentExp >= playerStat.MaxExp)
        {
            int remain = playerStat.CurrentExp - playerStat.MaxExp;

             LevelUp();

            if (remain > 0)
                TakeExp(remain);
        }
    }

    public void TakeKnockback(GameObject obj,float posX, float posY)
    {
        rigid.velocity = Vector2.zero;

        Vector2 attackedVelocity = Vector2.zero;
        if (transform.position.x < obj.transform.position.x)
            attackedVelocity = new Vector2(-posX, posY);
        else
            attackedVelocity = new Vector2(posX, posY);

        rigid.AddRelativeForce(attackedVelocity, ForceMode2D.Impulse);
    }

    public override void TakeDamage(GameObject obj, float damage, bool isCritical = false,bool isPersent = false, float trueDamage = 0, float counter = 0,float dash = 0)
    {
       
        //이미 죽었으면 데미지를 더 이상 받지 않게 처리
        if (isDeath)
            return;

        if (GetComponent<PlayerController>().skillUse == true)
            return;

        //TakeKnockback(obj, 3, 2);


        float damageValue = 0;

        if (isPersent)
        {
            damageValue = Mathf.Round(playerStat.MaxHealth * (damage / (100 + playerStat.trapResistance)));
        }
        else
            damageValue = Mathf.Round(damage - playerStat.Defense);

        damageValue = damageValue >= 1 ? damageValue : 1;

        playerStat.CurrentHealth -= damageValue;

        base.TakeDamage(obj, damageValue);
        PlayerController playerController = GetComponent<PlayerController>();
        if(!playerController.skillUse && !isStun && !playerController.isAttack && !playerController.isAttack_Strong)
            animator.SetTrigger("Hit");
        playerParticle[2].gameObject.SetActive(true);
        playerParticle[2].Play();

        if (playerStat.CurrentHealth <= 0)
        {
            isDeath = true;
            Die(obj);
        }

    }
    public void HpRecovery(int recovery)
    {
        Recovery();
        playerStat.CurrentHealth += recovery;
    }

    public void SpRecovery(int recovery)
    {
        Recovery();
        playerStat.CurrentStamina += recovery;

    }
    void Recovery()
    {
        playerParticle[5].gameObject.SetActive(true);
        playerParticle[5].Play();
    }

    public void GiveCoinEff()
    {
        playerParticle[6].gameObject.SetActive(true);
        playerParticle[6].Play();
    }
    
    void SmithyAttack()
    {
        playerParticle[7].gameObject.SetActive(true);
        playerParticle[7].Play();
    }

  

    public void Defensed(GameObject obj,float counterValue)
    {
        playerParticle[4].gameObject.SetActive(true);
        playerParticle[4].Play();

        if (playerStat.counter == 0)
            return;

        EnemyHealth eh = obj.GetComponent<EnemyHealth>();

        counterValue = counterValue * playerStat.counter * 0.01f;

        eh.TakeDamage(gameObject,0, counter:counterValue);

    }


    protected override void Die(GameObject obj)
    {
        //죽는 애니메이션 재생
        animator.speed = 1f;
        animator.SetTrigger("Death");
        coHp = null;
        coSt = null;
        coSk = null;
        //이동 못하게
        GetComponent<PlayerController>().enabled = false;
        this.enabled = false;
        if (playerStat.CurrentLife > 0)
            playerStat.CurrentLife--;
        else
        {
            GameController.Instance.StartCoroutine(GameController.Instance.LifeEnd());
            return;
        }
        GameController.Instance.GameOver();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            TakeDamage(gameObject, 10000);
        }
    }

}