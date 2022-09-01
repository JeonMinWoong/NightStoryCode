using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums
{
    public enum StoreType
    {
        None,
        Blacksmith,
        Trinkets,
        Traveling,
        Gravekeeper,
    }
    public enum NpcType
    {
        Main,
        Blacksmith,
        Trinkets,
        Traveling,
        Gravekeeper,
        WizardHouse,
        Ashes,
    }

    public enum GameMode
    {
        Selet,
        battle,
    }
    public enum State
    {
        Stop,
        Idle,
        Run,
        Action,

    }

    public enum MonsterType
    {
        None,
        Small,
        Normal,
        Big,
    }

    public enum Stage
    {
        Stage_1,
        Stage_2,
        None = 100,
    }

    public enum SkillGage
    {
        None,
        Max,
        MaxIng,
    }

    public enum StoreButton
    {
        All,
        Weapon,
        Armor,
        Necklace_Ring,
        Con,
        Etc,
    }

    public enum EquipType
    {
        Weapon,
        Helmet,
        Chest,
        Pants,
        Shoes,
        Gloves,
        Necklace,
        Ring,
    }

    public enum MapType
    {
        Empty,
        All,
        One,
    }

    public enum SkillType
    {
        Active,
        Passive
    }
    public enum SkillActiveType
    {
        None,
        Trigger,
        Buff,
    }
    public enum MapStory
    {
        None,
        Map_1,
        Map_2,
        Map_3,
        Map_4,
        Map_5,
        Map_6,
        Map_7,
    }
}
