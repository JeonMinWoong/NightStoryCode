using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class SkillStat
{
    public bool isGet;
    public bool isEquip;

    public int skillNumber;
    public int skillLevel = 1;
    public int maxSkillLevel;
    public Enums.SkillType type;
    public Enums.SkillActiveType activeType;
    public string skillName;
    public float coolTime;
    public int useEnergy;
    public float fullDamage;
    public float defultDamage;
    public float duration;
    [TextArea(3, 10)]
    public string wear;
    [TextArea(3, 10)]
    public string explanation;
    public bool isEmpty;
    public int passiveCount;

    public float passiveValue;

    public int needPoint;
    public int needLevel;
    public int needLevelPlus;
    public int nextLevel;
    public bool isMax;

    public SkillStat Copy()
    {
        SkillStat newSkillStat = new SkillStat();

        newSkillStat.isGet = isGet;
        newSkillStat.isEquip = isEquip;
        newSkillStat.skillNumber = skillNumber;
        newSkillStat.skillLevel = skillLevel;
        newSkillStat.maxSkillLevel = maxSkillLevel;
        newSkillStat.type = type;
        newSkillStat.activeType = activeType;
        newSkillStat.skillName = skillName;
        newSkillStat.coolTime = coolTime;
        newSkillStat.useEnergy = useEnergy;
        newSkillStat.fullDamage = fullDamage;
        newSkillStat.defultDamage = defultDamage;
        newSkillStat.duration = duration;
        newSkillStat.wear = wear;
        newSkillStat.explanation = explanation;
        newSkillStat.needLevel = needLevel;
        newSkillStat.needPoint = needPoint;
        newSkillStat.needLevelPlus = needLevelPlus;
        newSkillStat.nextLevel = nextLevel;
        newSkillStat.isMax = isMax;
        newSkillStat.passiveValue = passiveValue;

        return newSkillStat;
    }

}

