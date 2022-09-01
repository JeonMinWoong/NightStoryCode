using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : Singleton<DataManager>
{
    public List<PlayerAbility> playerAbilitys = new List<PlayerAbility>();
    public LoadFiles loadFiles;
    
    public string filePath;
    public bool isLoad;

    public string settingPath;


    private void Start()
    {
        filePath = Application.persistentDataPath + "/saves/";
        settingPath = Application.persistentDataPath + "/setting/";
        LoadFile();
        loadFiles = GetComponentInChildren<LoadFiles>(true);
        SettingLoad();


    }

    public void SaveFile(PlayerAbility playerA = null)
    {
        PlayerAbility playerStat = new PlayerAbility();

        if (SceneManager.GetActiveScene().name.Contains("Map_"))
            playerStat = GameController.Instance.player.GetComponent<PlayerStat>().playerAbility;
        else
            playerStat = playerA;

        playerStat.date = DateTime.Now.ToString("yyMMdd");

        playerAbilitys[playerStat.saveCount - 1] = playerStat;

        Debug.Log("완성");

        if(!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);

        string path = Path.Combine(filePath + playerStat.saveCount + ".json");
        string json = JsonUtility.ToJson(playerStat, true);
        string code = AES.AESEncrypt128(json);

        File.WriteAllText(path, code);

        SettingSave();
    }

    void SettingSave()
    {
        SettingData settingData = new SettingData();

        settingData = UIManager.Instance.settingUI.GetSetting();

        if (!Directory.Exists(settingPath))
            Directory.CreateDirectory(settingPath);

        string path = Path.Combine(settingPath + "Setting.json");
        string json = JsonUtility.ToJson(settingData, true);

        File.WriteAllText(path, json);
    }

    public void LoadFile()
    {
        PlayerAbility playerStat = new PlayerAbility();

        for (int i = 1; i <= playerAbilitys.Count; i++)
        {
            string path = Path.Combine(filePath, i + ".json");

            if (File.Exists(path))
            {
                string code = File.ReadAllText(path);
                string json = AES.AESDecrypt128(code);

                //string loadJson = File.ReadAllText(path);
                playerStat = JsonUtility.FromJson<PlayerAbility>(json);
                playerAbilitys[playerStat.saveCount - 1] = playerStat;
            }
            else
                Debug.Log(i + " 파일 없음");
        }
    }

    public void SettingLoad()
    {
        SettingData settingData = new SettingData();

        string path = Path.Combine(settingPath, "Setting.json");

        if (File.Exists(path))
        {
            string loadJson = File.ReadAllText(path);
            settingData = JsonUtility.FromJson<SettingData>(loadJson);
            UIManager.Instance.settingUI.SetSetting(settingData);
        }
        else
            Debug.Log("설정 파일 없음");
    }

    public void OnSaveUI()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        loadFiles.uitype = LoadFiles.UIType.SAVE;
        loadFiles.SetLoad();
    }

    public void OnLoadUI()
    {
        LoadFile();
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        loadFiles.uitype = LoadFiles.UIType.LOAD;
        loadFiles.SetLoad();
    }
}
