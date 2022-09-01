using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

using static Enums;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PlayerController : BaseController
{
    PlayerStat playerStat;
    PlayerHealth playerH;
    CapsuleCollider2D capCol;
    // Update is called once per frame
    [SerializeField]
    private float runspeed = 6f; //스피드
    private float jump = 15;
    public int jumpCount;
    int dashCount;
    public bool isDefense { get; set; }
    public bool skillUse { get; set; }
    public bool isDash { get; set; }
    public bool isAttack { get; set; }
    public bool isAttack_Strong { get; set; }
    private int skillLevel;
    private int skillUseEnergy;
    private int skillDamage;

    GameObject eventOJ;
    bool isEvent;
    public bool isStop { get; set; }
    
    // 인트로
    public Coroutine coIntro;
    // 일반 공격
    Coroutine coAttack;
    Coroutine coAttack_Strong;
    // 대기 카운트
    float idleTime;

    Coroutine coJump;
    // 대쉬
    Coroutine coDash;
    // 막기
    Coroutine coDefense;

    // 스킬
    Coroutine coSkill_Q;
    GameObject skillButton;

    Coroutine coSkill_E;
    GameObject skillButton2;

    Coroutine coSkill_R;
    GameObject skillButton3;

    public bool isCinema;
    public bool isJumpCheck;
    protected override void Awake()
    {
        base.Awake();
        playerStat = GetComponent<PlayerStat>();
        capCol = GetComponent<CapsuleCollider2D>();
        playerH = GetComponent<PlayerHealth>();
        playerStat.playerAbility.level = 1;
        skillButton = GameObject.Find("Skill_1_Slot");
        skillButton2 = GameObject.Find("Skill_2_Slot");
        skillButton3 = GameObject.Find("Skill_3_Slot");
    }

    private void OnEnable()
    {
        animator.Play("Idle");
        isAttack = false;
        isAttack_Strong = false;
        skillUse = false;
        coAttack = null;
        coAttack_Strong = null;
        coSkill_Q = null;
        coSkill_E = null;
        coSkill_R = null;
    }

    void Cinema()
    {
        animator.SetBool("IsFall", false);
        animator.SetBool("IsJump", false);
        animator.SetBool("IsRun", false);
        animator.Play("Idle");
    }

    public void ControllerInit()
    {
        if (SceneManager.GetActiveScene().name.Equals("MapSelect") == false)
        {
            if (playerStat.playerAbility.mapInpo == 0)
                StartCoroutine(GameController.Instance.Tutorial_Stage());
            else if(!playerStat.playerAbility.stageInpo[GameController.Instance.lvCount - 1].stageInpo[GameController.Instance.StageCount - 1])
                StageIntro();
        }
    }

    public void StageIntro()
    {
        coIntro = StartCoroutine(CoStageIntro());
    }

    public void PrevStageIntro()
    {
        coIntro = StartCoroutine(CoPrevStageIntro());
    }

    private void Update()
    {
        Interaction();

        if (isCinema)
        {
            Cinema();
            return;
        }

        if (isStop)
            return;

        if (playerH.isStun)
            return;

        Potion();

        if (skillUse)
            return;
        if (isAttack_Strong)
            return;
        if (isAttack)
            return;

        Defense();
        SkillUse();
        Attack();
        Dash();
        Jump();
        Animate();
        Move();
        
        WindowController();
        
    }

    IEnumerator CoStageIntro()
    {
        animator.Rebind();
        isStop = true;
        animator.SetBool("IsRun", true);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        while (true)
        {
            transform.position += Vector3.right * runspeed * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator CoPrevStageIntro()
    {
        animator.Rebind();
        isStop = true;
        animator.SetBool("IsRun", true);
        transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        while (true)
        {
            transform.position += Vector3.left * runspeed * Time.deltaTime;
            yield return null;
        }
    }

    void Move()
    {
        if (isDash)
            return;

        if (isDefense)
            return;

        Vector3 moveVelocity = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
        {
            moveVelocity = Vector3.left;
            transform.localScale = new Vector3(-3, 3);

        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveVelocity = Vector3.right;
            transform.localScale = new Vector3(3, 3);
        }

        transform.position += moveVelocity * runspeed * Time.deltaTime;
    }

    void Dash()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            if (playerStat.CurrentStamina < 5)
            {
                playerH.StminaLack();
                return;
            }
            if (isDefense == true)
                return;
            if (coDash != null)
                return;

            int posX = 0;

            if (transform.localScale.x < 0)
                posX = 1;
            else
                posX = -1;

            isDash = true;
            coDash = StartCoroutine(Dash(posX));
        }
    }

    IEnumerator Dash(int length)
    {
        Vector3 Dashpos = new Vector3(transform.position.x, transform.position.y);
        Debug.DrawRay(Dashpos, Vector2.right * -2f * length, new Color(1, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(Dashpos, Vector2.right * -2f * length, 1.5f, LayerMask.GetMask("Tile"));
        if (rayHit.collider == null)
        {
            Vector3 dashVector = new Vector2(-25f * length, 0);
            if (length > 0)
            {
                playerH.playerParticle[1].gameObject.SetActive(true);
                playerH.playerParticle[1].Play();
            }
            else
            {
                playerH.playerParticle[0].gameObject.SetActive(true);
                playerH.playerParticle[0].Play();
            }
            animator.SetTrigger("Dash");
            AudioManager.Instance.PlaySound("SwingSmall03", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            rigid.velocity = new Vector2(dashVector.x, 0);
            Physics2D.IgnoreLayerCollision(9, 10, true);
        }

        if(playerStat.dashTime != 0)
        {
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(dashPos.position, boxDash, 0);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider.tag == "Enemy")
                {
                    EnemyHealth eH = collider.GetComponent<EnemyHealth>();
                    float dashDamage = playerStat.playerAbility.attack * (playerStat.dashTime * 0.5f * 0.01f);
                    eH.TakeDamage(gameObject,0, dash:dashDamage);
                    PosionCheck(eH);
                }
            }
        }


        playerH.TakeStaMina(5);
        yield return new WaitForSeconds(0.15f);
        Physics2D.IgnoreLayerCollision(9, 10, false);
        rigid.velocity = Vector2.zero;
        isDash = false;
        float dashCool = playerStat.dashTime * 0.02f;
        if(playerStat.dashTime >= 30 && dashCount < 1)
        {
            dashCount++;
            yield return new WaitForSeconds(0.1f);
            //Debug.Log("이중대쉬");
            coDash = null;
            yield break;
        }
        else
        {
            
            yield return new WaitForSeconds(2f - dashCool);
            //Debug.Log("기본대쉬");
            coDash = null;
            dashCount = 0;
        }
    }

    void Jump()
    {
        if (((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && jumpCount < 2))
        {
            if (isDash == true)
                return;

            rigid.velocity = Vector2.zero;

            Vector2 jumpVelocity = new Vector2(0, jump);
            rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);
            //if (jumpCount >= 1)
            //{
            //    playerH.playerParticle[12].gameObject.SetActive(true) ;
            //    playerH.playerParticle[12].Play();
            //}
            jumpCount++;
            rigid.gravityScale = 1;

            if (coJump == null)
                coJump = StartCoroutine(CoJumpUp());

            AudioManager.Instance.PlaySound("BodyFallSmall03", pos.position, 1f + Random.Range(-0.1f, 0.1f));

        }
    }

    IEnumerator CoJumpUp()
    {
        float upValue = 0;
        Vector2 upPlue = Vector2.zero;
        while (true)
        {
            if (rigid.velocity.y > 0)
            {
                upValue -= Time.deltaTime * Mathf.Lerp(70,100,1);
                upPlue = new Vector2(0, upValue);
                rigid.AddForce(upPlue, ForceMode2D.Force);
            }
            else
            {
                coJump = null;
                break;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    void Defense()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if(playerStat.CurrentStamina < 7.5f)
            {
                playerH.StminaLack();
                return;
            }
            if (coDefense != null)
                return;

            playerH.TakeStaMina(7.5f);
            isDefense = true;
            animator.SetBool("IsDefense", true);
            coDefense = StartCoroutine(CoDefense());
        }
    }

    IEnumerator CoDefense()
    {
        while (true)
        {
            yield return null;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Defense") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                isDefense = false;
                animator.SetBool("IsDefense", false);
                yield return new WaitForSeconds(1);
                break;
            }
        }

        coDefense = null;
    }

    public Transform pos;
    public Vector2 boxSize;
    public Transform pos2;
    public Vector2 boxSize2;
    public Transform pos3;
    public Vector2 boxSize3;
    public Transform dashPos;
    public Vector2 boxDash;
    private void Attack()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true)
            return;

        if (Input.GetButtonDown("Fire1"))
        {
            if (playerStat.CurrentStamina < 3)
            {
                playerH.StminaLack();
                return;
            }
            if (isDefense == true)
                return;
            if (coAttack != null)
                return;
            coAttack = StartCoroutine(CoAttack());

        }
        else if (Input.GetButtonDown("Fire2"))
        {
            if (playerStat.CurrentStamina < 5)
            {
                playerH.StminaLack();
                return;
            }
            if (isDefense == true)
                return;
            if (coAttack_Strong != null)
                return;
            coAttack_Strong = StartCoroutine(CoAttack_Strong());
        }
    }
    IEnumerator CoAttack()
    {
        playerH.TakeStaMina(3);
        animator.SetTrigger("Attack");
        isAttack = true;
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.tag == "Enemy")
            {
                AudioManager.Instance.PlaySound("BlockShieldMetal01", pos.position, 1f + Random.Range(-0.1f, 0.1f));
                EnemyHealth eH = collider.GetComponent<EnemyHealth>();
                eH.TakeDamage(gameObject, playerStat.playerAbility.attack, CriticalPersent());
                eH.TakeKnockback(gameObject,10, 10);
                PosionCheck(eH);
                playerH.GiveSkill(3);
            }
            else
            {
                AudioManager.Instance.PlaySound("HitSword01", pos.position, 1f + Random.Range(-0.1f, 0.1f));
            }
        }

        while(true)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
            {
                isAttack = false;
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(playerStat.playerAbility.attackSpeed);
        coAttack = null;
    }
    IEnumerator CoAttack_Strong()
    {
        isAttack_Strong = true;
        playerH.TakeStaMina(5);
        animator.SetTrigger("Attack2");
        Collider2D[] collider2D2s = Physics2D.OverlapBoxAll(pos2.position, boxSize2, 0);
        foreach (Collider2D collider in collider2D2s)
        {
            if (collider.tag == "Enemy")
            {
                AudioManager.Instance.PlaySound("BlockShieldMetal02", pos.position, 1f + Random.Range(-0.1f, 0.1f));
                EnemyHealth eH = collider.GetComponent<EnemyHealth>();
                eH.TakeDamage(gameObject, playerStat.playerAbility.attack * 1.5f, CriticalPersent());
                eH.TakeKnockback(gameObject,15, 15);
                PosionCheck(eH);
                playerH.GiveSkill(5);
            }
            else
            {
                AudioManager.Instance.PlaySound("HitSword02", pos.position, 1f + Random.Range(-0.1f, 0.1f));
            }
        }

        while (true)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
            {
                isAttack_Strong = false;
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(playerStat.playerAbility.attackSpeed * 1.5f);
        coAttack_Strong = null;
        

    }

    public void IsAttackEnd()
    {
        isAttack = false;
    }

    public void IsAttackStrongEnd()
    {
        isAttack_Strong = false;
    }

    public void PosionCheck(EnemyHealth eH)
    {
        if (playerStat.poisonDamage != 0)
            eH.Posion(gameObject, playerStat.poisonDamage);
    }

    public GameObject skill2Effect;

    void SkillUse()
    {
        if (Input.GetKeyDown(KeyCode.Q) && skillButton.transform.GetChild(0).GetComponent<Image>().sprite != null)
        {
            if (playerStat.CurrentSkillGage < playerStat.equipSkills[0].useEnergy)
                return;

            if (isDefense == true)
                return;

            if (coSkill_Q != null)
                return;

            int posX = 0;

            if (transform.localScale.x < 0)
                posX = 1;
            else
                posX = -1;

            coSkill_Q = StartCoroutine(SkillUsing(posX));
        }
        if (Input.GetKeyDown(KeyCode.E) && skillButton2.transform.GetChild(0).GetComponent<Image>().sprite != null)
        {
            if (playerStat.CurrentSkillGage < playerStat.equipSkills[1].useEnergy)
                return;
            if (isDefense == true)
                return;
            if (coSkill_E != null)
                return;

            AudioManager.Instance.PlaySound("Skill2Start", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            animator.SetTrigger("Skill2");
            playerH.TakeSkill(playerStat.equipSkills[1].useEnergy);
            skill2Effect.GetComponent<PlayerSkill>().TakeSkillDamage(playerStat.equipSkills[1].fullDamage, CriticalPersent());

            int posX = 0;

            if (transform.localScale.x < 0)
                posX = 1;
            else
                posX = -1;

            coSkill_E = StartCoroutine(SkillUsing2(posX));
        }
        if (Input.GetKeyDown(KeyCode.R) && skillButton3.transform.GetChild(0).GetComponent<Image>().sprite != null)
        {
            if (playerStat.CurrentSkillGage < playerStat.equipSkills[2].useEnergy)
                return;
            if (isDefense == true)
                return;
            if (coSkill_R != null)
                return;

            playerH.TakeSkill(playerStat.equipSkills[2].useEnergy);

            coSkill_R = StartCoroutine(SkillUsing3());
        }
    }
    IEnumerator SkillUsing(int length)
    {
        Vector2 skillpos = new Vector2(transform.position.x, transform.position.y - 0.5f);
        Debug.DrawRay(skillpos, Vector2.right * -5f * length, new Color(1, 1, 0));
        RaycastHit2D[] rayHit = Physics2D.RaycastAll(skillpos, Vector2.right * -5f * length, 5, LayerMask.GetMask("Enemy", "Tile"));
        Vector2 tilePos = Vector2.zero;
        for (int i = 0; i < rayHit.Length; i++)
        {
            if (rayHit[i].collider.name.Contains("Tile"))
            {
                int x, y;
                x = rayHit[i].collider.GetComponent<Tilemap>().WorldToCell(rayHit[i].point).x;
                y = rayHit[i].collider.GetComponent<Tilemap>().WorldToCell(rayHit[i].point).y;
                tilePos = new Vector2(x, y);
            }
        }

        Vector2 rePos = tilePos - skillpos;
        Vector2 skillVector = new Vector2(-Mathf.Abs(rePos.x) * length, 0);
        Debug.Log(tilePos + " / " + skillVector);

        skillUse = true;
        animator.Play("Skillmotion");
        playerH.playerParticle[10].Play();
        rigid.isKinematic = true;
        rigid.simulated = false;
        yield return new WaitForSeconds(0.3f);
        playerH.playerParticle[9].Play();
        if (length > 0)
            playerH.playerParticle[9].transform.localScale = new Vector2(-Mathf.Abs(playerH.playerParticle[9].transform.localScale.x), playerH.playerParticle[9].transform.localScale.y);
        else
            playerH.playerParticle[9].transform.localScale = new Vector2(Mathf.Abs(playerH.playerParticle[9].transform.localScale.x), playerH.playerParticle[9].transform.localScale.y);
        playerH.TakeSkill(playerStat.equipSkills[0].useEnergy);
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(pos3.position, boxSize3, 0);
        foreach (var hit in hitColliders)
        {
            if (hit.tag == "Enemy")
            {
                rigid.velocity = Vector2.zero;
                Vector2 skillUsepos = new Vector2(transform.position.x - 2f * length, transform.position.y + 0.2f);
                AudioManager.Instance.PlaySound("SkillStart", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                AudioManager.Instance.PlaySound("SkillUse", pos.position, 1f + Random.Range(-0.1f, 0.1f));
                GameController.Instance.SkillUse();
                EnemyHealth eH = hit.gameObject.GetComponent<EnemyHealth>();
                eH.TakeDamage(gameObject, playerStat.equipSkills[0].fullDamage, CriticalPersent());
                PosionCheck(eH);
                if (!eH.isDeath)
                    eH.StartCoroutine(eH.IsSkilled(0));
            }
        }
        if (tilePos.x != 0)
        {
            if ((skillVector.x < -2 && skillVector.x < 0f) || skillVector.x > 2)
                transform.Translate(skillVector);
        }
        else
        {
            skillVector = new Vector2(-5f * length, 0);
            transform.Translate(skillVector);
        }
        animator.Play("SkillmotionEnd");
        yield return new WaitForSeconds(0.7f);
        AudioManager.Instance.PlaySound("Skill", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        StartCoroutine(SkillCool());
        animator.Play("Idle");
        rigid.isKinematic = false;
        rigid.simulated = true;
        skillUse = false;
    }

    IEnumerator SkillCool()
    {
        Image sImage = skillButton.transform.GetChild(0).GetComponent<Image>();
        Image sDownImage = sImage.transform.GetChild(0).GetComponent<Image>();
        Text sText = sDownImage.transform.GetChild(0).GetComponent<Text>();
        float coolTime = playerStat.equipSkills[0].coolTime;


        //sImage.color = new Color(0.1f, 0.1f, 0.1f);
        sDownImage.transform.gameObject.SetActive(true);
        sDownImage.fillAmount = 1;
        while (coolTime > 0)
        {
            sText.text = coolTime.ToString("0.0");
            yield return new WaitForEndOfFrame();
            sDownImage.fillAmount = coolTime / playerStat.equipSkills[0].coolTime;
            coolTime -= Time.deltaTime;
        }
        //sImage.color = new Color(1f, 1f, 1f);
        sDownImage.gameObject.SetActive(false);
        coSkill_Q = null;
    }
    IEnumerator SkillUsing2(int length)
    {
        skillUse = true;
        yield return new WaitForSeconds(0.3f);
        Vector2 usePos = new Vector2(transform.position.x, transform.position.y);
        for (int i = 1; i <= 10; i++)
        {
            Vector2 skillUsepos2 = new Vector2(usePos.x - (1 * i) * length, usePos.y + 4.5f);
            GameObject skill2Effect2 = Instantiate(skill2Effect, skillUsepos2, Quaternion.identity);
            skill2Effect2.GetComponent<PlayerSkill>().player = gameObject;
            skill2Effect2.GetComponent<Animator>().SetTrigger("Skill2Effect");
            AudioManager.Instance.PlaySound("Skill2", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            Destroy(skill2Effect2, 1);
            yield return new WaitForSeconds(0.05f);
        }
        skillUse = false;
        StartCoroutine(SkillCool2());
        GetComponent<CameraShake>().ShakeCamera();
    }
    IEnumerator SkillCool2()
    {
        Image sImage = skillButton2.transform.GetChild(0).GetComponent<Image>();
        Image sDownImage = sImage.transform.GetChild(0).GetComponent<Image>();
        Text sText = sDownImage.transform.GetChild(0).GetComponent<Text>();
        float coolTime = playerStat.equipSkills[1].coolTime;

        //sImage.color = new Color(0.1f, 0.1f, 0.1f);
        sDownImage.gameObject.SetActive(true);
        sDownImage.fillAmount = 1;
        while (coolTime > 0)
        {
            sText.text = coolTime.ToString("0.0");
            yield return new WaitForEndOfFrame();
            sDownImage.fillAmount = coolTime / playerStat.equipSkills[1].coolTime;
            coolTime -= Time.deltaTime;
        }
        //sImage.color = new Color(1f, 1f, 1f);
        sDownImage.gameObject.SetActive(false);
        coSkill_E = null;
    }

    IEnumerator SkillUsing3()
    {
        int skillLv = playerStat.equipSkills[2].skillLevel;
        StartCoroutine(SkillCool3(skillLv * 2 + playerStat.equipSkills[2].duration ));
        playerH.playerParticle[11].Play();
        playerStat.playerAbility.attack += skillLv * 10;
        playerStat.playerAbility.attackSpeed -= skillLv * 0.15f;
        playerStat.SetAttackAnimSpeed(skillLv * 0.15f);
        playerStat.playerAbility.criticalPercent += skillLv * 8f;
        runspeed += runspeed * 0.4f;
        playerStat.Defense -= (int)(playerStat.Defense * 0.5f);
        float timer = 0;
        while (timer <= playerStat.equipSkills[2].duration + skillLv * 2)
        {
            playerStat.CurrentSkillGage = 0;
            playerStat.CurrentStamina = playerStat.MaxStamina;
            timer += Time.deltaTime;
            yield return null;
        }
        playerStat.playerAbility.attack -= skillLv * 10;
        playerStat.playerAbility.attackSpeed += skillLv * 0.15f;
        playerStat.SetAttackAnimSpeed(-skillLv * 0.15f);
        playerStat.playerAbility.criticalPercent -= skillLv * 8f;
        runspeed -= runspeed * 0.4f;
        playerStat.Defense += (int)(playerStat.Defense * 0.5f);
        playerH.playerParticle[11].Stop();
    }
    IEnumerator SkillCool3(float timer)
    {
        yield return new WaitForSeconds(timer);

        Image sImage = skillButton3.transform.GetChild(0).GetComponent<Image>();
        Image sDownImage = sImage.transform.GetChild(0).GetComponent<Image>();
        Text sText = sDownImage.transform.GetChild(0).GetComponent<Text>();
        float coolTime = playerStat.equipSkills[2].coolTime;

        sDownImage.gameObject.SetActive(true);
        sDownImage.fillAmount = 1;
        while (coolTime > 0)
        {
            sText.text = coolTime.ToString("0.0");
            yield return new WaitForEndOfFrame();
            sDownImage.fillAmount = coolTime / playerStat.equipSkills[2].coolTime;
            coolTime -= Time.deltaTime;
        }
        sDownImage.gameObject.SetActive(false);
        coSkill_R = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos.position, boxSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pos2.position, boxSize2);
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(pos3.position, boxSize3);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(dashPos.position, boxDash);
    }

    void RunTrue()
    {
        AudioManager.Instance.PlaySound("StepGrass01", pos.position, 1f + Random.Range(-0.1f, 0.1f));
    }

    void Runfalse()
    {
        AudioManager.Instance.PlaySound("StepGrass02", pos.position, 1f + Random.Range(-0.1f, 0.1f));
    }

    bool CriticalPersent()
    {
        float rand = Random.Range(0.1f, 100.9f);

        //Debug.Log(rand + " / " + playerStat.CriticalPercent);

        if (rand <= playerStat.CriticalPercent)
        {
            return true;
        }
        else
            return false;
    }

    void Animate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Run")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Fall") || animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            idleTime += Time.deltaTime;
            if (idleTime >= 5)
            {
                playerH.IdleStamina();
                idleTime = 4.95f;
            }
        }
        else
            idleTime = 0;

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            animator.SetBool("IsRun", false);
        }
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && jumpCount == 0)
        {
            animator.SetBool("IsRun", true);
        }

        if (rigid.velocity.y < -1f)
        {
            animator.SetBool("IsRun", false);
            animator.SetBool("IsJump", false);
            animator.SetBool("IsFall", true);
            if (rigid.gravityScale <= 3.5f)
                rigid.gravityScale += Time.deltaTime * 6;
        }
        // 점프하고 있는 경우
        else if (rigid.velocity.y > 0 && jumpCount > 0)
        {
            animator.SetBool("IsRun", false);
            animator.SetBool("IsJump", true);
            animator.SetBool("IsFall", false);
        }
        // 가만히 있는 경우
        else
        {
            animator.SetBool("IsJump", false);
            animator.SetBool("IsFall", false);
            rigid.gravityScale = 1;
        }
    }

    void Interaction()
    {
        if (isEvent == true)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {

                //Debug.Log("이벤트 작동");
                if (eventOJ == null)
                    return;

                if (eventOJ.name.Contains("Door"))
                {
                    eventOJ.GetComponent<StageSelect>().EventActive();
                    isStop = (isStop == true) ? false : true;
                }
                else if(eventOJ.name.Contains("EndGate"))
                {
                    if (GameController.Instance.isGameClear == true)
                    {
                        GameController.Instance.StartCoroutine(GameController.Instance.StageEnd());
                    }
                }
                else if(eventOJ.name.Contains("Memory"))
                {
                    if(!UIManager.Instance.playerMemory.gameObject.activeSelf)
                        playerStat.SaveButton();
                }

                StoryLog storyNpc = eventOJ.GetComponent<StoryLog>();

                if (storyNpc != null)
                {
                    UIManager.Instance.npcUI.OnNpcUI(eventOJ);
                    animator.SetBool("IsRun", false);
                }

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "NPC")
        {
            isEvent = true;
            eventOJ = collision.gameObject;
        }

        if (collision.tag == "RePoint")
        {
            if (coIntro != null)
                StopCoroutine(coIntro);

            coIntro = null;
            isStop = false;
            GameController.Instance.originPos = transform.position;
        }
        else if (collision.tag == "EndPoint")
        {
            if (coIntro != null)
                StopCoroutine(coIntro);

            //Debug.Log("이전");
            coIntro = null;
            isStop = false;
        }

        if (collision.gameObject.tag == "Tile" || collision.gameObject.tag == "Enemy")
        {
            if (!isJumpCheck)
            {
                isJumpCheck = true;
                StartCoroutine(CoJumpCheck());
                AudioManager.Instance.PlaySound("BodyFallSmall05", pos.position, 1f + Random.Range(-0.1f, 0.1f));
            }//rigid.velocity = new Vector2(rigid.velocity.x, 0);
            jumpCount = 0;
            return;
        }

    }

    IEnumerator CoJumpCheck()
    {
        yield return new WaitForSeconds(0.5f);
        isJumpCheck = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "NPC")
        {
            if (collision.GetComponent<Store>() != null)
            {
                UIManager.Instance.OutNPC();
            }
            isEvent = false;
            eventOJ = null;
            
        }
    }

    #region WindowController

    void WindowController()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            UIManager.Instance.ActiveWindow(0);
        else if (Input.GetKeyDown(KeyCode.I))
            UIManager.Instance.ActiveWindow(1);
        else if (Input.GetKeyDown(KeyCode.K))
            UIManager.Instance.ActiveWindow(2);
        else if (Input.GetKeyDown(KeyCode.J))
            UIManager.Instance.ActiveWindow(3);
        else if (Input.GetKeyDown(KeyCode.O))
            UIManager.Instance.ActiveWindow(5);
        else if (Input.GetKeyDown(KeyCode.M))
            GameController.Instance.InputMiniMap();
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.AllActiveFalseWindow(true);
        }
#if UNITY_EDITOR
        else if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            UIManager.Instance.QuestPlus(playerStat.playerAbility.playerQuests[1], 1);
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            UIManager.Instance.QuestPlus(playerStat.playerAbility.playerQuests[0], 1);
        }
        else if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            playerH.LevelUp();
        }
        else if(Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Inventory.Instance.NewItem("101", 5);
            Inventory.Instance.NewItem("103", 10);
        }
#endif
    }

    void Potion()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Inventory.Instance.ItemUse(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Inventory.Instance.ItemUse(1);
        }

    }
#endregion
}



