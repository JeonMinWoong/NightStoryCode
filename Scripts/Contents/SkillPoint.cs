using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPoint : MonoBehaviour
{
    Text pointText;
    public PlayerStat playerStat;

    public int HaveSkillPoint
    {
        get
        {
            return playerStat.SkillPoint;
        }
        set
        {
            playerStat.SkillPoint = value;
        }
    }

    private void Awake()
    {
        pointText = GetComponentInChildren<Text>();
        playerStat = FindObjectOfType<PlayerStat>();
        HaveSkillPoint = playerStat.SkillPoint;
    
    }

    private void Update()
    {
        HaveSkillPoint = playerStat.SkillPoint;
        pointText.text = HaveSkillPoint.ToString();
    }
}
