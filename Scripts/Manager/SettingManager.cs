using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : Singleton<SettingManager>
{
    public enum Language
    {
        Korea,
        English,
    }


    public Language languageSetting = Language.Korea;
}

