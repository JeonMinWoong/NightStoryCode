using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class SettingData
{
    public float[] soundSetData = new float[3];
    public bool[] IssoundCheckData = new bool[3];
    public int selectWindowData;
    public bool isWindowData = true;
}

public class Setting : MonoBehaviour
{
    [SerializeField]
    AudioMixerGroup masterMixer;
    public Slider[] soundSlider;
    public Toggle[] soundToggle;

    public GameObject settingOJ;
    public Dropdown resolutionArr;
    public Toggle fullToggle;
    public Toggle windowToggle;
    int optionCount;
    [SerializeField]
    bool isFull = true;
    List<Resolution> resolutions = new List<Resolution>();

    [SerializeField]
    SettingData settingData = new SettingData();

    bool isLoad;

    private void Awake()
    {
        AudioInit();
        ResoulutionInit();
        DataManager.Instance.SettingLoad();
    }

    void OnEnable()
    {
        Time.timeScale = 0;
    }

    void OnDisable()
    {
        settingOJ.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnContineButton()
    {
        AudioManager.Instance.PlaySound("SettingClose", Camera.main.transform.position);
        gameObject.SetActive(false);
    }

    public void OnLoadButton()
    {
        AudioManager.Instance.PlaySound("Save_Load_Click", Camera.main.transform.position);
        DataManager.Instance.OnLoadUI();
    }
    public void OnMainButton()
    {
        AudioManager.Instance.PlaySound("Save_Load_Click",Camera.main.transform.position);
        SceneManager.LoadScene("Main");
        Destroy(GameManager.Instance.gameObject);
        Destroy(DBManager.Instance.gameObject);
    }

    public void OnSettingButton()
    {
        AudioManager.Instance.PlaySound("Ok", Camera.main.transform.position);
        settingOJ.SetActive(true);
    }

    public void OnEndButton()
    {
        AudioManager.Instance.PlaySound("MainMenuStart", Camera.main.transform.position);
        Application.Quit();
    }
    void AudioInit()
    {
        masterMixer = AudioManager.Instance.GetComponent<AudioSource>().outputAudioMixerGroup;
        for (int i = 0; i < soundSlider.Length; i++)
        {
            int count = i;
            soundSlider[i].onValueChanged.AddListener((float value) => AudioControl(count));
            soundToggle[i].onValueChanged.AddListener((bool isActive) => ToggleControl(count));
        }
    }

    void ResoulutionInit()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].width < 800)
                continue;

            if (resolutions.Count > 0)
            {
                for (int j = resolutions.Count - 1; j < resolutions.Count; j++)
                {
                    //Debug.Log(Screen.resolutions[i].width+ " / " + resolutions[j].width);
                    if (Screen.resolutions[i].refreshRate >= 60 && resolutions[j].width < Screen.resolutions[i].width)
                    {
                        resolutions.Add(Screen.resolutions[i]);
                        break;
                    }
                }
            }
            else
            {
                resolutions.Add(Screen.resolutions[i]);
            }
        }

        resolutionArr.options.Clear();

        for (int i = 0; i < resolutions.Count; i++)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = resolutions[i].width + " x " + resolutions[i].height;
            resolutionArr.options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                resolutionArr.value = optionCount;
            optionCount++;
        }
        resolutionArr.RefreshShownValue();
        isFull = fullToggle.isOn;
        if (!isLoad)
            ToggleWindow();
        ResoulutionCount();
    }
    void AudioControl(int count)
    {
        float sound = 0;
        string stringTarget = "";

        switch (count)
        {
            case 0:
                sound = soundSlider[0].value;
                settingData.soundSetData[0] = sound;
                stringTarget = "Master";
                break;
            case 1:
                sound = soundSlider[1].value;
                settingData.soundSetData[1] = sound;
                stringTarget = "Music";
                break;
            case 2:
                sound = soundSlider[2].value;
                settingData.soundSetData[2] = sound;
                stringTarget = "Eff";
                break;
            default:
                break;
        }

        if (sound == -40f)
            masterMixer.audioMixer.SetFloat(stringTarget, -80);
        else
            masterMixer.audioMixer.SetFloat(stringTarget, sound);
    }
    void ToggleControl(int count)
    {
        bool isSound = false;
        string stringTarget = "";

        switch (count)
        {
            case 0:
                isSound = soundToggle[0].isOn;
                settingData.IssoundCheckData[0] = isSound;
                stringTarget = "Master";
                break;
            case 1:
                isSound = soundToggle[1].isOn;
                settingData.IssoundCheckData[1] = isSound;
                stringTarget = "Music";
                break;
            case 2:
                isSound = soundToggle[2].isOn;
                settingData.IssoundCheckData[2] = isSound;
                stringTarget = "Eff";
                break;
            default:
                break;
        }

        if (isSound)
        {
            Debug.Log(stringTarget +"음소거");
            masterMixer.audioMixer.SetFloat(stringTarget, -80); 
            if (!isLoad)
                AudioManager.Instance.PlaySound("CheckOn", Camera.main.transform.position);
        }
        else
        {
            Debug.Log(stringTarget + "해제");
            masterMixer.audioMixer.SetFloat(stringTarget, soundSlider[count].value);
            if (!isLoad)
                AudioManager.Instance.PlaySound("CheckOff", Camera.main.transform.position);
        }
    }
    public void ResoulutionCount()
    {
        if (!isLoad)
            AudioManager.Instance.PlaySound("Ok", Camera.main.transform.position);
        optionCount = resolutionArr.value;
        settingData.selectWindowData = resolutionArr.value;
        Screen.SetResolution(resolutions[optionCount].width, resolutions[optionCount].height, isFull);
    }

    public void ToggleWindow()
    {
        if(!isLoad)
            AudioManager.Instance.PlaySound("Ok", Camera.main.transform.position);
        isFull = fullToggle.isOn;
        Screen.fullScreen = isFull;
        settingData.isWindowData = isFull;
    }

    public void ButtonClose()
    {
        AudioManager.Instance.PlaySound("Ok", Camera.main.transform.position);
    }

    public SettingData GetSetting()
    {
        SettingData newSettingData = new SettingData();

        newSettingData = settingData;

        return newSettingData;
    }

    public void SetSetting(SettingData _settingData)
    {
        settingData = _settingData;
        isLoad = true;
        masterMixer = AudioManager.Instance.GetComponent<AudioSource>().outputAudioMixerGroup;
        ResoulutionInit();
        
        for (int i = 0; i < soundSlider.Length; i++)
        {
            soundSlider[i].value = settingData.soundSetData[i];
            AudioControl(i);
        }

        for (int i = 0; i < soundToggle.Length; i++)
        {
            soundToggle[i].isOn = settingData.IssoundCheckData[i];
            ToggleControl(i);
        }

        resolutionArr.value = settingData.selectWindowData;

        fullToggle.isOn = settingData.isWindowData;
        windowToggle.isOn = !settingData.isWindowData;
        ToggleWindow();
        ResoulutionCount();
        isLoad = false;
    }
}
