using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using static Enums;
using UnityEngine.UI;

public class EnemyBossController : EnemyController
{
    public Transform attackPos2;
    private float pos2X;
    public Vector2 boxSize2;
    public Transform attackPos3;
    private float pos3X;
    public Vector3 boxSize3;
    public Transform attackPos4;
    private float pos4X;
    public Vector3 boxSize4;

    private int tpTionRoot;
    public GameObject[] tpPos;
    private int skillRoot;
    [SerializeField]
    private EnemyHealth enemyHealth;
    public GameObject[] firetrp;
    public GameObject fireBall;

    public IEnumerator coSkill;
    float skill_Cool;
    public IEnumerator coSkill2;
    float skill_Cool2;
    public IEnumerator coSkill3;
    float skill_Cool3;
    public bool isSkill;

    public IEnumerator coLastSkill;
    float lastSkill_Cool;
    public bool isLastSkill;

    Text speech;
    Text subSpeech;
    [SerializeField]
    [TextArea(3, 10)]
    string[] speechGroup;

    Coroutine coSpeech;

    float waitingLength;

    Coroutine coWaiting;

    [SerializeField]
    Vector2[] particlePos;

    protected override void Awake()
    {
        base.Awake();
        pos2X = attackPos2.localPosition.x;
        pos3X = attackPos3.localPosition.x;
        pos4X = attackPos4.localPosition.x;
        speech = transform.Find("TextCanvas/Text").GetComponent<Text>();
        subSpeech = speech.transform.Find("Text").GetComponent<Text>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        animator.speed = 1;
        coSkill = null;
        coSkill2 = null;
        coSkill3 = null;
        coWaiting = null;
        coLastSkill = null;
        isAttack = false;
        isSkill = false;

        coSkill = SkillCool(skill_Cool);
        coSkill2 = SkillCool2(skill_Cool2);
        coLastSkill = SkillCoolLast(lastSkill_Cool);

        StartCoroutine(coSkill);
        StartCoroutine(coSkill2);
        StartCoroutine(coLastSkill);
    }

    protected override void Start()
    {
        base.Start();
        enemyHealth = GetComponent<EnemyHealth>();

        if (name.Equals("GoblinBoss"))
        {
            waitingLength = 2;

            skillRoot = 2;
            skill_Cool = 10;
            skill_Cool2 = 20;
            lastSkill_Cool = 10;
        }
        if (name.Equals("WizardBoss"))
        {
            waitingLength = 2.5f;

            skillRoot = 3;
            skill_Cool = 7.5f;
            skill_Cool2 = 5;
            skill_Cool3 = 12.5f;
        }

    }
    protected override void Update()
    {
        if (isSkill)
            return;
        if (enemyHealth.isDeath == true)
            return;

        base.Update();

        if (coWaiting != null)
            return;

        if (GetComponent<EnemyHealth>().isStun)
            return;

        BossSkill();
    }

    protected override void Attack()
    {
        if (traceTarget == null)
            return;
        if (traceTarget.GetComponent<BaseHealth>().isDeath == true)
            return;
        if (traceTarget.GetComponent<PlayerController>().isDash == true)
            return;

        if (isSkill == true)
            return;

        if (isAttackPossible == false)
            return;

        if (coAttack != null)
            return;

        if (coWaiting != null)
            return;

        if (stat.MonsterType == Enums.MonsterType.Small)
        {
            coAttack = CoAttack();
            StartCoroutine(coAttack);
        }
        else if (stat.MonsterType >= Enums.MonsterType.Normal)
        {
            int attackType = Random.Range(0, stat.AttackClass);

            //Debug.Log("보스 공격");

            if (attackType == 0)
            {
                coAttack = CoAttack();
                StartCoroutine(coAttack);
                coWaiting = StartCoroutine(CoWaiting());
            }
            else if (attackType == 1)
            {
                coAttack = CoAttack_Strong();
                StartCoroutine(coAttack);
                coWaiting = StartCoroutine(CoWaiting());
            }
        }
    }

