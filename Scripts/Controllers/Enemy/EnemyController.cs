using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;
using static Enums;

public class EnemyController : BaseController
{
    protected BaseHealth baseHealth;
    protected SpriteRenderer spriteRenderer;
    protected int flipX;
    protected Stat stat;

    [SerializeField]
    protected GameObject Tracing;
    [SerializeField]
    protected GameObject traceTarget;
    [SerializeField]
    protected bool isTracing = false;

    [SerializeField]
    protected bool isAttackPossible = false;
    public bool isAttack;
    protected bool isRun = false;
    protected int movementFlag = 0;

    protected bool isAttackFalse;
    public bool enemyAttacKing = false;

    // 공격 콜라이더
    [SerializeField]
    protected Transform attackPos;
    protected float posX;
    public Vector2 boxSize;

    // 이동
    protected Coroutine coMove;
    // 공격
    public IEnumerator coAttack;

    protected override void Awake()
    {
        base.Awake();
        baseHealth = GetComponent<BaseHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        stat = GetComponent<Stat>();
        posX = attackPos.localPosition.x;
    }

    protected virtual void OnEnable()
    {
        coAttack = null;
        isAttack = false;
        coMove = null;
        coMove = StartCoroutine(EnemyMovement());
    }

    protected virtual void Start()
    {
        coMove = StartCoroutine(EnemyMovement());
    }

    protected virtual void Update()
    {
        if (baseHealth.isDeath == true)
            return;

        if (spriteRenderer.flipX == true)
            flipX = 1;
        else
            flipX = -1;

        if (isAttack)
            return;

        if (GetComponent<EnemyHealth>().isSkilld == true)
            return;

        if (GetComponent<EnemyHealth>().isStun)
            return;

        Stop();
        Move();
        Attack();
        Animation();
    }

    protected virtual void Animation()
    {
        if (isRun)
        {
            if ((stat.MonsterType != 0 && movementFlag != 0) || isTracing == true)
                animator.SetBool("EnemyRun", true);
            else
                animator.SetBool("EnemyRun", false);
        }
        else
            animator.SetBool("EnemyRun", false);
    }


    protected virtual void Stop()
    {
        if (traceTarget == null)
        {
            isRun = true;
            return;
        }

        Vector3 playerPos = traceTarget.transform.position;
        if (playerPos.x <= transform.position.x)
            spriteRenderer.flipX = true;
        else if (playerPos.x > transform.position.x)
            spriteRenderer.flipX = false;

        BoxPos();

        Vector3 targetPos = traceTarget.transform.position - transform.position;
        float dist = Mathf.Round(targetPos.magnitude*10) *0.1f;
        if (dist <= stat.AttackRange)
        {
            rigid.velocity = Vector2.zero;
            isRun = false;
            isAttackPossible = true;
        }
        else
        {
            isRun = true;
            isAttackPossible = false;
        }
    }

    protected virtual void Attack()
    {
        if (traceTarget == null)
            return;
        if (traceTarget.GetComponent<PlayerController>().isCinema)
            return;
        if (traceTarget.GetComponent<BaseHealth>().isDeath == true)
            return;
        if (traceTarget.GetComponent<PlayerController>().isDash == true)
            return;

        if (isAttackPossible == false)
            return;

        if (coAttack != null)
            return;

        if (stat.MonsterType == Enums.MonsterType.Small)
        {
            coAttack = CoAttack();
            StartCoroutine(coAttack);
        }
        else if (stat.MonsterType >= Enums.MonsterType.Normal)
        {
            int attackType = Random.Range(0, stat.AttackClass);

            if (attackType == 0)
            {
                coAttack = CoAttack();
                StartCoroutine(coAttack);
            }
            else
            {
                coAttack = CoAttack_Strong();
                StartCoroutine(coAttack);
            }
        }
    }

