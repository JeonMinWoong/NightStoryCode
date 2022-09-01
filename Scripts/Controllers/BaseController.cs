using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    
    public IEnumerator coTest;
    
    protected Animator animator;
    protected Rigidbody2D rigid;


    protected int attackDamage;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

    }
}
