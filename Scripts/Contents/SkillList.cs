using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillList : MonoBehaviour
{
    public GameObject[] activeSlot;

    public GameObject[] passiveSlot;

    PlayerStat playerStat;
    public ParticleSystem[] particleSystems;
    public GameObject[] skillList;
    public GameObject passiveList;
    public GameObject talents;

    public SkillIcon[] equipSkills = new SkillIcon[6];

    private IEnumerator Start()
    {
        playerStat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStat>();
        playerStat.skillList = this;
        for (int i = 0; i < activeSlot.Length; i++)
        {
            activeSlot[i].GetComponent<Button>().interactable = false;
        }

        for (int i = 0; i < passiveSlot.Length; i++)
        {
            passiveSlot[i].GetComponent<Button>().interactable = false;
        }

        talents.gameObject.SetActive(true);
        for (int i = 0; i < skillList.Length; i++)
        {
            skillList[i].SetActive(true);
        }
        passiveList.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }

    public void SkillButton_On(int count)
    {
        AudioManager.Instance.PlaySound("SkillList", Camera.main.transform.position);

        talents.SetActive(true);
        for (int i = 0; i < skillList.Length; i++)
        {
            if (i == count)
                skillList[count].SetActive(true);
            else
                skillList[i].SetActive(false);
        }
        passiveList.SetActive(false);
    }

    public void PassiveButton_On(int count)
    {
        AudioManager.Instance.PlaySound("SkillList", Camera.main.transform.position);

        talents.SetActive(true);
        for (int i = 0; i < skillList.Length; i++)
        {
            skillList[i].SetActive(false);
        }
        passiveList.SetActive(true);
        SkillGroup skillGroup = passiveList.GetComponentInChildren<SkillGroup>();
        
        skillGroup.passiveCount = count;
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            PassiveCheck();
            ActiveCheck();
        }
    }

    public void SkillListEnd()
    {
        AudioManager.Instance.PlaySound("SkillListEnd", Camera.main.transform.position);
    }

    void PassiveCheck()
    {
        if (playerStat.playerAbility.level > 4 && passiveSlot[0].GetComponent<Button>().interactable == false)
        {
            passiveSlot[0].GetComponent<Button>().interactable = true;
            passiveSlot[0].transform.GetChild(1).gameObject.SetActive(false);
            passiveSlot[0].transform.GetChild(0).gameObject.SetActive(true);
            Sprite icon = passiveSlot[0].transform.GetChild(0).GetComponent<Image>().sprite;

            if(icon == null)
                particleSystems[1].gameObject.SetActive(true);
        }
        if (playerStat.playerAbility.level > 12 && passiveSlot[1].GetComponent<Button>().interactable == false)
        {
            passiveSlot[1].GetComponent<Button>().interactable = true;
            passiveSlot[1].transform.GetChild(1).gameObject.SetActive(false);
            passiveSlot[1].transform.GetChild(0).gameObject.SetActive(true);
            Sprite icon = passiveSlot[1].transform.GetChild(0).GetComponent<Image>().sprite;

            if (icon == null)
                particleSystems[4].gameObject.SetActive(true);
        }
        if (playerStat.playerAbility.level > 20 && passiveSlot[2].GetComponent<Button>().interactable == false)
        {
            passiveSlot[2].GetComponent<Button>().interactable = true;
            passiveSlot[2].transform.GetChild(1).gameObject.SetActive(false);
            passiveSlot[2].transform.GetChild(0).gameObject.SetActive(true);
            Sprite icon = passiveSlot[2].transform.GetChild(0).GetComponent<Image>().sprite;

            if (icon == null)
                particleSystems[5].gameObject.SetActive(true);
        }
    }

    void ActiveCheck()
    {
        if (playerStat.playerAbility.level > 1 && activeSlot[0].GetComponent<Button>().interactable == false)
        {
            activeSlot[0].GetComponent<Button>().interactable = true;
            activeSlot[0].transform.GetChild(1).gameObject.SetActive(false);
            activeSlot[0].transform.GetChild(0).gameObject.SetActive(true);
            Sprite icon = activeSlot[0].transform.GetChild(0).GetComponent<Image>().sprite;

            if (icon == null)
                particleSystems[0].gameObject.SetActive(true);
        }
        if (playerStat.playerAbility.level > 5 && activeSlot[1].GetComponent<Button>().interactable == false)
        {
            activeSlot[1].GetComponent<Button>().interactable = true;
            activeSlot[1].transform.GetChild(1).gameObject.SetActive(false);
            activeSlot[1].transform.GetChild(0).gameObject.SetActive(true);
            Sprite icon = activeSlot[1].transform.GetChild(0).GetComponent<Image>().sprite;

            if (icon == null)
                particleSystems[2].gameObject.SetActive(true);
        }
        if (playerStat.playerAbility.level > 11 && activeSlot[2].GetComponent<Button>().interactable == false)
        {
            activeSlot[2].GetComponent<Button>().interactable = true;
            activeSlot[2].transform.GetChild(1).gameObject.SetActive(false);
            activeSlot[2].transform.GetChild(0).gameObject.SetActive(true);
            Sprite icon = activeSlot[2].transform.GetChild(0).GetComponent<Image>().sprite;

            if (icon == null)
                particleSystems[3].gameObject.SetActive(true);
        }
    }

    public void SaveSkill()
    {
        for (int i = 0; i < playerStat.playerAbility.skillInpo.Length; i++)
        {
            playerStat.playerAbility.skillInpo[i].skillStats.Clear();

            if (i != 3)
            {
                SkillIcon[] skillIcons = skillList[i].GetComponentsInChildren<SkillIcon>(true);
                for (int j = 0; j < skillIcons.Length; j++)
                {
                    playerStat.playerAbility.skillInpo[i].skillStats.Add(skillIcons[j].skillStat);
                }
            }
            else
            {
                SkillIcon[] passives = passiveList.GetComponentsInChildren<SkillIcon>(true);
                for (int j = 0; j < passives.Length; j++)
                {
                    playerStat.playerAbility.skillInpo[i].skillStats.Add(passives[j].skillStat);
                }
            }
        }
    }

    public void LoadSkill()
    {
        GameObject loadOJ = new GameObject("Skill");
        loadOJ.SetActive(false);
        SkillStat loadSkill = loadOJ.AddComponent<SkillIcon>().skillStat;
       
        for (int i = 0; i < playerStat.playerAbility.skillInpo.Length; i++)
        {
            if (i != 3)
            {
                SkillIcon[] skillIcons = skillList[i].GetComponentsInChildren<SkillIcon>();
                SkillGroup skillGroup = skillList[i].GetComponentInChildren<SkillGroup>();
                for (int j = 0; j < playerStat.playerAbility.skillInpo[i].skillStats.Count; j++)
                {
                    
                    loadSkill = playerStat.playerAbility.skillInpo[i].skillStats[j];
                    skillIcons[j].skillStat = loadSkill;
                    skillIcons[j].LinkSkill();
                    if (loadSkill.isEquip)
                    {
                        playerStat.equipSkills[i] = loadSkill;
                        skillGroup.prevIcon = skillIcons[j];
                        skillGroup.currentIcon = skillIcons[j];
                        skillGroup.IconReset();
                    }
                }
            }
            else
            {
                SkillIcon[] passives = passiveList.GetComponentsInChildren<SkillIcon>();
                SkillGroup passiveGroup = passiveList.GetComponentInChildren<SkillGroup>();
                int passiveCount = 0;
                for (int j = 0; j < playerStat.playerAbility.skillInpo[i].skillStats.Count; j++)
                {
                    loadSkill = playerStat.playerAbility.skillInpo[i].skillStats[j];
                    passives[j].skillStat = loadSkill;
                    passives[j].LinkSkill();
                    if (loadSkill.isEquip)
                    {
                        Debug.Log(i);
                        playerStat.equipSkills[i + passiveCount] = loadSkill;
                        passiveGroup.prevIcon = passives[j];
                        passiveGroup.currentIcon = passives[j];
                        passiveGroup.passiveCount = passives[j].skillStat.passiveCount;
                        passiveGroup.IconReset();
                        passiveCount++;
                    }
                }
            }
        }

        gameObject.SetActive(false);
    }

    public void ResetSkill()
    {
        for (int i = 0; i < equipSkills.Length; i++)
        {
            if (equipSkills[i] == null)
                continue;

            equipSkills[i].LinkSkill();
            equipSkills[i].SkillLevelCheck();
        }
    }

    private void OnDisable()
    {
        talents.SetActive(false);
        skillList[0].gameObject.SetActive(false);
        skillList[1].gameObject.SetActive(false);
        skillList[2].gameObject.SetActive(false);
        passiveList.gameObject.SetActive(false);
    }
}