    protected virtual IEnumerator CoAttack()
    {
        isAttack = true;
        animator.SetTrigger("EnemyAttack");
        
        if (name.Equals("Worf"))
            AudioManager.Instance.PlaySound("WorfAttack", transform.position, 1f + Random.Range(-0.1f, 0.1f));

        yield return new WaitUntil(() => enemyAttacKing);
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
                    EnemyHealth eh = GetComponent<EnemyHealth>();
                    StartCoroutine(eh.Stun(1));
                    AudioManager.Instance.PlaySound("BlockMetalMetal05", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                }
                else
                {
                    if (name.Equals("Goblin") || name.Equals("RedGoblin"))
                        AudioManager.Instance.PlaySound("HitSword07", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                    else if (name.Equals("Bat"))
                        AudioManager.Instance.PlaySound("BatAttack", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                    else if (name.Equals("Mushroom"))
                        AudioManager.Instance.PlaySound("MushroomAttack1", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                    collider.GetComponent<PlayerHealth>().TakeDamage(gameObject, stat.Attack);
                }
                collider.GetComponent<PlayerHealth>().TakeKnockback(gameObject, 3, 0);
            }
        }
        yield return new WaitForSeconds(stat.AttackSpeed);
        isAttack = false;
        coAttack = null;
    }
    protected virtual IEnumerator CoAttack_Strong()
    {
        isAttack = true;
        animator.SetTrigger("EnemyAttack2");

        if (name.Equals("Worf"))
            AudioManager.Instance.PlaySound("WorfAttack2", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        yield return new WaitUntil(() => enemyAttacKing);

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
                    EnemyHealth eh = GetComponent<EnemyHealth>();
                    StartCoroutine(eh.Stun(1.25f));
                    AudioManager.Instance.PlaySound("BlockMetalMetal05", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                }
                else
                {
                    if (name.Equals("Mushroom"))
                        AudioManager.Instance.PlaySound("MushroomAttack2", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                    else if (name.Equals("RedGoblin"))
                        AudioManager.Instance.PlaySound("HitSword07", transform.position, 1f + Random.Range(-0.1f, 0.1f));

                    collider.GetComponent<PlayerHealth>().TakeDamage(gameObject, stat.Attack * 1.5f);
                }
                collider.GetComponent<PlayerHealth>().TakeKnockback(gameObject, 5, 0);
            }
        }
        yield return new WaitForSeconds(stat.AttackSpeed * 1.5f);
        isAttack = false;
        coAttack = null;
    }

    protected virtual IEnumerator IsAttackFalse()
    {
        yield return new WaitForSeconds(1.5f);
        //Debug.Log("공격 실패 끝");
        isAttackFalse = false;
    }

    protected void EnemyAttackTrue()
    {
        //Debug.Log("공격");
        enemyAttacKing = true;
    }

    protected void EnemyAttackFalse()
    {
        //Debug.Log("성공");
        enemyAttacKing = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        if (attackPos != null)
            Gizmos.DrawWireCube(attackPos.position, boxSize);
    }

    protected virtual void Move()
    {
        if (!isRun)
            return;

        Vector3 moveVelocity = Vector2.zero;

        string dist = "";

        if (isTracing)
        {
            Vector3 playerPos = traceTarget.transform.position;
            if (playerPos.x <= transform.position.x)
                dist = "Left";
            else if (playerPos.x > transform.position.x)
                dist = "Right";
        }
        else
        {
            if (movementFlag == -1)
                dist = "Left";
            else if (movementFlag == 1)
                dist = "Right";
        }

        if (dist == "Left")
        {
            moveVelocity = Vector2.left;
            spriteRenderer.flipX = true;
        }
        else if (dist == "Right")
        {
            moveVelocity = Vector2.right;
            spriteRenderer.flipX = false;
        }

        BoxPos();

        if (baseHealth.isdamage == false && isAttackFalse == false)
        {
            rigid.velocity = new Vector2(moveVelocity.x * stat.MoveSpeed, rigid.velocity.y);
        }
        Vector2 frontVec = new Vector2(transform.position.x + movementFlag * 0.2f, transform.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(1, 0, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 5f, LayerMask.GetMask("Tile"));
        {
            if (rayHit.collider == null)
            {
                movementFlag *= -1;
                if(coMove != null)
                    StopCoroutine(coMove);
                coMove = null;
                coMove = StartCoroutine(EnemyMovement());
            }
        }
    }


    protected virtual void BoxPos()
    {
        if (spriteRenderer.flipX == true)
        {
            attackPos.localPosition = new Vector3(-posX, attackPos.localPosition.y, 0);
        }
        else
        {
            attackPos.localPosition = new Vector3(posX, attackPos.localPosition.y, 0);
        }
    }

    protected virtual IEnumerator EnemyMovement()
    {
        movementFlag = Random.Range(-1, 2);

        yield return new WaitForSeconds(3f);

        coMove = null;

        if (coMove == null)
            coMove = StartCoroutine(EnemyMovement());
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.GetComponent<PlayerController>().isCinema)
                return;

            traceTarget = collision.gameObject;
            isTracing = true;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag.Contains("Player"))
        {
            if (collision.GetComponent<PlayerController>().isCinema && traceTarget != null)
            {
                traceTarget = null;
                isTracing = false;
                coMove = null;

                if (gameObject.activeSelf == false)
                    return;

                if (baseHealth.isDeath == false && coMove == null)
                    coMove = StartCoroutine(EnemyMovement());
            }
            else
            {
                traceTarget = collision.gameObject;
                isTracing = true;
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.GetComponent<PlayerController>().isCinema)
                return;

            traceTarget = null;
            isTracing = false;
            coMove = null;

            if (gameObject.activeSelf == false)
                return;

            if (baseHealth.isDeath == false && coMove == null)
                coMove = StartCoroutine(EnemyMovement());
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
