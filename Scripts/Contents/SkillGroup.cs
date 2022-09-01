using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillGroup : MonoBehaviour
{
    SkillList skillList;

    SkillIcon[] skill_IconGroup;

    public SkillIcon prevIcon { get; set; }
    public SkillIcon currentIcon { get; set; }

    [SerializeField]
    bool isPassive;
    public int passiveCount { get; set; }

    public Image slotSkill;
    public Image quickSlotSkill;
    public ParticleSystem skillParticle;
    public ParticleSystem[] skillParticles;
    public Image[] passiveSlotSkill;
    public Image[] passiveQuickSlotSkill;
    void Start()
    {
        skillList = FindObjectOfType<SkillList>();

        skill_IconGroup = new SkillIcon[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            skill_IconGroup[i] = transform.GetChild(i).GetComponent<SkillIcon>();
        }
        currentIcon = null;
        IconReset();
    }

    public void IconReset()
    {
        if (isPassive == false)
        {
            for (int i = 0; i < skill_IconGroup.Length; i++)
            {
                if (skill_IconGroup[i] == currentIcon)
                {
                    skill_IconGroup[i].transform.GetChild(1).gameObject.SetActive(true);
                    slotSkill.sprite = skill_IconGroup[i].GetComponent<Image>().sprite;
                    skillParticle.gameObject.SetActive(false);
                    slotSkill.color = new Color(1, 1, 1, 1);
                    quickSlotSkill.sprite = skill_IconGroup[i].GetComponent<Image>().sprite;
                    SkillStat dummySkill = currentIcon.skillStat.Copy();
                    dummySkill.isEmpty = true;
                    SkillIcon newSkill = quickSlotSkill.gameObject.AddComponent<SkillIcon>();
                    newSkill.skillStat = dummySkill;
                    quickSlotSkill.color = new Color(1, 1, 1, 1);
                    if (gameObject.name.Contains("Basic"))
                        skillList.equipSkills[0] = currentIcon;
                    else if(gameObject.name.Contains("Middle"))
                        skillList.equipSkills[2] = currentIcon;
                    else if(gameObject.name.Contains("High"))
                        skillList.equipSkills[3] = currentIcon;
                }
                else
                    skill_IconGroup[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < skill_IconGroup.Length; i++)
            {
                if (skill_IconGroup[i] == currentIcon)
                {
                    Text checkSlot = skill_IconGroup[i].GetComponentInChildren<Text>(true);

                    if (skill_IconGroup[i].transform.GetChild(1).gameObject.activeSelf == true)
                    {
                        prevIcon.transform.GetChild(1).gameObject.SetActive(true);
                        prevIcon.GetComponent<SkillIcon>().skillStat.isEquip = true;
                        return;
                    }

                    skill_IconGroup[i].skillStat.passiveCount = passiveCount;
                    checkSlot.text = (passiveCount + 1).ToString();
                    skill_IconGroup[i].transform.GetChild(1).gameObject.SetActive(true);
                    passiveSlotSkill[passiveCount].sprite = skill_IconGroup[i].GetComponent<Image>().sprite;
                    skillParticles[passiveCount].gameObject.SetActive(false);
                    passiveSlotSkill[passiveCount].color = new Color(1, 1, 1, 1);
                    passiveQuickSlotSkill[passiveCount].sprite = skill_IconGroup[i].GetComponent<Image>().sprite;
                    passiveQuickSlotSkill[passiveCount].color = new Color(1, 1, 1, 1);
                    SkillStat dummySkill = currentIcon.skillStat.Copy();
                    dummySkill.isEmpty = true;
                    SkillIcon newSkill = passiveQuickSlotSkill[passiveCount].gameObject.AddComponent<SkillIcon>();
                    newSkill.skillStat = dummySkill;
                    prevIcon = null;
                    if (passiveCount == 0)
                        skillList.equipSkills[1] = currentIcon;
                    else if (passiveCount == 1)
                        skillList.equipSkills[4] = currentIcon;
                    else if (passiveCount == 2)
                        skillList.equipSkills[5] = currentIcon;
                }
            }
            for (int i = 0; i < skill_IconGroup.Length; i++)
            {
                if (prevIcon != null)
                    break;

                if (skill_IconGroup[i] != currentIcon)
                {
                    if (skill_IconGroup[i].GetComponent<SkillIcon>().skillStat.isEquip == false)
                        skill_IconGroup[i].transform.GetChild(1).gameObject.SetActive(false);
                    else
                    {
                        Text checkSlot = skill_IconGroup[i].GetComponentInChildren<Text>(true);
                        int textCount = passiveCount + 1;
                        if (checkSlot.text.Contains(textCount.ToString()))
                        {
                            skill_IconGroup[i].transform.GetChild(1).gameObject.SetActive(false);
                            skill_IconGroup[i].GetComponent<SkillIcon>().skillStat.isEquip = false;
                            skill_IconGroup[i].GetComponent<SkillIcon>().LinkSkill();
                            skill_IconGroup[i].skillStat.passiveCount = 0;
                            SkillIcon newSkill = passiveQuickSlotSkill[passiveCount].gameObject.GetComponent<SkillIcon>();
                            Destroy(newSkill);
                        }
                    }
                }
            }
        }
    }

    public void LinkSkillLevel(int count, SkillStat value)
    {
        SkillStat dummySkill = value.Copy();

        if (count > 2)
        {
            if (quickSlotSkill.GetComponent<SkillIcon>() == null)
                return;

            quickSlotSkill.GetComponent<SkillIcon>().skillStat = dummySkill;
            quickSlotSkill.GetComponent<SkillIcon>().skillStat.isEmpty = true;
        }
        else
        {
            if (passiveQuickSlotSkill[count].GetComponent<SkillIcon>() == null)
                return;

            passiveQuickSlotSkill[count].GetComponent<SkillIcon>().skillStat = dummySkill;
            passiveQuickSlotSkill[count].GetComponent<SkillIcon>().skillStat.isEmpty = true;
        }
    }
}
