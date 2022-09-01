using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadFiles : MonoBehaviour
{ 
    public enum UIType
    { 
        SAVE,
        LOAD,
    }

    public UIType uitype;

    [Header("Save")]
    [SerializeField]
    GameObject saveName;
    [SerializeField]
    GameObject overwrite;

    public int selectCount;

    [Header("Load")]
    [SerializeField]
    Button[] loadButton;
    [SerializeField]
    Transform view;
    [SerializeField]
    Text[] textGroup;

    private void Awake()
    {
        loadButton = view.GetComponentsInChildren<Button>();
    }

    private void Update()
    {
        if((!saveName.activeSelf && gameObject.activeSelf) && (!overwrite.activeSelf && gameObject.activeSelf))
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                AudioManager.Instance.PlaySound("Cancel", Camera.main.transform.position);
                transform.parent.gameObject.SetActive(false);
            }
        }
        if(saveName.activeSelf)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                AudioManager.Instance.PlaySound("Cancel", Camera.main.transform.position);
                saveName.SetActive(false);
            }
        }

        if(overwrite.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                AudioManager.Instance.PlaySound("Cancel", Camera.main.transform.position);
                overwrite.SetActive(false);
            }
        }
    }

    public void FirstSave(int count)
    {
        AudioManager.Instance.PlaySound("Save_Load_Click", Camera.main.transform.position);
        if (DataManager.Instance.playerAbilitys[count - 1].saveName == "")
        {
            Debug.Log(count);
            selectCount = count;
            saveName.SetActive(true);
        }
        else
        {
            selectCount = count;
            overwrite.SetActive(true);
        }
    }

    public void OnFirstSaveNameCheck()
    {
        DataManager.Instance.isLoad = false;
        AudioManager.Instance.PlaySound("Save_Load_Click", Camera.main.transform.position);
        InputField inputValue = saveName.GetComponentInChildren<InputField>();
        PlayerAbility newPlayer = new PlayerAbility();
        newPlayer.saveName = inputValue.text;
        newPlayer.saveCount = selectCount;
        DataManager.Instance.SaveFile(newPlayer);
        inputValue.text = "";
        transform.parent.gameObject.SetActive(false);
        GameManager.Instance.PlayGame();
        saveName.SetActive(false);
    }

    public void SetLoad()
    {
        for (int i = 0; i < loadButton.Length; i++)
        {
            //Debug.Log(DataManager.Instance.playerAbilitys.Count + " / " + i);
            if (DataManager.Instance.playerAbilitys[i].saveCount != 0)
            {
                loadButton[i].transform.GetChild(0).gameObject.SetActive(true);
                loadButton[i].transform.GetChild(1).gameObject.SetActive(false);

                Slider progressSlider = loadButton[i].GetComponentInChildren<Slider>();
                textGroup = loadButton[i].GetComponentsInChildren<Text>();
                textGroup[0].text = DataManager.Instance.playerAbilitys[i].saveName;
                textGroup[2].text = (((float)DataManager.Instance.playerAbilitys[i].mapInpo / 8) * 100).ToString("0") + "%";
                textGroup[4].text = DataManager.Instance.playerAbilitys[i].date;
                textGroup[6].text = DataManager.Instance.playerAbilitys[i].level.ToString();
                textGroup[8].text = DataManager.Instance.playerAbilitys[i].coinGet.ToString();
                textGroup[10].text = (DataManager.Instance.playerAbilitys[i].playTime / 60).ToString("0");

                progressSlider.value = (float)DataManager.Instance.playerAbilitys[i].mapInpo / 8;

                int count = i + 1;

                loadButton[i].onClick.RemoveAllListeners();

                if (uitype == UIType.SAVE)
                    loadButton[i].onClick?.AddListener(() => FirstSave(count));
                else
                    loadButton[i].onClick?.AddListener(() => LoadPlayer(count));
            }
            else
            {
                loadButton[i].transform.GetChild(0).gameObject.SetActive(false);
                loadButton[i].transform.GetChild(1).gameObject.SetActive(true);
                int count = i + 1;

                loadButton[i].onClick.RemoveAllListeners();

                if (uitype == UIType.SAVE)
                {
                    loadButton[i].onClick?.AddListener(() => FirstSave(count));
                    loadButton[i].interactable = true;
                }
                else
                    loadButton[i].interactable = false;
            }
        }
    }

    void LoadPlayer(int count)
    {
        AudioManager.Instance.PlaySound("Save_Load_Click", Camera.main.transform.position);
        transform.parent.gameObject.SetActive(false);
        DataManager.Instance.isLoad = true;
        selectCount = count;
        
        if (SceneManager.GetActiveScene().name.Contains("Map_"))
            GameManager.Instance.Load();
        else
            GameManager.Instance.ManagerDestroyLoad();
    }

    public void ButtonOk()
    {
        AudioManager.Instance.PlaySound("Save_Load_Click", Camera.main.transform.position);
    }

    public void ButtonCancel()
    {
        UIManager.Instance.CancelSound();
    }
}
