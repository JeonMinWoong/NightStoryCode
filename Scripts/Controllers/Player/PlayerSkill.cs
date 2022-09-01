using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    public float skillDamage;
    public bool isCritical;

    public GameObject player;

    public void TakeSkillDamage(float _Damage , bool _critical)
    {
        skillDamage = _Damage;
        isCritical = _critical;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy" )
        {
            EnemyHealth eH = collision.GetComponent<EnemyHealth>();
            if (!eH.isSkillHit)
            {
                eH.SkillHit();
                eH.TakeDamage(player, skillDamage, isCritical);
                player.GetComponent<PlayerController>().PosionCheck(eH);
            }
        }

    }
    
}