    protected override IEnumerator CoAttack()
    {
        isAttack = true;
        animator.SetTrigger("EnemyAttack");
        yield return new WaitUntil(() => enemyAttacKing);
        Speech(0);
        Collider2D[] collider2D = Physics2D.OverlapBoxAll(attackPos.position, boxSize, 0);
        foreach (Collider2D collider in collider2D)
        {
            if (collider.tag == "Player")
            {
                if (coMove != null)
                {
                    StopCoroutine(coMove);
                    coMove = null;
                }

                if (collider.GetComponent<PlayerController>().isDefense == true)
                {
                    isAttackFalse = true;
                    StartCoroutine(IsAttackFalse());
                    collider.GetComponent<PlayerHealth>().Defensed(gameObject, stat.Attack);
                    rigid.velocity = Vector2.zero;
                    Vector2 attackfalse = Vector2.zero;
                    attackfalse = new Vector2(20f * flipX, 0);
                    rigid.AddForce(attackfalse, ForceMode2D.Impulse);
                    collider.GetComponent<PlayerHealth>().TakeDamage(gameObject, stat.Attack * 0.2f);
                    EnemyHealth eh = GetComponent<EnemyHealth>();
                    StartCoroutine(eh.Stun(1));
                    if (name.Equals("GoblinBoss"))
                        AudioManager.Instance.PlaySound("BlockMetalMetal05", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                }
                else
                {
                    if (name.Equals("WizardBoss"))
                        AudioManager.Instance.PlaySound("WizardAttack", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                    else if (name.Equals("GoblinBoss"))
                        AudioManager.Instance.PlaySound("HitSword07", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                    collider.GetComponent<PlayerHealth>().TakeDamage(gameObject, stat.Attack);
                }
                collider.GetComponent<PlayerHealth>().TakeKnockback(gameObject, 6, 0);
            }
        }
        isAttack = false;
        yield return new WaitForSeconds(stat.AttackSpeed);
        coAttack = null;

    }

    protected override IEnumerator CoAttack_Strong()
    {
        isAttack = true;
        animator.SetTrigger("EnemyAttack2");
        Speech(1);
        yield return new WaitUntil(() => enemyAttacKing);
        Collider2D[] collider2D = Physics2D.OverlapBoxAll(attackPos2.position, boxSize, 0);
        foreach (Collider2D collider in collider2D)
        {
            if (collider.tag == "Player")
            {
                if (coMove != null)
                {
                    StopCoroutine(coMove);
                    coMove = null;
                }

                if (collider.GetComponent<PlayerController>().isDefense == true)
                {
                    isAttackFalse = true;
                    StartCoroutine(IsAttackFalse());
                    collider.GetComponent<PlayerHealth>().Defensed(gameObject, stat.Attack * 1.5f);
                    rigid.velocity = Vector2.zero;
                    Vector2 attackfalse = Vector2.zero;
                    attackfalse = new Vector2(20 * flipX, 0);
                    rigid.AddForce(attackfalse, ForceMode2D.Impulse);

                    EnemyHealth eh = GetComponent<EnemyHealth>();
                    StartCoroutine(eh.Stun(1));

                    collider.GetComponent<PlayerHealth>().TakeDamage(gameObject, stat.Attack * 0.4f);
                    AudioManager.Instance.PlaySound("BlockMetalMetal05", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                }
                else
                {
                    AudioManager.Instance.PlaySound("HitSword07", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                    
                    collider.GetComponent<PlayerHealth>().TakeDamage(gameObject, stat.Attack * 1.5f);
                }
                if (name.Equals("GoblinBoss"))
                    collider.GetComponent<PlayerHealth>().TakeKnockback(gameObject, 10, 0);
                else
                    collider.GetComponent<PlayerHealth>().TakeKnockback(gameObject, 3, 2);
            }
        }
        isAttack = false;
        yield return new WaitForSeconds(stat.AttackSpeed * 1.5f);
        coAttack = null;

    }

    public void Speech(int _count)
    {
        if (coSpeech == null)
            coSpeech = StartCoroutine(CoSpeech(_count));
        else
        {
            StopCoroutine(coSpeech);
            coSpeech = StartCoroutine(CoSpeech(_count));
        }
    }

    IEnumerator CoSpeech(int _count)
    {
        speech.transform.gameObject.SetActive(true);
        speech.text = speechGroup[_count];
        subSpeech.text = speechGroup[_count];
        Debug.Log(_count + " / " + speechGroup.Length);
        if(_count + 1 != speechGroup.Length)
            yield return new WaitForSeconds(1);
        else
            yield return new WaitForSeconds(3);
        speech.text = "";
        subSpeech.text = "";
        speech.transform.gameObject.SetActive(false);
        coSpeech = null;
    }

    protected override void BoxPos()
    {
        base.BoxPos();
        if (spriteRenderer.flipX == true)
        {
            attackPos2.localPosition = new Vector3(-pos2X, attackPos.localPosition.y, 0);
            attackPos3.localPosition = new Vector3(-pos3X, attackPos.localPosition.y, 0);
            attackPos4.localPosition = new Vector3(-pos4X, attackPos.localPosition.y, 0);
        }
        else
        {
            attackPos2.localPosition = new Vector3(pos2X, attackPos.localPosition.y, 0);
            attackPos3.localPosition = new Vector3(pos3X, attackPos.localPosition.y, 0);
            attackPos4.localPosition = new Vector3(pos4X, attackPos.localPosition.y, 0);
        }
    }

    void BossSkill()
    {
        if (GameController.Instance.bosszon.bossZon == false)
            return;

        if (isAttack == true)
            return;

        if (isSkill == true)
            return;

        if (enemyAttacKing == true)
            return;

        if (name.Equals("GoblinBoss"))
        {
            int currentHealthPer = Mathf.RoundToInt(enemyHealth.stat.MaxHealth * 0.4f);

            int rand = enemyHealth.stat.CurrentHealth <= currentHealthPer ? Random.Range(0, skillRoot + 1) : Random.Range(0, skillRoot);

            if (isAttackPossible == false)
                return;

            Debug.Log(rand);

            if (rand == 2)
            {
                if (coLastSkill != null)
                    return;

                coLastSkill = GBLastSkill();
                StartCoroutine(coLastSkill);
                coWaiting = StartCoroutine(CoWaiting());
                Speech(4);
            }
            else if (rand == 0)
            {
                if (coSkill != null)
                    return;

                Collider2D collider2D = Physics2D.OverlapBox(attackPos3.position, boxSize3, 0);

                if (collider2D.tag.Equals("Player"))
                {
                    coSkill = GBSkill();
                    StartCoroutine(coSkill);
                    coWaiting = StartCoroutine(CoWaiting());
                    Speech(2);
                }
                else
                    return;
            }
            else if (rand == 1)
            {
                if (isRun == true)
                    return;

                if (coSkill2 != null)
                    return;

                coSkill2 = GBSkill2();
                StartCoroutine(coSkill2);
                coWaiting = StartCoroutine(CoWaiting());
                Speech(3);
            }
        }
        else if (name.Equals("WizardBoss"))
        {
            int rand = Random.Range(0, skillRoot);

            if (rand == 0)
            {
                if (coSkill != null)
                    return;

                coSkill = WBSkill();
                StartCoroutine(coSkill);
            }
            else if(rand == 1)
            {
                if (coSkill2 != null)
                    return;

                coSkill2 = WBSkill2();
                StartCoroutine(coSkill2);
            }
            else
            {
                if (coSkill3 != null)
                    return;

                coSkill3 = WBSkill3();
                StartCoroutine(coSkill3);
            }
        }
    }

    IEnumerator GBSkill()
    {
        isSkill = true;

        float value = 0;
        Color defultColor = spriteRenderer.color;
        while(true)
        {
            value += 0.02f;
            spriteRenderer.color = new Color(defultColor.r, defultColor.g - value, defultColor.b - value);
            if (value >= 1)
                break;
            yield return null;
        }
        AudioManager.Instance.PlaySound("goblinBossSkill", transform.position, 1f + Random.Range(-0.1f, 0.1f));

        Collider2D collider2D = Physics2D.OverlapBox(attackPos3.position, boxSize3, 0);
        if (collider2D.tag == "Player")
        {
            if (spriteRenderer.flipX == true)
            {
                enemyHealth.enemyParticle[3].gameObject.SetActive(true);
                enemyHealth.enemyParticle[3].Play();
            }
            else
            {
                enemyHealth.enemyParticle[4].gameObject.SetActive(true);
                enemyHealth.enemyParticle[4].Play();
            }
            collider2D.GetComponent<PlayerHealth>().TakeStaMina(20);
            collider2D.GetComponent<PlayerHealth>().TakeKnockback(gameObject, 3, 2);
            collider2D.GetComponent<PlayerHealth>().TakeDamage(gameObject, stat.Attack * 1.5f);
        }
        spriteRenderer.color = defultColor;
        StartCoroutine(SkillCool(skill_Cool));
        isSkill = false;
    }

    IEnumerator GBSkill2()
    {
        isSkill = true;
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlaySound("goblinBossSkill", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        int count = 0;
        while (count < 5)
        {
            animator.speed = 3;

            animator.SetTrigger("EnemyAttack");

            Collider2D[] collider2D = Physics2D.OverlapBoxAll(attackPos.position, boxSize3, 0);
            foreach (Collider2D collider in collider2D)
            {
                if (collider.tag == "Player")
                {
                    collider.GetComponent<PlayerHealth>().TakeKnockback(gameObject, 3, 2);
                    collider.GetComponent<PlayerHealth>().TakeDamage(gameObject, stat.Attack * 0.5f);
                }
            }
            yield return new WaitForSeconds(0.2f);
            count++;
        }
        animator.speed = 1;
        StartCoroutine(SkillCool2(skill_Cool2));
        isSkill = false;
    }

    IEnumerator GBLastSkill()
    {
        isSkill = true;
        animator.Play("Skill");
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlaySound("goblinBossSkill", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        enemyHealth.enemyParticle[5].Play();
        float effSize = 1;
        while (effSize < 1.75f)
        {
            enemyHealth.enemyParticle[5].gameObject.transform.GetChild(0).localScale = new Vector3(effSize, effSize);
            effSize += 0.01f;
            yield return null;
        }
        animator.Play("SkillEnd");
        StartCoroutine(SkillCoolLast(lastSkill_Cool));
        isSkill = false;
    }

    public void CheckEff(int count)
    {
        StartCoroutine(MoveEff(count));
        if (count >= 5)
        {
            count = 0;
            enemyHealth.enemyParticle[5].Stop();
            enemyHealth.enemyParticle[5].gameObject.transform.GetChild(0).localScale = new Vector2(1, 1);
            Vector2 pos = enemyHealth.enemyParticle[5].gameObject.transform.localPosition;
            if (spriteRenderer.flipX == true)
                enemyHealth.enemyParticle[6].gameObject.transform.localPosition = new Vector2(-Mathf.Abs(pos.x), pos.y);
            else
                enemyHealth.enemyParticle[6].gameObject.transform.localPosition = new Vector2(Mathf.Abs(pos.x), pos.y);

            enemyHealth.enemyParticle[6].Play();
            Collider2D[] collider2D = Physics2D.OverlapBoxAll(attackPos4.position, boxSize4, 0);
            foreach (Collider2D collider in collider2D)
            {
                if (collider.tag == "Player")
                {
                    PlayerHealth pH = collider.GetComponent<PlayerHealth>();
                    pH.TakeDamage(gameObject, stat.Attack * 2);
                    StartCoroutine(pH.Stun(2f));
                    StartCoroutine(pH.GetComponent<CameraShake>().CoDamageShake(1,10,1));
                    AudioManager.Instance.PlaySound("LastSkill", Camera.main.transform.position);
                }
            }
        }
    }

    IEnumerator MoveEff(int count)
    {
        Vector2 effPos = particlePos[count];
        Vector2 prevPos = enemyHealth.enemyParticle[5].transform.localPosition;
        if (spriteRenderer.flipX)
            effPos = new Vector2(-effPos.x, effPos.y);
        else
            effPos = new Vector2(Mathf.Abs(effPos.x), effPos.y);
        //if (count == 1)
        //    animator.speed = 0;
        float dist = 0;
        
        while (true)
        {
            enemyHealth.enemyParticle[5].transform.localPosition = Vector2.Lerp(prevPos, effPos, 1);
            dist = (effPos - prevPos).magnitude;
            Debug.Log(prevPos + " / " + effPos + " = " + dist);
            if (Mathf.Abs(dist) <= 1.5f)
                break;
            yield return null;
        }
    }

    IEnumerator WBSkill()
    {
        AudioManager.Instance.PlaySound("WizardSkill1", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        Vector2 TPpos = new Vector2(transform.position.x, transform.position.y);
        StartCoroutine(CoWBSkill());
        tpTionRoot = Random.Range(0, 3);
        if (tpTionRoot == 0)
        {
            transform.position = tpPos[0].transform.position;
        }
        else if (tpTionRoot == 1)
        {
            transform.position = tpPos[1].transform.position;
        }
        if (tpTionRoot == 2)
        {
            transform.position = tpPos[2].transform.position;
        }
        yield return new WaitForSeconds(skill_Cool);
        coSkill = null;
    }

    IEnumerator WBSkill2()
    {
        AudioManager.Instance.PlaySound("FireBallExplosion", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        StartCoroutine(CoWBSkill2());
        yield return new WaitForSeconds(skill_Cool2);
        coSkill2 = null;
    }

    IEnumerator WBSkill3()
    {
        Instantiate(fireBall, transform.position, Quaternion.identity);
        StartCoroutine(CoWBSkill3());
        yield return new WaitForSeconds(skill_Cool3);
        coSkill3 = null;
    }

    IEnumerator CoWBSkill()
    {
        enemyHealth.enemyParticle[1].gameObject.SetActive(true);
        enemyHealth.enemyParticle[1].Play();
        yield return new WaitForSeconds(1f);
    }

    IEnumerator CoWBSkill2()
    {
        for (int i = 0; i < 6; i++)
        {
            firetrp[i].gameObject.SetActive(true);
        }
        AudioManager.Instance.PlaySound("FlameBlastLoop", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        yield return new WaitForSeconds(6f);
        for (int i = 0; i < 6; i++)
        {
            firetrp[i].gameObject.SetActive(false);
        }
    }
    IEnumerator CoWBSkill3()
    {
        if (transform.localScale.x < 0)
        {
            fireBall.GetComponent<FireBall>().x = false;
        }
        else
        {
            fireBall.GetComponent<FireBall>().x = true;
        }

        yield return new WaitForSeconds(3f);
    }

    IEnumerator CoWaiting()
    {
        yield return new WaitForSeconds(waitingLength);
        coWaiting = null;

    }

    IEnumerator SkillCool(float skillCoolTime)
    {
        while (skillCoolTime > 0)
        {
            yield return new WaitForSeconds(1);
            //Debug.Log("스킬 1 " + skillCoolTime);
            skillCoolTime -= 1;
        }

        coSkill = null;
    }

    IEnumerator SkillCool2(float skillCoolTime)
    {
        while (skillCoolTime > 0)
        {
            yield return new WaitForSeconds(1);
            //Debug.Log("스킬 2 " + skillCoolTime);
            skillCoolTime -= 1;
        }

        coSkill2 = null;
    }

    IEnumerator SkillCoolLast(float skillCoolTime)
    {
        while (skillCoolTime > 0)
        {
            yield return new WaitForSeconds(1);
            //Debug.Log("스킬 Last " + skillCoolTime);
            skillCoolTime -= 1;
        }

        coLastSkill = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        if (attackPos != null)
            Gizmos.DrawWireCube(attackPos.position, boxSize);
        Gizmos.color = Color.red;
        if(attackPos2 != null)
        Gizmos.DrawWireCube(attackPos2.position, boxSize2);
        Gizmos.color = Color.blue;
        if (attackPos3 != null)
            Gizmos.DrawWireCube(attackPos3.position, boxSize3);
        Gizmos.color = Color.yellow;
        if (attackPos4 != null)
            Gizmos.DrawWireCube(attackPos4.position, boxSize4);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