public class SkillIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum SkillStep
    {
        None,
        Basic,
        Middle,
        High = 4
    }

    PlayerStat playerStat;

    public SkillStat skillStat;

    GameObject dummyParent;
    GameObject skillInpo;
    SkillList skillList;
    SkillPoint skillPoint;
    Button[] skillLevelButton = new Button[2];
    Text skillLvText;
    Sprite lockImage;
    Sprite noneImage;
    PointerEventData.InputButton btn1 = PointerEventData.InputButton.Left;
    PointerEventData.InputButton btn2 = PointerEventData.InputButton.Right;



    public SkillStep skillStep = SkillStep.None;

    private void Start()
    {
        dummyParent = GameObject.Find("PlayerCanvas");
        playerStat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStat>();
        skillInpo = dummyParent.transform.GetChild(3).gameObject;

        if (skillStat.isEmpty)
            return;

        skillList = dummyParent.GetComponentInChildren<SkillList>(true);
        skillPoint = skillList.GetComponentInChildren<SkillPoint>();
        skillLvText = transform.GetComponentInChildren<Outline>().gameObject.GetComponent<Text>();
        lockImage = Resources.Load<Sprite>("Sprite/Lock");
        noneImage = Resources.Load<Sprite>("Sprite/None");
        transform.GetChild(0).GetComponent<Image>().sprite = lockImage;
        skillLvText.gameObject.SetActive(false);
        SkillStepInit();
        SkillLevelButtonInit();
    }
    private void OnEnable()
    {
        LinkSkill();
    }
    public void LinkSkill()
    {
        if (playerStat == null)
            return;

        switch (skillStat.skillNumber)
        {
            case 0:
                skillStat.fullDamage = skillStat.defultDamage * skillStat.skillLevel + playerStat.playerAbility.attack * 1.25f;
                skillStat.activeType = Enums.SkillActiveType.Trigger;
                break;
            case 1:
                skillStat.fullDamage = skillStat.defultDamage * skillStat.skillLevel + playerStat.playerAbility.attack * 1.5f;
                skillStat.activeType = Enums.SkillActiveType.Trigger;
                break;
            case 2:
                skillStat.activeType = Enums.SkillActiveType.Buff;
                break;
            case 27:
            case 28:
            case 29:
            case 30:
            case 31:
            case 32:
            case 34:
                skillStat.passiveValue = skillStat.defultDamage * skillStat.skillLevel;
                break;
            case 33:
                skillStat.passiveValue = skillStat.defultDamage * skillStat.skillLevel;
                skillStat.fullDamage = skillStat.defultDamage * skillStat.skillLevel * 0.05f;
                break;
            case 35:
                skillStat.passiveValue = skillStat.defultDamage * skillStat.skillLevel;
                skillStat.fullDamage = skillStat.defultDamage * skillStat.skillLevel;
                break;
            case 36:
                skillStat.passiveValue = skillStat.defultDamage * skillStat.skillLevel;
                skillStat.fullDamage = skillStat.defultDamage * skillStat.skillLevel * 0.5f;
                break;
            default:
                break;
        }

        if (!skillStat.isEquip)
        {
            if(skillStat.type == Enums.SkillType.Passive)
                PassiveSet(false);
            return;
        }
        
        switch (skillStep)
        {
            case SkillStep.None:
                playerStat.equipSkills[skillStat.passiveCount + 3] = skillStat;
                PassiveSet(true);
                break;
            case SkillStep.Basic:
                playerStat.equipSkills[0] = skillStat;
                break;
            case SkillStep.Middle:
                playerStat.equipSkills[1] = skillStat;
                break;
            case SkillStep.High:
                playerStat.equipSkills[2] = skillStat;
                break;
            default:
                break;
        }
    }

    void PassiveSet(bool isEquip)
    {
        switch (skillStat.skillNumber)
        {
            case 27:
                if (isEquip)
                    playerStat.trapResistance = (byte)skillStat.passiveValue;
                else
                    playerStat.trapResistance = 0;
                break;
            case 28:
                if (isEquip)
                    playerStat.armorLgnore = (byte)skillStat.passiveValue;
                else
                    playerStat.armorLgnore = 0;
                break;
            case 29:
                if (isEquip)
                    playerStat.poisonDamage = skillStat.passiveValue;
                else
                    playerStat.poisonDamage = 0;
                break;
            case 30:
                if (isEquip)
                    playerStat.goldenRule = (byte)skillStat.passiveValue;
                else
                    playerStat.goldenRule = 0;
                break;
            case 31:
                if (isEquip)
                    playerStat.soul = (byte)skillStat.passiveValue;
                else
                    playerStat.soul = 0;
                break;
            case 32:
                if (isEquip)
                    playerStat.potionPlus = (byte)skillStat.passiveValue;
                else
                    playerStat.potionPlus = 0;
                break;
            case 33:
                if (isEquip)
                    playerStat.staminaPlus = (byte)skillStat.passiveValue;
                else
                    playerStat.staminaPlus = 0;
                playerStat.PlusStamina();
                break;
            case 34:
                if (isEquip)
                    playerStat.counter = (byte)skillStat.passiveValue;
                else
                    playerStat.counter = 0;
                break;
            case 35:
                if (isEquip)
                {
                    playerStat.blessing = (byte)skillStat.passiveValue;
                    playerStat.playerH.Blessing();
                }
                else
                {
                    playerStat.blessing = 0;
                    if (playerStat.playerH.coBlessing != null)
                    {
                        playerStat.playerH.StopCoroutine(playerStat.playerH.coBlessing);
                        playerStat.playerH.coBlessing = null;
                    }
                }
                break;
            case 36:
                if (isEquip)
                    playerStat.dashTime = (byte)skillStat.passiveValue;
                else
                    playerStat.dashTime = 0;
                break;
            default:
                break;
        }

        //Debug.Log(skillStat.passiveValue);
    }

    void Update()
    {
        if (skillStat.isEmpty)
            return;

        if (skillStat.isGet == true)
        {
            Skill_Level_Button_Check();

            if (skillLvText.text.Contains(skillStat.skillLevel.ToString()) == false)
            {
                if(skillStat.isMax == false)
                    skillLvText.text = "LV " + skillStat.skillLevel.ToString();
                else
                    skillLvText.text = "MAX";
            }
            if (transform.GetChild(0).GetComponent<Image>().sprite.name.Contains("None") == false)
            {
                transform.GetChild(0).GetComponent<Image>().sprite = noneImage;
                skillLvText.gameObject.SetActive(true);
            }

            if (skillStat.isMax == false && skillStat.skillLevel == skillStat.maxSkillLevel)
            {
                skillStat.isMax = true;
            }
            else if(skillStat.isMax == true && skillStat.skillLevel != skillStat.maxSkillLevel)
            {
                skillStat.isMax = false;
            }
        }
    }

    void SkillStepInit()
    {
        string skillStepString = transform.parent.name;

        if (skillStat.type == Enums.SkillType.Active)
            skillStepString = skillStepString.Replace("Active_", "");
        else
            skillStepString = skillStepString.Replace("Passives_", "");

        if (skillStep == SkillStep.None)
        {
            skillStep = (SkillStep)Enum.Parse(typeof(SkillStep), skillStepString);

            switch (skillStep)
            {
                case SkillStep.None:
                    skillStat.maxSkillLevel = 7;
                    skillStat.needLevel = 5;
                    skillStat.needLevelPlus = 3;
                    break;
                case SkillStep.Basic:
                    skillStat.maxSkillLevel = 11;
                    skillStat.needLevel = 2;
                    skillStat.needLevelPlus = 2;
                    break;
                case SkillStep.Middle:
                    skillStat.maxSkillLevel = 6;
                    skillStat.needLevel = 6;
                    skillStat.needLevelPlus = 4;
                    break;
                case SkillStep.High:
                    skillStat.maxSkillLevel = 4;
                    skillStat.needLevel = 12;
                    skillStat.needLevelPlus = 6;
                    break;
            }
        }
    }

    void SkillLevelButtonInit()
    {
        skillLvText.text = "LV " + skillStat.skillLevel.ToString();

        for (int i = 0; i < skillLevelButton.Length; i++)
        {
            skillLevelButton[i] = transform.GetChild(2).GetChild(i).GetComponent<Button>();
        }
        skillLevelButton[0].onClick.AddListener(Skill_Level_UP);
        skillLevelButton[1].onClick.AddListener(Skill_Level_Down);

        if (skillStat.isGet == false)
        {
            for (int i = 0; i < skillLevelButton.Length; i++)
            {
                skillLevelButton[i].enabled = false;
                skillLevelButton[i].GetComponent<SkillLevelButton>().ButtonDeActive();
            }
        }

        int value = (int)skillStep;

        skillStat.needPoint = skillStat.skillLevel * 5 * (value = value == 0 ? 1 : value);

        if (skillPoint.HaveSkillPoint < skillStat.needPoint)
        {
            for (int i = 0; i < skillLevelButton.Length; i++)
            {
                skillLevelButton[i].enabled = false;
                skillLevelButton[i].GetComponent<SkillLevelButton>().ButtonDeActive();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (transform.GetChild(0).GetComponent<Image>().sprite.name.Contains("Lock") == true)
            return;

        skillInpo.transform.position = gameObject.transform.position;

        if (skillInpo.transform.localPosition.y <= -68)
            skillInpo.transform.GetChild(0).localPosition = new Vector2(skillInpo.transform.GetChild(0).localPosition.x, skillInpo.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
        else
            skillInpo.transform.GetChild(0).localPosition = new Vector2(skillInpo.transform.GetChild(0).localPosition.x, 0);

        skillInpo.SetActive(true);

        SkillInpoCheck();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TextGroup inpo = skillInpo.GetComponent<TextGroup>();

        for (int i = 0; i < inpo.textGroup.Count; i++)
        {
            inpo.textGroup[i].text = "";
        }
        inpo.skill_Icon.sprite = null;
        skillInpo.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (skillStat.isGet == false)
            return;

        if (skillStat.isEquip)
            return;

        if ((eventData.button == btn1 && eventData.clickCount == 2) || eventData.button == btn2)
        {
            AudioManager.Instance.PlaySound("SkillEquip", Camera.main.transform.position);
            skillStat.isEquip = true;
            SkillGroup skillGroup = gameObject.transform.parent.GetComponent<SkillGroup>();

            skillGroup.prevIcon = this;
            skillGroup.currentIcon = this;

            skillGroup.IconReset();
            LinkSkill();
        }
    }

    void Skill_Level_Button_Check()
    {
        if(skillStat.nextLevel != skillStat.needLevel + skillStat.needLevelPlus * skillStat.skillLevel)
            skillStat.nextLevel = skillStat.needLevel + skillStat.needLevelPlus * skillStat.skillLevel;

        if (skillStat.needPoint > skillPoint.HaveSkillPoint || skillStat.nextLevel > playerStat.playerAbility.level || skillStat.isMax)
        {
            skillLevelButton[0].GetComponent<SkillLevelButton>().ButtonDeActive();
            skillLevelButton[0].enabled = false;
        }
        else if(skillStat.needPoint <= skillPoint.HaveSkillPoint && skillStat.nextLevel <= playerStat.playerAbility.level && skillStat.isMax == false)
        {
            skillLevelButton[0].GetComponent<SkillLevelButton>().ButtonActive();
            skillLevelButton[0].enabled = true;
        }
        
        if(skillStat.skillLevel <= 1)
        {
            skillLevelButton[1].GetComponent<SkillLevelButton>().ButtonDeActive();
            skillLevelButton[1].enabled = false;
        }
        else
        {
            skillLevelButton[1].GetComponent<SkillLevelButton>().ButtonActive();
            skillLevelButton[1].enabled = true;
        }
    }
    public void Skill_Level_UP()
    {
        AudioManager.Instance.PlaySound("SkillUpDown", Camera.main.transform.position);

        int value = (int)skillStep;

        skillStat.needPoint = skillStat.skillLevel * 5 * (value = value == 0 ? 1 : value);

        if (skillStat.needPoint > skillPoint.HaveSkillPoint)
            return;

        skillPoint.HaveSkillPoint -= skillStat.needPoint;
        skillStat.skillLevel++;

        LinkSkill();
        SkillInpoUpdate();

        if (!skillStat.isEquip)
            return;

        SkillLevelCheck();
    }

    public void SkillLevelCheck()
    {
        SkillGroup skillGroup = gameObject.transform.parent.GetComponent<SkillGroup>();

        if (skillStep == SkillStep.None)
        {
            skillGroup.LinkSkillLevel(skillStat.passiveCount, skillStat);
        }
        else
            skillGroup.LinkSkillLevel(3, skillStat);
    }

    public void Skill_Level_Down()
    {
        AudioManager.Instance.PlaySound("SkillUpDown", Camera.main.transform.position);

        if (skillStat.skillLevel <= 1)
            return;

        int value = (int)skillStep;

        skillStat.skillLevel--;
        skillStat.needPoint = skillStat.skillLevel * 5 * (value = value == 0 ? 1 : value);
        skillPoint.HaveSkillPoint += skillStat.needPoint;

        LinkSkill();
        SkillInpoUpdate();

        if (!skillStat.isEquip)
            return;

        SkillLevelCheck();
    }

    void SkillInpoCheck()
    {
        TextGroup inpo = skillInpo.GetComponent<TextGroup>();

        if (skillStat.skillLevel == skillStat.maxSkillLevel)
            skillStat.isMax = true;
        else
            skillStat.isMax = false;

        if (skillStat.isMax == false)
        {
            inpo.textGroup[0].text = "LV " + skillStat.skillLevel.ToString();
            inpo.textGroup[6].text = skillStat.nextLevel.ToString();
            inpo.textGroup[6].transform.parent.gameObject.SetActive(true);
            inpo.textGroup[7].text = skillStat.needPoint.ToString();
        }
        else
        {
            inpo.textGroup[0].text = "MAX";
            inpo.textGroup[6].transform.parent.gameObject.SetActive(false);
            inpo.textGroup[7].text = "-";
        }

        inpo.textGroup[1].text = skillStat.type.ToString();
        inpo.textGroup[2].text = skillStat.skillName;


        inpo.textGroup[5].text = skillStat.explanation;


        if (skillStat.type == Enums.SkillType.Passive)
        {
            string[] wearGroup = skillStat.wear.Split('#');
            if (wearGroup.Length > 1)
            {
                inpo.textGroup[4].text = "";

                if(wearGroup.Length == 2)
                    inpo.textGroup[4].text = wearGroup[0] + "<color=#05FF00>" + skillStat.passiveValue + "</color>" + wearGroup[1];
                else
                    for (int i = 0; i < wearGroup.Length; i++)
                    {
                        if (i == 2)
                            inpo.textGroup[4].text += wearGroup[i];
                        else if(i == 1)
                            inpo.textGroup[4].text += wearGroup[i] + "<color=#05FF00>" + skillStat.fullDamage + "</color>";
                        else
                            inpo.textGroup[4].text += wearGroup[i] + "<color=#05FF00>" + skillStat.passiveValue + "</color>";
                    }
            }
            else
                inpo.textGroup[4].text = skillStat.wear + skillStat.passiveValue;
            inpo.textGroup[3].transform.parent.gameObject.SetActive(false);
            inpo.textGroup[8].transform.parent.gameObject.SetActive(false);
        }
        else
        {
            if (skillStat.skillNumber != 2)
                inpo.textGroup[4].text = skillStat.wear + "<color=#FF0000>" + skillStat.fullDamage + "</color>" + " 피해를 입힌다.";
            else
                inpo.textGroup[4].text = skillStat.wear;

            inpo.textGroup[3].transform.parent.gameObject.SetActive(true);
            inpo.textGroup[8].transform.parent.gameObject.SetActive(true);
            inpo.textGroup[3].text = skillStat.coolTime.ToString("0.0") + " sec";
            inpo.textGroup[8].text = skillStat.useEnergy.ToString();
        }

        inpo.skill_Icon.sprite = GetComponent<Image>().sprite;
    }

    void SkillInpoUpdate()
    {
        int value = (int)skillStep;

        skillStat.nextLevel = skillStat.needLevel + skillStat.needLevelPlus * skillStat.skillLevel;
        skillStat.needPoint = skillStat.skillLevel * 5 * (value = value == 0 ? 1 : value);

        SkillInpoCheck();
    }

    private void OnDisable()
    {
        if (skillInpo != null)
            skillInpo.SetActive(false);
    }
}
